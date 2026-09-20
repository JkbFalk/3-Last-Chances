// FILE: Assets\Scripts\Stat\UltimateEnergy.cs
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stat_UltimateEnergy : Stat
{
    public const float MAX_ULTIMATE_ENERGY = 30f;
    public const float ENERGY_SPENT_TO_ULTIMATE_RATIO = 1f;
    public const float STAGGER_GAIN_AMOUNT = 15f;

    private Image _ultimateBarImage;
    private Material _ultimateBarMaterial;
    private bool _wasFull = false;

    public bool IsFull => Current >= Maximum;

    public Stat_UltimateEnergy(Unit stat_owner, float base_amount = MAX_ULTIMATE_ENERGY) 
        : base(stat_owner, MAX_ULTIMATE_ENERGY)
    {
        Owner = stat_owner;
        Base = MAX_ULTIMATE_ENERGY;
        Maximum = MAX_ULTIMATE_ENERGY;
        CurrentCanBeLowerThanMaximum = true;
        Current = 0f;

        InitUI();

        EventManager.AbilityEnergyConsumed.AddListener(OnEnergyConsumed);
        EventManager.EffectStarted.AddListener(OnEffectStarted);
        EventManager.ExitCombat.AddListener(OnExitCombat);
    }

    private void InitUI()
    {
        if (UIManager.Objects.ResourceBars != null)
        {
            Transform ultimateBarTransform = UIManager.Objects.ResourceBars.transform.Find("Energy/UltimateBar");
            if (ultimateBarTransform != null)
            {
                _ultimateBarImage = ultimateBarTransform.GetComponent<Image>();
                
                // Create an instance of the material so we don't modify the project asset
                if (_ultimateBarImage != null && _ultimateBarMaterial == null)
                {
                    _ultimateBarMaterial = new Material(_ultimateBarImage.material);
                    _ultimateBarImage.material = _ultimateBarMaterial;
                }
            }
        }
    }

    public void AddCharge(float amount)
    {
        bool hasUnlockedFamilies = SaveFile.Instance != null && 
                                   SaveFile.Instance.UnlockedUltimateFamilies != null && 
                                   SaveFile.Instance.UnlockedUltimateFamilies.Count > 0;
        if (!hasUnlockedFamilies) 
        {
            return;
        }
        // Spam mode keeps the ultimate gauge permanently full
        if (DebugController.SpamModeEnabled)
        {
            Current = Maximum;
            return;
        }
        if (Owner != null && Owner.InCombat)
        {
            Current += amount;
        }
    }

    public void ConsumeCharge()
    {
        // Spam mode instantly refills the gauge, even right after a charge is spent
        if (DebugController.SpamModeEnabled)
        {
            Current = Maximum;
            _wasFull = true;
            return;
        }
        Current = 0f;
        _wasFull = false;
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged()
    {
        base.AdditionalStatSpecificActionsAfterCurrentValueChanged();

        if (_ultimateBarImage == null)
        {
            InitUI();
        }

        // Update the custom shader properties
        if (_ultimateBarMaterial != null)
        {
            _ultimateBarMaterial.SetFloat("_FillAmount", Current / Maximum);
            _ultimateBarMaterial.SetFloat("_FadeWidth", IsFull ? 0f : 0.15f);
        }

        // Sound cue when full
        if (!_wasFull && IsFull)
        {
            _wasFull = true;
            Utils.PlaySoundEffect(Owner?.AudioSource, "Generic/PowerUp1", 0.9f);
        }
        else if (!IsFull)
        {
            _wasFull = false;
        }
    }

    private void OnEnergyConsumed(Ability ability, float amount, bool wasFullEnergy)
    {
        // Prevent Ultimates from charging themselves
        if (ability != null && ability.Is(Ability.Property.Ultimate)) return;

        if (amount > 0f)
        {
            AddCharge(amount * ENERGY_SPENT_TO_ULTIMATE_RATIO);
        }
    }

    private void OnEffectStarted(Effect effect)
    {
        // 15 charge awarded whenever a hostile enemy is Staggered
        if (effect is Effect_Staggered && effect.TargetOfEffect != null && effect.TargetOfEffect.IsHostile)
        {
            if (effect.SourceOfEffect?.User == Owner)
            {
                AddCharge(STAGGER_GAIN_AMOUNT);
            }
        }
    }

    private void OnExitCombat(Unit unit)
    {
        if (unit == Owner)
        {
            ConsumeCharge();
        }
    }
}