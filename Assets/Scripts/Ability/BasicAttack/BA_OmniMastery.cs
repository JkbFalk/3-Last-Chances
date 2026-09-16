using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BA_OmniMastery : BasicAttack
{

    public static bool CanBeUsedDuringOtherAbilities = true;
    public float DamageScaling = 0;
    public float SharpGiven = 0;
    private bool _gaveSharp = false;

    public BA_OmniMastery(Unit ability_user) : base(ability_user)
    {
        NameOfAnimationToAutoPlay = "OmniMastery_" + ability_user.CurrentWeaponClass;
        AddCustomSound("SwingGreatsword", "Greatsword/Greatsword_Swing20", 0.8f);
        AddCustomSound("SwingLongblade", "Greatsword/Greatsword_Swing20", 0.8f);
        AddCustomSound("SwingPolearm", "Polearm/Polearm_Swing2", 0.8f);
        AddCustomSound("SwingTwinBlades", "TwinBlades/TwinBlades_Swing5", 0.8f);
        AddCustomSound("SwingDaggers", "Daggers/Daggers_Swing1", 0.8f);
        AddCustomSound("SwingGun", "Generic/Generic_Swoosh4", 0.8f);
        AddCustomSound("SwingBow", "Generic/Generic_Swoosh4", 0.8f);
        AddCustomSound("SwingMagic", "Magic/Magic_Blast2", 0.8f);

    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Player.Instance.IsPerfectlyBlocking = true;
        Player.Instance.AddEffect(new Effect_Block(new(this)));
        if (Player.Instance.CurrentWeaponDamageType == Constants.DamageType.Ranged)
        {
            DamageSources.Add(new DamageSource(DamageScaling, DamageScaling, Constants.DamageType.Ranged, "AoE"));
        }
        else
        {
            DamageSources.Add(new DamageSource(DamageScaling, DamageScaling, Player.Instance.CurrentWeaponDamageType));
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        Player.Instance.IsPerfectlyBlocking = false;
        Player.Instance.GetEffect(typeof(Effect_Block))?.EndThisEffect();
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        if (_gaveSharp == false && damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed != null && IsCounteredByRipostes(damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed))
        {
            Player.Instance.AddEffect(new Effect_Sharp(SharpGiven, new(damage.SourceOfDamage)));
            _gaveSharp = true;
        }
    }

    private bool IsCounteredByRipostes(Ability ability)
    {
        return ability.IsNot(Property.Unstoppable) &&
                (ability.IsNot(Property.CounteredByBackstep) || ability.Is(Property.CounteredByRiposte)) &&
                (ability.IsNot(Property.CounteredByRoll) || ability.Is(Property.CounteredByRiposte));
    }
}
