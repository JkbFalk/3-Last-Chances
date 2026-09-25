using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_PlundererProprius : Ability {
    public static AbilityFamily Family = AbilityFamily.Proprius;
    private AreaOfEffect _aoe;
    public NPCAbility_PlundererProprius(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Magic;
        DamageSources.Add(new DamageSource(200, 400, Constants.DamageType.Heavy) {KnockbackInMeters = 3f});
        WaitTimeBeforeNextAction = 0.1f;
        AddCustomSound("Rotate", "Greatsword/Greatsword_Swing21", 0.4f);
        Properties.Add(Property.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RootedInPlace(new(this)) };
        TransitionIntoAnimationDuration = 0;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "PlundererProprius");
        _aoe.transform.localPosition = Vector2.zero;
        GameController.Instance.WaitAndRunMethod(0.01f, AdjustRotation);
    }

    public override void CallAbilityEvent2()
    {
        _aoe.DealingDamage = false;
    }

    public void AdjustRotation() {
        _aoe.transform.localEulerAngles = Vector3.zero;
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_aoe != null && !_aoe== null) {
            _aoe.MakeObjectDisappear();
        }
    }
}