using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FireDance : Ability {

    public static float Cooldown = 18;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public Projectile Missile;

    public NPCAbility_FireDance(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 20, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.1f;
        Properties.Add(Property.ImmuneToFlinch);
        AddCustomSound("Use1", "Fire/Fire3", 0.35f);
        AddCustomSound("Use2", "Fire/Fire4", 0.35f);
        AddCustomSound("Use3", "Fire/Fire5", 0.35f);
        AddCustomSound("Use4", "Fire/Fire6", 0.35f);
        AddCustomSound("Use5", "Fire/Fire7", 0.35f);
        AddCustomSound("Use6", "Fire/Fire8", 0.35f);
        AddCustomSound("Use7", "Fire/Fire9", 0.35f);
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };

    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Missile = Utils.CreateProjectile(new(this), "FireDance");
        Missile.transform.SetParent(User.transform);
        Missile.name = "Projectile_FireDance";
        Missile.CleanUpAfter(12 / User.MagicAttackSpeed.Current);
        User.Animator.Rebind();
        PerformActionAfterIntervals(5, 1);
        GameController.Instance.WaitAndRunMethod(0.01f, StartAnimation);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(Missile != null) {
            Missile.CleanUpObject();
        }
    }

    public void StartAnimation() {
        User.PlayAnimation("FireDance", 0);
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        Missile.DealingDamage = true;
    }

    public override void CallAbilityEvent2()
    {
        if(Missile != null && Missile.IsDestroyed() == false) {
            Missile.MakeObjectDisappear();
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(15 * User.MagicStagger.Current / 100, new(this)));
    }

    public override void ActionToPerformAfterIntervals() {
        ResetPotentialTargets();
    }
}