using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientBreastplate : Effect { 

    public Effect_ChangeCompositeStat InjuryBuff;
    public Effect_ChangeCompositeStat StaggerBuff;
    public float BuffAmount = 0;

    public Effect_AncientBreastplate(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.ItemEquipped);
    }

    public override void OnEffectValueChanged()
    {
        BuffAmount = 0.125f * LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BuffAmount) };
    }

    public override void OnStart() {
        base.OnStart();
        if(SaveFile.Instance.EquippedHelmet is Helmet_Ancient && SaveFile.Instance.EquippedArmor is Armor_Ancient) {
            AddBuffs();
        }
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2)
    {
        if (InjuryBuff != null && (item1 is Helmet_Ancient || item1 is Armor_Ancient))
        {
            InjuryBuff.EndThisEffect();
            StaggerBuff.EndThisEffect();
        }
        else if ((item2 is Armor_Ancient && SaveFile.Instance.EquippedHelmet is Helmet_Ancient) || (item2 is Helmet_Ancient && SaveFile.Instance.EquippedArmor is Armor_Ancient))
        {
            AddBuffs();
        }
    }

    public void AddBuffs() {
        InjuryBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, SourceOfEffect) {PercentageAmount = BuffAmount};
        StaggerBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, SourceOfEffect) {PercentageAmount = BuffAmount};
        Player.Instance.AddEffect(InjuryBuff);
        Player.Instance.AddEffect(StaggerBuff);
    }
}
