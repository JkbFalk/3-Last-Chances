using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Ancient : Item
{
    public Boots_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> { new ItemEffect("AncientBoots", 0.2f), new ItemEffect("AttackSpeed", 0.8f) };
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("MovementSpeed")};
    }
}
