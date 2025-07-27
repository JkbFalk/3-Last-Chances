using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_Flamethrower : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_Flamethrower(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.1f);
        DamageSources.Add(new DamageSource(50, 50, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        NameOfAnimationToAutoPlay = "FlamethrowerNPC";
    }

    public void ResetTargets() {
        if(_aoe != null && _aoe.gameObject != null && _aoe.gameObject.IsDestroyed() == false) {
            ResetPotentialTargets();
        }
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlamethrowerNPC");
        GameController.Instance.WaitAndRunMethod(0.01f, AdjustTransform);
        EventManager.OneTenthSecondElapsedInGame.AddListener(ResetTargets);
        GameController.Instance.WaitAndRunMethod(4.5f, new System.Action(() => { EventManager.OneTenthSecondElapsedInGame.RemoveListener(ResetTargets); }));
    }

    public void AdjustTransform() {
        _aoe.transform.localEulerAngles = new Vector3(0, 0, -90);
        _aoe.transform.localPosition = new Vector2(0.8f, 0);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_aoe != null && _aoe.IsDestroyed() == false) {
            _aoe.MakeObjectDisappear();
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(5 * User.MagicStagger.Current / 100, new(this)));
    }
}