using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Permafrost : Technique
{
    public static float EnergyCost = 40;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    private List<AreaOfEffect> _aoes = new List<AreaOfEffect>();
    private GameObject _vfx;
    private static float _freezeStaggerScalingPerSecond = 10;
    private static float _slowAppliedPerSecond = 15;
    private static float _upgradeATenacityReductionPerSecond = 5;
    private static float _upgradeBHealPercentPerSecond = 2.5f;
    private static float _ultimateFreezeStaggerScaling = 150;
    private static float _ultimateSlowApplied = 300;

    public static bool CanBeUsedDuringOtherAbilities
    {
        get
        {
            return Player.Instance.PreparingForUltimate == false;
        }
    }

    public Ability_Permafrost(Unit ability_user) : base(ability_user)
    {
        if (Is(Property.Ultimate))
        {
            NameOfAnimationToAutoPlay = "Permafrost_Ultimate";
            DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Magic) { KnockbackInMeters = 1.5f });
        }
        else
        {
            AutoPlayAbilityAnimation = false;
            DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Magic));
        }
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        AddCustomSound("Start", "Ability/Permafrost", 0.4f);
        AddCustomSound("Start2", "Ability/Permafrost_Ultimate", 0.4f);
        AddCustomSound("Start3", "Ability/Permafrost_Ultimate2", 0.4f);
        AddCustomSound("Pushback", "Ability/Ability_WindBlast_Dash", 0.5f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicStagger.Current * _freezeStaggerScalingPerSecond / 100).ToString(), _freezeStaggerScalingPerSecond.ToString(), _slowAppliedPerSecond.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeATenacityReductionPerSecond.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _upgradeBHealPercentPerSecond.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicStagger.Current * _ultimateFreezeStaggerScaling / 100).ToString(), _ultimateFreezeStaggerScaling.ToString(), _ultimateSlowApplied.ToString() };
    }

    public override void ActionsToPerformDuringAnotherAbility()
    {
        if (Player.Instance.Energy.Current < EnergyCost || Player.Instance.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == GetType()) != null)
        {
            return;
        }
        ConsumeEnergyAndCooldownForTheAbility();
        EventManager.OneTenthSecondElapsedInGame.AddListener(ResetPotentialTargets);
        EventManager.OneTenthSecondElapsedInGame.AddListener(AddAoE);
        PlayCustomSound("Start");
        _vfx = Utils.CreateVisualEffect(new(this), "Permafrost");
        _vfx.transform.SetParent(Player.Instance.SpriteRenderers["Lower Body"].Bone);
        _vfx.transform.localPosition = new Vector2(0.5f, 0);
        Player.Instance.AddEffect(new Effect_Id("StandingOnPermafrost", new(this)), 15);
        GameController.Instance.WaitAndRunMethod(15, EndAbility);
        
    }

    public override void CallAbilityEvent1()
    {
        PlayCustomSound("Pushback");
        foreach (Unit enemy in Utils.GetSpecifiedUnits(new System.Func<Unit, bool>(unit => unit.IsHostile && Vector2.Distance(User.transform.position, unit.transform.position) < 5)))
        {
            enemy.PushInTargetDirection(-20 * (User.transform.position - enemy.transform.position).normalized, this);
        }
    }

    public override void CallAbilityEvent2()
    {
        Utils.CreateAreaOfEffect(new(this), "Permafrost_Ultimate", User.transform.position.x, User.transform.position.y - 1f);
    }

    public void AddAoE()
    {
        if (Is(Property.UpgradeB))
        {
            Player.Instance.Health.Current += Player.Instance.Health.Missing * _upgradeBHealPercentPerSecond / 100 / 10;
        }
        float lowestDistance = 999;
        foreach (AreaOfEffect aoe in _aoes.Where(a => a != null && a.IsDestroyed() == false))
        {
            float distance = Vector2.Distance(aoe.transform.position, new Vector2(User.transform.position.x, User.transform.position.y - 0.5f));
            lowestDistance = distance < lowestDistance ? distance : lowestDistance;
        }
        if (lowestDistance > 0.3f)
        {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "PermafrostAoE", User.transform.position.x, User.transform.position.y - 0.5f);
            _aoes.Add(aoe);
        }
    }

    public void EndAbility()
    {
        EventManager.OneTenthSecondElapsedInGame.RemoveListener(ResetPotentialTargets);
        if (IsNot(Property.Ultimate))
        {
            EventManager.OneTenthSecondElapsedInGame.RemoveListener(AddAoE);
            foreach (AreaOfEffect aoe in _aoes.ToList())
            {
                MonoBehaviour.Destroy(aoe);
            }
        }
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        base.ExtraBehaviourOnHit(damage);
        if (IsNot(Property.Ultimate))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Freeze(_freezeStaggerScalingPerSecond / 100 / 10 * User.MagicStagger.Current, new(this)));
            damage.TargetOfDamage.AddEffect(new Effect_Slow(_slowAppliedPerSecond / 10, new(this)));
            Player.Instance.AddEffect(new Effect_Id("StandingOnPermafrost", new(this)), 0.2f);
            if (Is(Property.UpgradeA))
            {
                Effect tenacityDebuff = damage.TargetOfDamage.GetEffect(new System.Func<Effect, bool>(effect => effect.Id == "PermafrostTenacityDebuff"));
                if (tenacityDebuff != null)
                {
                    tenacityDebuff.FlatAmount -= _upgradeATenacityReductionPerSecond / 10;
                    tenacityDebuff.RemainingDuration = 15;
                    tenacityDebuff.UICooldownDisplay.fillAmount = 1;
                    tenacityDebuff.UIText = Utils.GetFormattedFloat(-tenacityDebuff.FlatAmount);
                }
                else
                {
                    damage.TargetOfDamage.AddEffect(new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(this)) { FlatAmount = -_upgradeATenacityReductionPerSecond / 10, Id = "PermafrostTenacityDebuff", ShowsInUI = true, PathToUIGraphic = "UI/Control", UIText = Utils.GetFormattedFloat(_upgradeATenacityReductionPerSecond / 10) }, 15);
                }
            }
        }
        else if (Is(Property.Ultimate) && AffectedEnemies.ContainsKey(damage.TargetOfDamage) == false)
        {
            damage.TargetOfDamage.AddEffect(new Effect_Freeze(_ultimateFreezeStaggerScaling / 100 / User.MagicStagger.Current, new(this)));
            damage.TargetOfDamage.AddEffect(new Effect_Slow(_ultimateSlowApplied, new(this)));
        }
    }
}