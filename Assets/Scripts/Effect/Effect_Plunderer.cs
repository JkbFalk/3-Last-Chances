using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Effect_Plunderer : Effect
{
    public float Amount;
    public Effect_Plunderer(float amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Amount = amount;
        Listeners.Add(EventManager.AbilityUsed);
    }

    public override void OnInvokeAbilityUsed(Ability ability) {
        FieldInfo family = ability.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static);
        if(ability.User != Player.Instance && ability.User.IsHostile && Vector2.Distance(Player.Instance.transform.position, ability.User.transform.position) < 10 && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Type == typeof(Effect_Plunderer)) == null && family != null && !Player.Instance.CheckIfUnderEffect(typeof(Effect_PlundererAbilityAmplify))) {
            Player.Instance.AddEffect(new Effect_PlundererAbilityAmplify(Amount, SourceOfEffect) {AmplifiedFamily = (Ability.AbilityFamily)family.GetValue(null)}, 15);
            Player.Instance.AddCooldown(new Cooldown(typeof(Effect_Plunderer), 30, Player.Instance));
        }
    }
}
