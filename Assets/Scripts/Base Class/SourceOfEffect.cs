using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceOfEffect
{
    public Unit SourceUnit;
    public Item SourceItem;
    public Ability SourceAbility;
    public string SourcePassivePowerUp;
    public SourceOfEffect(string passive_power_up)
    {
        SourcePassivePowerUp = passive_power_up;
    }
    public SourceOfEffect(Unit unit)
    {
        SourceUnit = unit;
    }
    public SourceOfEffect(Item item)
    {
        SourceItem = item;
    }
    public SourceOfEffect(Ability ability)
    {
        SourceAbility = ability;
    }

    public Unit Target
    {
        get
        {
            if (SourcePassivePowerUp != null)
            {
                return Player.Instance;
            }
            else if (SourceUnit != null)
            {
                return SourceUnit;
            }
            else if (SourceItem != null)
            {
                return Player.Instance;
            }
            else if (SourceAbility != null)
            {
                return SourceAbility.Target != null ? SourceAbility.Target : SourceAbility.User.CurrentTarget != null ? SourceAbility.User.CurrentTarget : SourceAbility.User.GetClosestValidTarget();
            }
            else
            {
                return null;
            }
        }
    }

    public Unit User
    {
        get
        {
            if (SourcePassivePowerUp != null)
            {
                return Player.Instance;
            }
            else if (SourceUnit != null)
            {
                return SourceUnit;
            }
            else if (SourceItem != null)
            {
                return Player.Instance;
            }
            else if (SourceAbility != null)
            {
                return SourceAbility.User;
            }
            else
            {
                return null;
            }
        }
    }

    public string Source
    {
        get
        {
            if (SourcePassivePowerUp != null)
            {
                return SourcePassivePowerUp;
            }
            else if (SourceUnit != null)
            {
                return SourceUnit.gameObject.name.Replace("(Clone)", "");
            }
            else if (SourceItem != null)
            {
                return SourceItem.GetType() + " - " + SourceItem.Grade;
            }
            else if (SourceAbility != null)
            {
                return SourceAbility.GetType().ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
