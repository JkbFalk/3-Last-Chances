using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Effect_Barrier : Effect
{
    private Slider _barrierBar;
    public Slider BarrierBar
    {
        get
        {
            if (_barrierBar == null)
            {
                _barrierBar = TargetOfEffect.Health.HUDSlider.transform.Find("Barrier").GetComponent<Slider>();
            }
            return _barrierBar;
        }
    }
    public Effect_Barrier(float barrier_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        _initialAmount = barrier_amount;
        ShowsInUI = true;
        PathToUIGraphic = "Effect/Barrier";
        DefaultDecaySpeed = 0.05f;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        if (TargetOfEffect is Player && Player.Instance.CurrentStance.StanceEffect is Stance_BodyOfSteel && SaveFile.Instance.ActiveUpgrades.Contains("Stance_BodyOfSteel3") && amount_decayed > 0)
        {
            Player.Instance.AddEffect(new Effect_Empowered(-amount_changed * Stance_BodyOfSteel.Upgrade3PercentageOfDecayedBarrierConvertedIntoEmpowered / 100, new(Player.Instance.CurrentStance.StanceEffect)));
        }
        else if (TargetOfEffect is Player && Player.Instance.CurrentStance.StanceEffect is Stance_BodyOfSteel && SaveFile.Instance.ActiveUpgrades.Contains("Stance_BodyOfSteel3") && amount_changed < 0)
        {
            Player.Instance.Health.Current += amount_decayed * Stance_BodyOfSteel.Upgrade3PercentageOfUsedBarrierRestoringHealthAndStaggerBar / 100;
            Player.Instance.StaggerBar.Current += amount_decayed * Stance_BodyOfSteel.Upgrade3PercentageOfUsedBarrierRestoringHealthAndStaggerBar / 100;
        }
        float healthAndStaggerBarTotal = TargetOfEffect.Health.Maximum + TargetOfEffect.StaggerBar.Maximum;
        StackingEffectIntensityLevel = Amount < healthAndStaggerBarTotal * 0.1f ? 1 : Amount < healthAndStaggerBarTotal * 0.25f ? 2 : 3;
        BarrierBar.value = Amount / TargetOfEffect.Health.Maximum;
        UIText = Utils.GetFormattedFloat(Amount, 0);
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        base.OnInvokeAfterHitDamageCalculation(damage);
        if(damage.Injury + damage.Stagger > Amount) {
            float amount = Amount;
            if(damage.Injury > amount) {
                damage.Injury -= amount;
            }
            else {
                amount -= damage.Injury;
                damage.Injury = 0;
                damage.Stagger -= amount;
            }
            Utils.PlaySoundEffect(TargetOfEffect.AudioSource, "Effect/Effect_Barrier_End" + UnityEngine.Random.Range(1, 4), 0.9f);
            EndThisEffect();
        }
        else {
            Utils.PlaySoundEffect(TargetOfEffect.AudioSource, "Effect/Effect_Barrier_Hit" + UnityEngine.Random.Range(1, 7), 0.6f);
            ChangeAmount(-damage.Injury - damage.Stagger);
            damage.Injury = 0;
            damage.Stagger = 0;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        if (TargetOfEffect is Player)
        {
            Player.Instance.Health.HUDSlider.transform.Find("Barrier").gameObject.SetActive(true);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if (TargetOfEffect is Player)
        {
            Player.Instance.Health.HUDSlider.transform.Find("Barrier").gameObject.SetActive(false);
        }
    }
}
