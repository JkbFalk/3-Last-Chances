using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameWhip : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public GameObject Chain;
    private Unit EnemyHit = null;

    public NPCAbility_FlameWhip(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Impact/Whip2", 0.8f);
        DamageSources.Add(new DamageSource(150, 350, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Chain = Utils.CreateVisualEffect(new(this), "FlameChain");
        Chain.gameObject.name = "VisualEffect_FlameChain";
        Chain.transform.SetParent(User.transform);
        Chain.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        User.Animator.Rebind();
        GameController.Instance.WaitAndRunMethod(0.01f, StartAnimation);
    }

    public void StartAnimation() {
        User.PlayAnimation("FlameWhip", 0);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(Chain != null && Chain.gameObject.IsDestroyed() == false) {
            Chain.GetComponentInChildren<AreaOfEffect>().DealingDamage = false;
            Chain.GetComponent<TemporaryObject>().MakeObjectDisappear(1);
        }
    }

    public override void CallAbilityEvent1()
    {
        Chain.GetComponentInChildren<AreaOfEffect>().DealingDamage = true;
        AimWhipAtTarget();
    }

    public void AimWhipAtTarget() {
        if(User.Actions.CurrentAbilityBeingPerformed == this) {
            Chain.transform.Find("New CCDSolver2D/Target").transform.position = Target.transform.position;
            GameController.Instance.WaitAndRunMethod(0.05f, AimWhipAtTarget);
        }
    }

    public override void CallAbilityEvent2()
    {
        if(EnemyHit == null) {
            EndThisAbility();
        }
        else {
            EnemyHit.PushIntoPosition(User.transform.position - new Vector3(0.5f * (User.Actions.IsFlipped ? -1 : 1), 0), this);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        EnemyHit = damage.TargetOfDamage;
        Utils.PlaySoundEffect(damage.TargetOfDamage.AudioSource, "Impact/Whip1", 0.9f);
        damage.TargetOfDamage.AddEffect(new Effect_Burn(25 * User.MagicStagger.Current / 100, new(this)));
        if(damage.DamageWasBlocked == false) {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 4);
        }
    }
}