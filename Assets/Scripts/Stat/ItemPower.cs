using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPower : Stat {

    public ItemPower(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }
}