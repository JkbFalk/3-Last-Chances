using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Longblade_BladeOfJudgment : Item
{
    public Longblade_BladeOfJudgment(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(90, 120, 0.9f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
        new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, GetFirstModifierEffectValue(false) * 1.25f, new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Sharp)},EffectTypeName="ExtraEffectiveButConsumableSharp", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 1.25f, 0)}},
        new Effect_CustomizableEffectOnEvent(new(this)) {EffectTypeName="NoDescription", ConditionCheckForAbilityEnded = new Func<Ability, bool>((ability) => 
            ability.User is Player && (ability.IsRiposte || ability.IsCounter) && ability.User.CheckIfUnderEffect(typeof(Effect_Sharp))), 
            ActionOnAbilityEnded = new Action<Ability, Effect_CustomizableEffectOnEvent> ((Ability, effect) =>  {
                Effect sharp = Player.Instance.GetEffect(typeof(Effect_Sharp));
                sharp.EndThisEffect();
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableEffectOnEvent(new(this)) { UsesTheFollowingEffects=new() {typeof(Effect_Sharp)}, DescriptionParameters = new List<String> {"5", Utils.GetFormattedFloat(0.2f * GetSecondModifierEffectValue(), 1), Utils.GetFormattedFloat(0.2f * GetSecondModifierEffectValue()+ 0.2f * GetSecondModifierEffectValue() * GetFirstModifierEffectValue(false) * 1.25f / 100, 1)}, EffectTypeName="ReceiveSharpWhileInProximity", FlatAmount = 0.2f * GetSecondModifierEffectValue(), ConditionCheckForOneFifthSecondElapsedNotRealtime = new Func<bool>(() => 
            Utils.GetAllUnits(true, true).FirstOrDefault(enemy => Vector2.Distance(enemy.transform.position, Player.Instance.transform.position) < 5) != null), ActionOnOneFifthSecondElapsedNotRealtime = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                Player.Instance.AddEffect(new Effect_Sharp(effect.FlatAmount / 5, new(this)));
        })}};
    }
}
