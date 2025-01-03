using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_EnergyBarrage : Technique
{
    public static float EnergyCost = 5;
    public static float Cooldown = 12;

    private bool isRightHand = true;
    private int counter = 0;
    private int[] directions = new int[] { - 90, -60, -75, -105, -120};
    private Dictionary<Unit, List<int>> enemies_hit = new Dictionary<Unit, List<int>>();

    public static AbilityFamily Family = AbilityFamily.Proprius;
    public static Constants.DamageType TechniqueDamageCategory = Constants.DamageType.Magic;
    public static bool IsVariableEnergyTechnique = true;

    public static float HealthScaling = 70;
    public static float StaggerScaling = 70;
    public static float MasteryAAnalyzedApplied = 10;

    public Ability_EnergyBarrage(Unit ability_user) : base(ability_user)
    {
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        EffectsAffectingUserDuringAbility = new List<Effect> {new Effect_Unstoppable(new(this)), new Effect_Immovable(new(this))};
        AddCustomSound("Spawn", "Ability/Ability_EnergyBarrageSpawn", 0.5f);
        CustomHitSound = "Ability/Ability_EnergyBarrageHit";
        HitSoundVolume = 0.5f;
    }

    public override void CallAbilityEvent1() {
        counter++;
        Player.Instance.Actions.ConsumeEnergyAndCooldownForTheAbility();
        for(int i = 0; i < (UpgradeBUnlocked ? 5 : 1); i++) {
            Projectile proj = Utils.CreateProjectile(new(this), "EnergyBarrage", Player.Instance.SpriteRenderers[isRightHand ? "Right Hand" : "Left Hand"].Bone.transform.position.x, Player.Instance.SpriteRenderers[isRightHand ? "Right Hand" : "Left Hand"].Bone.transform.position.y);
            proj.transform.rotation = Quaternion.Euler(0, Player.Instance.Actions.IsFlipped ? 180 : 0, directions[i]);
            proj.gameObject.name = "EnergyBarrage_" + counter.ToString();
            proj.CleanUpAfter(6);
        }
        isRightHand = false;
        if(UpgradeAUnlocked && Player.Instance.Animator.GetFloat("Technique Speed") < 2) {
            Player.Instance.Animator.SetFloat("Technique Speed", Player.Instance.Animator.GetFloat("Technique Speed") + 0.1f);
        }
        Player.Instance.Actions.PlayAbilityCustomSound("Spawn");
        if(!(HoldingMainButton || HoldingTechniqueButton) || Player.Instance.Energy.Current < EnergyCost) {
            Player.Instance.PlayAnimation("EnergyBarrage", 0.1f, 0.58f);
        }
    }

    public override void CallAbilityEvent2() {
        if((HoldingMainButton || HoldingTechniqueButton) && Player.Instance.Energy.Current >= EnergyCost) {
            isRightHand = true;
            Player.Instance.PlayAnimation("EnergyBarrage", 0, 0.1f);
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(UpgradeBUnlocked) {
            int number = int.Parse(object_hitting.gameObject.name.Split("_")[1]);
            if(enemies_hit.ContainsKey(unit_getting_attacked) == false) {
                enemies_hit.Add(unit_getting_attacked, new List<int> {number});
            }
            DamageSources = new List<DamageSource> {new DamageSource(enemies_hit[unit_getting_attacked].Contains(number) ? HealthScaling / 4 : HealthScaling, enemies_hit[unit_getting_attacked].Contains(number) ? StaggerScaling / 4 : StaggerScaling, Constants.DamageType.Magic) {KnockbackIntoRange = 0.1f}};
            base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
            if(enemies_hit[unit_getting_attacked].Contains(number) == false) {
                enemies_hit[unit_getting_attacked].Add(number);
            }
        }
        else {
            DamageSources = new List<DamageSource> {new DamageSource( HealthScaling, StaggerScaling, Constants.DamageType.Magic)};
            base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        }
    }

    public override void ExtraBehaviourOnHit(Damage damage) {
        if(UpgradeAUnlocked) {
            damage.TargetOfDamage.AddEffect(new Effect_Analysis(MasteryAAnalyzedApplied, new(this)) {PlaySoundEffect = false});
        }
    }


    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicInjury.Current * StaggerScaling / 100).ToString(), HealthScaling.ToString(), (Player.Instance.MagicStagger.Current * StaggerScaling / 100).ToString(), StaggerScaling.ToString(), EnergyCost.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { "10", MasteryAAnalyzedApplied.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { };
    }

    public static string GetAbilitySpecificEnergyCostText() {
        return GetEnergyCost(typeof(Ability_EnergyBarrage)) + "+";
    }

    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        if(UpgradeAUnlocked) {
            Player.Instance.Animator.SetFloat("Technique Speed", 1);
        }
    }
}