using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Stance_SingularPursuit : Effect_Stance
{
    private Image _fillGauge;
    private TextMeshProUGUI _amountDisplay;

    public float MaxSpiritBond;
    private float _spiritBond = 0;
    public float SpiritBond {
        get => _spiritBond;
        set {
            _spiritBond = value < 0 ? 0 : value > MaxSpiritBond ? MaxSpiritBond : value;
            if(Player.Instance.CurrentStance.StanceEffect == this && _fillGauge != null && _amountDisplay != null) {
                _fillGauge.fillAmount = _spiritBond / MaxSpiritBond;
                _amountDisplay.text = ((int)_spiritBond).ToString() + "%";
            }
        }
    }

    public Stance_SingularPursuit(SourceOfEffect source_of_effect) : base(source_of_effect) {
        MaxSpiritBond = 30 + (UnlockedUpgrade1 ? 20 : 0) + (UnlockedUpgrade2 ? 20 : 0) + (UnlockedUpgrade3 ? 30 : 0);
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.DamageDealt, EventManager.UnitKnockedOut, EventManager.HealthBarBroken};
    }

    public override void OnInvokeUnitKnockedOut(Damage damage)
    {
        base.OnInvokeUnitKnockedOut(damage);
        Activate(damage);
    }

    public override void OnInvokeHealthBarBroken(Damage damage)
    {
        base.OnInvokeHealthBarBroken(damage);
        Activate(damage);
    }

    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Anima;

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User == TargetOfEffect && IsActive && SpiritBond > 0) {
            damage.Injury += damage.Injury * SpiritBond / 100;
            damage.Stagger += damage.Stagger * SpiritBond / 100;
            base.OnInvokeHitDealt(damage);
        }
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if(IsActive && UnlockedUpgrade1 && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)) {
            SpiritBond += 1 / Player.Instance.CurrentWeaponAttackSpeed.Current;
        }
        else if(IsActive && UnlockedUpgrade2 && damage.SourceOfDamage.Is(Ability.Property.Technique)) {
            SpiritBond += 5;
        }
        else if(IsActive && UnlockedUpgrade3 && damage.SourceOfDamage.Is(Ability.Property.Riposte)) {
            SpiritBond += 3;
        }
        else if(IsActive && UnlockedUpgrade3 && damage.SourceOfDamage.Is(Ability.Property.Counter)) {
            SpiritBond += 5;
        }
        base.OnInvokeDamageDealt(damage);
    }

    public void Activate(Damage damage)
    {
        if(IsActive) {
            SpiritBond += 10;
        }
    }

    public override void OnFixedUpdate()
    {
        if(SpiritBond > 0) {
            SpiritBond -= 0.5f / 50;
        }
        else {
            SpiritBond = 0;
        }
    }

    public override void CreateStanceDisplay() {
        base.CreateStanceDisplay();
        _fillGauge = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Fill").GetComponent<Image>();
        _amountDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Amount").GetComponent<TextMeshProUGUI>();
        SpiritBond = SpiritBond;
    }
}
