using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientFirearmsDamage : Effect { 

    public Effect_ChangeCompositeStat DamageBuff;
    public float DamageBuffAmount = 2;

    public Effect_AncientFirearmsDamage(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.ItemEquipped);
    }

    public override void OnEffectValueChanged()
    {
        DamageBuffAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DamageBuffAmount) };
    }

    public override void OnStart() {
        base.OnStart();
        Activate();
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2) {
        Debug.Log("EQ " + item1 + " , " + item2);
        base.OnInvokeItemEquipped(item1, item2);
        Activate();
    }

    public override void OnEnd() {
        base.OnEnd();
        Activate();
    }

    public void Activate() {
        if(DamageBuff != null && DamageBuff.EffectEnded == false) {
            DamageBuff.EndThisEffect();
        }
        int ancientCount = 1;
        if(SaveFile.Instance.EquippedHelmet is Helmet_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedArmor is Armor_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedGloves is Gloves_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedBoots is Boots_Ancient) {
            ancientCount++;
        }
        Debug.Log("ACTIVATED " + ancientCount);
        DamageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, SourceOfEffect) {PercentageAmount = ancientCount * DamageBuffAmount};
        Player.Instance.AddEffect(DamageBuff);
    }
}
