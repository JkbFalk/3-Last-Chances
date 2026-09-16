using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_SpearsOfIce : Technique
{
    public static float EnergyCost = 5;
    public static float Cooldown = 3;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    public static bool IsStacksBasedTechnique = true;
    public static int MaxStacks
    {
        get
        {
            return 20;
        }
    }
    public static int UltimateMaxStacks
    {
        get
        {
            return 100;
        }
    }
    public static float _upgradeBExtraMagicInjuryScaling = 50;
    public static float _upgradeBTechniqueSpeedIncrease = 50;
    public static float _magicInjuryScaling = 50;
    public static float _magicStaggerFreezeScaling = 5;
    public static float _knockbackInMeters = 0.5f;
    public static float _upgradeAKnockbackInMeters = 2.0f;
    public static float _upgradeAPercentageOfEnemyStaggerBarStaggerDealt = 5;

    public Ability_SpearsOfIce(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        DamageSources.Add(new DamageSource((Is(Property.UpgradeB) && IsNot(Property.Ultimate)) ? _magicInjuryScaling + _upgradeBExtraMagicInjuryScaling : _magicInjuryScaling, 0, Constants.DamageType.Magic) { KnockbackInMeters = (Is(Property.UpgradeA) && IsNot(Property.Ultimate)) ? _upgradeAKnockbackInMeters : _knockbackInMeters});
        AddCustomSound("Spawn1", "Ability/Ability_SpearsOfIce_Spawn1", 0.6f);
        AddCustomSound("Spawn2", "Ability/Ability_SpearsOfIce_Spawn2", 0.6f);
        AddCustomSound("Spawn3", "Ability/Ability_SpearsOfIce_Spawn3", 0.6f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        NameOfAnimationToAutoPlay = "SpearsOfIce" + (Is(Property.Ultimate) ? "_Ultimate" : "");
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicInjury.Current * _magicInjuryScaling / 100).ToString(), _magicInjuryScaling.ToString(), (Player.Instance.MagicStagger.Current * _magicStaggerFreezeScaling / 100).ToString(), _magicStaggerFreezeScaling.ToString(), Utils.GetFormattedFloat(_knockbackInMeters)};
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(_upgradeAKnockbackInMeters), Utils.GetFormattedFloat(_upgradeAPercentageOfEnemyStaggerBarStaggerDealt)};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(_upgradeBTechniqueSpeedIncrease), (Player.Instance.MagicInjury.Current * _upgradeBExtraMagicInjuryScaling / 100).ToString(), _upgradeBExtraMagicInjuryScaling.ToString(),};
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if (Is(Property.UpgradeB) && IsNot(Property.Ultimate))
        {
            Player.Instance.Animator.SetFloat("Technique Speed", 1 + _upgradeBTechniqueSpeedIncrease / 100);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        Player.Instance.Animator.SetFloat("Technique Speed", 1.0f);
    }

    public override void CallAbilityEvent1()
    {
        if (Is(Property.Ultimate) && Player.Instance.CurrentUltimateTechniqueStacks[typeof(Ability_SpearsOfIce)] > 0 && Player.Instance.Energy.Current >= 1 && HoldingTechniqueButton)
        {
            Vector2 randomPosition = new Vector2(Player.Instance.transform.position.x + UnityEngine.Random.Range(-1.5f, 1.5f), Player.Instance.transform.position.y + UnityEngine.Random.Range(-1.5f, 1.5f));
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_SpearsOfIce), Player.Instance.CurrentTechniqueStacks[typeof(Ability_SpearsOfIce)] - 1, true);
            Player.Instance.Energy.Current -= 1;
            Utils.CreateProjectile(new(this), "SpearsOfIce" + UnityEngine.Random.Range(1, 4), randomPosition.x, randomPosition.y);
            GameObject vfx = Utils.CreateVisualEffect(new(this), "SpearsOfIce_Flash", randomPosition.x, randomPosition.y);
            vfx.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? 90 : -90);
            PlayCustomSound("Spawn" + UnityEngine.Random.Range(1, 4));
        }
        else if (Player.Instance.CurrentTechniqueStacks[typeof(Ability_SpearsOfIce)] > 0 && Player.Instance.Energy.Current >= 5 && HoldingTechniqueButton)
        {
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_SpearsOfIce), Player.Instance.CurrentTechniqueStacks[typeof(Ability_SpearsOfIce)] - 1);
            Player.Instance.Energy.Current -= 5;
            Utils.CreateProjectile(new(this), "SpearsOfIce" + UnityEngine.Random.Range(1, 4));
            GameObject vfx = Utils.CreateVisualEffect(new(this), "SpearsOfIce_Flash");
            vfx.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? 90 : -90);
            PlayCustomSound("Spawn" + UnityEngine.Random.Range(1, 4));
        }
        else
        {
            EndThisAbility();
            return;
        }
    }

    public override void CallAbilityEvent2()
    {
        User.PlayAnimation("SpearsOfIce" + (Is(Property.Ultimate) ? "_Ultimate" : ""), 0, 0.48f);
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        base.ExtraBehaviourOnHit(damage);
        if (Is(Property.UpgradeA) && IsNot(Property.Ultimate))
        {
            damage.Stagger += damage.TargetOfDamage.StaggerBar.Maximum * _upgradeAPercentageOfEnemyStaggerBarStaggerDealt / 100;
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_Freeze(_magicStaggerFreezeScaling / 100 * User.MagicStagger.Current, new(this)));
    }
}