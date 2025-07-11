using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;

public class Stance_HeatOfBattle : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Ignis;

    private int _counter = 0;
    private Image _cooldownDisplay;
    private Image _healIndicator;
    private TextMeshProUGUI _amountDisplay;
    private GameObject _burnVfx;

    public static float PercentageHealthHealed = 5;
    public static float StaggerBurnScaling = 10;
    public static float MaximumDamageIncreasedAgainstBurn = 50;
    public static float HealthRestoredPerBurn = 5;

    public Stance_HeatOfBattle(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.EffectStarted, EventManager.UnitStatCurrentAmountChanged};
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(IsActive && UnlockedUpgrade2 && damage?.SourceOfDamage?.User != null && damage.SourceOfDamage.User is Player && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Burn)) && damage.TargetOfDamage.IsHostile) {
            base.OnInvokeHitDealt(damage);
            float increaseAmount = damage.TargetOfDamage.GetEffect(typeof(Effect_Burn)).DecayingAmount * 0.5f;
            damage.InjuryDealtPercentageModifier += increaseAmount;
            damage.StaggerDealtPercentageModifier += increaseAmount;
        }
    }

    public override void OnStanceActivated() {
        if(UnlockedUpgrade1) {
            _burnVfx = Utils.CreateVisualEffect(SourceOfEffect, "HeatOfBattle");
            _burnVfx.transform.SetParent(Player.Instance.transform);
            _burnVfx.transform.localPosition = new Vector2(0, -0.5f);
        }
    }

    public override void OnStanceDeactivated()
    {
        if(UnlockedUpgrade1 && _burnVfx != null && !_burnVfx.IsDestroyed()) {
            _burnVfx.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
    }

    public override void OnFixedUpdate()
    {
        if(IsActive && _counter == 10) {
            _counter = 0;
            if(Utils.GetAllUnits(true).FirstOrDefault(enemy => enemy.CheckIfUnderEffect(typeof(Effect_Burn)) && Vector2.Distance(enemy.transform.position, Player.Instance.transform.position) <= 3) != null) {
                _healIndicator.gameObject.SetActive(false);
                Player.Instance.Health.Current += Player.Instance.Health.Maximum * PercentageHealthHealed / 5 / 100;
            }
            else {
                _healIndicator.gameObject.SetActive(true);
            }
            if(UnlockedUpgrade1) {
                foreach(Unit enemy in Utils.GetAllUnits(true).Where(enemy => Vector2.Distance(enemy.transform.position, Player.Instance.transform.position) <= 3)) {
                    float burnAmount = Player.Instance.HeavyStagger.Current > Player.Instance.MagicStagger.Current ? Player.Instance.HeavyStagger.Current * StaggerBurnScaling / 100 : Player.Instance.MagicStagger.Current * StaggerBurnScaling / 100;
                    enemy.AddEffect(new Effect_Burn(burnAmount / 50, SourceOfEffect));
                }
            }
            if(UnlockedUpgrade3) {
                float BurnTotal = 0;
                foreach(Unit enemy in Utils.GetAllUnits(true).Where(enemy => enemy.CheckIfUnderEffect(typeof(Effect_Burn)))) {
                    BurnTotal += enemy.GetEffect(typeof(Effect_Burn)).DecayingAmount;
                }
                _amountDisplay.text = Utils.GetFormattedFloat(BurnTotal * HealthRestoredPerBurn);
            }
            else {
                _amountDisplay.text = "";
            }
        }
        else if(IsActive){
            _counter++;
        }
    }

    public override void CreateStanceDisplay() {
        base.CreateStanceDisplay();
        _healIndicator = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/HealIndicator").GetComponent<Image>();
        _cooldownDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Cooldown").GetComponent<Image>();
        _amountDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Amount").GetComponent<TextMeshProUGUI>();
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount) {
        if(IsActive && UnlockedUpgrade3 && stat is Health && stat.Owner is Player && Player.Instance.Health.Current <= 0 && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Type == typeof(Stance_HeatOfBattle)) == null) {
            float BurnTotal = 0;
            foreach(Unit enemy in Utils.GetAllUnits(true).Where(enemy => enemy.CheckIfUnderEffect(typeof(Effect_Burn)))) {
                BurnTotal += enemy.GetEffect(typeof(Effect_Burn)).DecayingAmount;
                enemy.GetEffect(typeof(Effect_Burn)).EndThisEffect();
            }
            Player.Instance.Health.Current = BurnTotal * HealthRestoredPerBurn;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Fire/Fire13", 2f);
            Utils.CreateVisualEffect(SourceOfEffect, "HeatOfBattleSave", Player.Instance.SpriteRenderers["Upper Body"].Bone.position.x, Player.Instance.SpriteRenderers["Upper Body"].Bone.position.y);
            _amountDisplay.text = "0";
            Player.Instance.AddCooldown(new Cooldown(typeof(Stance_HeatOfBattle), 60, Player.Instance) {CooldownDisplay = _cooldownDisplay});
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> {PercentageHealthHealed.ToString()};
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> {(Player.Instance.HeavyStagger.Current > Player.Instance.MagicStagger.Current ? Player.Instance.HeavyStagger.Current * StaggerBurnScaling / 100 : Player.Instance.MagicStagger.Current * StaggerBurnScaling / 100).ToString(), StaggerBurnScaling.ToString()};
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> {MaximumDamageIncreasedAgainstBurn.ToString()};
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> {"100", "20"};
    }
}
