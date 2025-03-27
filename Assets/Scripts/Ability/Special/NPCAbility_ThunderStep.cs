using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_ThunderStep : Ability {

    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Tonitrui;
    public NPCAbility_ThunderStep(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(50, 550, Constants.DamageType.Magic) {Knockback = 500, CustomHitSound="Generic/Generic_Impact1"});
        AddCustomSound("Use", "Generic/Generic_Explosion1", 0.8f);
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
        WaitTimeBeforeNextAction = 0f;
        Properties.Add(AbilityProperty.CounteredByBlock);
    }

    public override void CallAbilityEvent1()
    {
        PlayCustomSound("Use");
        if(GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene && Area.Instance.gameObject.name.Contains("AnimaIsland")) {
            ChaseCurrentTargetAtGivenDegreeAngle(475, 35, 16);
        }
        else {
            ChaseCurrentTargetAtGivenDegreeAngle(450, 35, 15);
        }
        Utils.CreateVisualEffect(new(this), "Ryker_ThunderStep_Blast");
        Utils.CreateAreaOfEffect(new(this), "Ryker_ThunderStep", User.transform.position.x, User.transform.position.y);
        ObjectsToDestroyOnceAbilityEnds.Add(Utils.CreateVisualEffect(new(this), "Ryker_ThunderStep_Trail").GetComponent<TemporaryObject>());
        Damage self_damage = new Damage(User, this, null).SetDamageSource(User.Health.Current * 0.25f, (User.StaggerBar.Maximum - User.StaggerBar.Current) * 0.25f, Constants.DamageType.Magic);
        self_damage.PlaySoundOnEnemyHit = false;
        self_damage.CalculateDamage();
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        GameObject vfx = Utils.CreateVisualEffect(new(this), "Ryker_ThunderStep_Impact");
        vfx.transform.position = damage.TargetOfDamage.transform.position;
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 2);
    }
}