using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_PlundererInfuse : Ability {
    public static float Cooldown = 10;
    private bool _isOmni = false;
    public static new bool DoesNotRequireTarget = true;
    private Ability.AbilityFamily _blockedFamily;
    public NPCAbility_PlundererInfuse(Unit ability_user) : base(ability_user) {
        TransitionOutOfAnimationDuration = 0;
        Properties.Add(Property.ImmuneToFlinch);
    }

    public override void CallAbilityEvent1()
    {
        _blockedFamily = GetValidFamilyToBlock();
        _isOmni = User.gameObject.name.Contains("Blaine") == false && UnityEngine.Random.Range(0, 100) < 20;
        if(_isOmni) {
            foreach(Ability.AbilityFamily family in new List<Ability.AbilityFamily>{Ability.AbilityFamily.Anima, Ability.AbilityFamily.Ignis, Ability.AbilityFamily.Glacies, Ability.AbilityFamily.Molis, Ability.AbilityFamily.Salutis, Ability.AbilityFamily.Tonitrui, Ability.AbilityFamily.Proprius}) {
                if(SaveFile.Instance.DifficultyLevel >= 2) {
                    Player.Instance.AddEffect(new Effect_BlockTechniquesFromGivenFamily(family, new(this)) {UIText=Label.Get("Effect_TechniquesBlockedIndicator")}, 20);
                }
                else {
                    Player.Instance.AddEffect(new Effect_PlundererAbilityAmplify(new(this)) {AmplifiedFamily = family, PercentageAmount = -50}, 20);
                }
            }
        }
        else {
            if(SaveFile.Instance.DifficultyLevel >= 2) {
                Player.Instance.AddEffect(new Effect_BlockTechniquesFromGivenFamily(_blockedFamily, new(this)) {UIText=Label.Get("Effect_TechniquesBlockedIndicator")}, 20);
            }
            else {
                Player.Instance.AddEffect(new Effect_PlundererAbilityAmplify(new(this)) {AmplifiedFamily = _blockedFamily, PercentageAmount = -75}, 20);
            }
        }
        GameObject vfx = Utils.CreateVisualEffect(new(this), "Plunderer_" + (_isOmni ? "Omni" : _blockedFamily));
        vfx.transform.SetParent(User.SpriteRenderers["Heavy"].Bone);
        vfx.transform.localEulerAngles = new Vector3(0, 0, 90);
        vfx.transform.localPosition = new Vector2(1.3f, 0);
        if(SaveFile.Instance != null && SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission is Mission_Ignis3) {
            SaveFile.Instance.ChangeIgnisEnergy(-100);
        }
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void CallAbilityEvent2()
    {
        EndThisAbility();
        Type followUp = AbilityTypeRegistry.GetPlundererFollowUp(_blockedFamily, _isOmni);
        if (followUp == null)
        {
            return;
        }
        User.Actions.UseAbility(followUp);
        User.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.Property.Unstoppable);
    }

    public Ability.AbilityFamily GetValidFamilyToBlock() {
        List<Type> valid_abilities = new();
        foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            if(ability.Type != null) {
                valid_abilities.Add(ability.Type);
            }
        }
        if(valid_abilities.Count == 0) {
            return new List<Ability.AbilityFamily>{Ability.AbilityFamily.Anima, Ability.AbilityFamily.Ignis, Ability.AbilityFamily.Glacies, Ability.AbilityFamily.Molis, Ability.AbilityFamily.Salutis, Ability.AbilityFamily.Tonitrui, Ability.AbilityFamily.Proprius}[UnityEngine.Random.Range(0, 7)];
        }
        return Ability.GetFamily(valid_abilities[UnityEngine.Random.Range(0, valid_abilities.Count)]);
    }
}