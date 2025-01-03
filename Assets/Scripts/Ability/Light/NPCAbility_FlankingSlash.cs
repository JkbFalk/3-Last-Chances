using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlankingSlash : Ability {
    public static float Cooldown = 6;
    private Projectile _projectile;

    public NPCAbility_FlankingSlash(Unit ability_user) : base(ability_user) {
        AddCustomSound("Throw", "TwinBlades/TwinBlades_Swing9", 0.5f);
        AddCustomSound("Fly", "Daggers/Daggers_Swing9", 0.4f);
        DamageSources.Add(new DamageSource(50, 400, Constants.DamageType.Light));
        DamageSources.Add(new DamageSource(200, 0, Constants.DamageType.Light, "FlankingBlade"));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
    }

    public override void CallAbilityEvent1()
    {
        _projectile = Utils.CreateProjectile(new(this), "FlankingBlade");
        _projectile.HomingOntoUnit = User.CurrentTarget;
        Utils.CopyGameObjectAppearance(_projectile.gameObject, User.SpriteRenderers["Light Left"].SpriteResolver.gameObject, false);
        _projectile.transform.localScale = User.SpriteRenderers["Light Left"].Bone.transform.localScale;
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(450, 80, 30);
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(!damage.DamagingObject.gameObject.name.Contains("FlankingBlade")) {
            return;
        }
        if(Utils.CheckIfGameObjectIsBehindUnit(damage.DamagingObject.gameObject, damage.TargetOfDamage)) {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 2);
            User.PlayAnimation("FlankingSlash", 0.05f, 0.8f);
        }
        else {
            User.AddEffect(new Effect_ChangeStat(User.LightAttackSpeed, new(this)) {PercentageAmount=50}, 1);
            User.PlayAnimation("FlankingSlash", 0.05f, 0.8f);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_projectile != null && _projectile.IsDestroyed() == false) {
            _projectile.MakeObjectDisappear(0);
        }
    }
}