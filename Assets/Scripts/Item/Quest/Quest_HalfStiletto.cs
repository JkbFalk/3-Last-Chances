using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_HalfStiletto : Item
{
    public Quest_HalfStiletto(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Quest;
        Grade = ItemGrade.Excellent;
    }
}
