using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientCrown : Effect { 

    public Effect_ChangeStat CDRBuff;
    public float BuffAmount = 0;

    public Effect_AncientCrown(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.ItemEquipped);
    }

    public override void OnEffectValueChanged()
    {
        BuffAmount = 0.125f * NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BuffAmount) };
    }

    public override void OnStart() {
        base.OnStart();
        if(SaveFile.Instance.EquippedGloves is Gloves_Ancient && SaveFile.Instance.EquippedHelmet is Helmet_Ancient) {
            AddBuffs();
        }
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2)
    {
        if (CDRBuff != null && (item1 is Gloves_Ancient || item1 is Helmet_Ancient))
        {
            CDRBuff.EndThisEffect();
        }
        else if ((item2 is Helmet_Ancient && SaveFile.Instance.EquippedGloves is Gloves_Ancient) || (item2 is Gloves_Ancient && SaveFile.Instance.EquippedHelmet is Helmet_Ancient))
        {
            AddBuffs();
        }
    }

    public void AddBuffs() {
        CDRBuff = new Effect_ChangeStat(Player.Instance.CooldownReduction, SourceOfEffect) {PercentageAmount = BuffAmount};
        Player.Instance.AddEffect(CDRBuff);
    }
}
