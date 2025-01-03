using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Barrier : Effect
{
    private RectTransform _playerHealthBar;
    private RectTransform _playerBarrierBar;
    public Effect_Barrier(float barrier_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        _initialDecayingAmount = barrier_amount;
        DisplayEffectIndicator = true;
        PathToEffectGraphic = "Effect/Barrier";
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
        DescriptionLabel = "Effect_Analysis_Explanation";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        if(TargetOfEffect is Player && _playerHealthBar == null) {
            _playerHealthBar = Player.Instance.Health.HUDSlider.GetComponent<RectTransform>();
            _playerBarrierBar = Player.Instance.Health.HUDSlider.transform.Find("Barrier").GetComponent<RectTransform>();
        }
        if(DecayingAmount <= 0) {
            EndThisEffect();
        }
        else if(TargetOfEffect is Player) {
            float widthHealth = DecayingAmount > Player.Instance.Health.Maximum ? _playerHealthBar.sizeDelta.x : _playerHealthBar.sizeDelta.x * (DecayingAmount / Player.Instance.Health.Maximum);
            float widthStaggerBar = DecayingAmount > Player.Instance.StaggerBar.Maximum ? _playerHealthBar.sizeDelta.x : _playerHealthBar.sizeDelta.x * (DecayingAmount / Player.Instance.StaggerBar.Maximum);
            _playerBarrierBar.transform.localPosition = new Vector2(-5, 0);
            _playerBarrierBar.sizeDelta = new Vector2(20 + (widthHealth + widthStaggerBar) / 4, 60);
        }
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        base.OnInvokeAfterHitDamageCalculation(damage);
        if(damage.Injury + damage.Stagger > DecayingAmount) {
            float amount = DecayingAmount;
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
            AddDecayingAmount(-damage.Injury - damage.Stagger);
            damage.Injury = 0;
            damage.Stagger = 0;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if(TargetOfEffect is Player) {
            Player.Instance.Health.HUDSlider.transform.Find("Barrier").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);
        }
    }
}
