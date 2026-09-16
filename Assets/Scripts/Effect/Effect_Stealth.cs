using System.Linq;
using UnityEngine;

public class Effect_Stealth : Effect {

    public Effect_Stealth(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        ShowsInUI = true;
        Listeners.Add(EventManager.HitDealt);
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }

    public override void OnInvokeHitDealt(DamageInstance damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect) {
            Effect prolongStealth = Player.Instance.GetEffectWithGivenId("BasicAttacksAndTechniquesDontEndStealthImmediately");
            if (TargetOfEffect == Player.Instance && prolongStealth != null && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown(prolongStealth.Id))
            {
                RemainingDuration = prolongStealth.FlatAmount;
                Player.Instance.AddCooldown(typeof(Effect), prolongStealth.FlatAmount, prolongStealth.Id);
            }
            else
            {
                EndThisEffect();
            }
            base.OnInvokeHitDealt(damage);
        }
    }
}