using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Tool_IceCoating : Item
{
    public bool EnemyWasHit = false;
    private bool _soundPlayed = false;

    public static float Cooldown = 10;
    public Tool_IceCoating(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Tool;
        OnUseAbility = typeof(Ability_ThrowItem);
        DamageSources = new List<Ability.DamageSource>()
        {
            new Ability.DamageSource(1 * GetMultiplierForGrade(), 0, Constants.DamageType.None, "VacuumGrenade"),
            new Ability.DamageSource(3 * GetMultiplierForGrade(), 10 * GetMultiplierForGrade(), Constants.DamageType.None, "AoE")
        };
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description" + (detailed ? "Detailed" : "")), new object[] { Utils.GetFormattedFloat(3 * GetMultiplierForGrade()), Utils.GetFormattedFloat(10 * GetMultiplierForGrade()), Utils.GetFormattedFloat(20 * GetMultiplierForGrade()), 5, Utils.GetFormattedFloat(1 * GetMultiplierForGrade()) }) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n[CD] " + Cooldown.ToString();
    }

    public override void OnUse()
    {
        base.OnUse();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemType.Tool, "Tool_VacuumGrenade_" + Grade);
        _soundPlayed = false;
        EnemyWasHit = false;
        GameController.Instance.WaitAndRunMethod(1.5f, ExplodeIfNotHittingEnemy);
    }

    public override void OnEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        EnemyWasHit = true;
        if (object_hitting.name.Contains("VacuumGrenade"))
        {
            Activate();
        }
    }

    public void Activate() {
        foreach (Transform child in ToolObject.transform)
        {
            child.gameObject.SetActive(true);
        }
        Transform on_hit_vfx = ToolObject.transform.Find("OnHit");
        if(on_hit_vfx != null && on_hit_vfx.gameObject!= null) {
            GameObject cloned_on_hit_vfx = MonoBehaviour.Instantiate(on_hit_vfx.gameObject);
            cloned_on_hit_vfx.gameObject.SetActive(true);
            cloned_on_hit_vfx.transform.position = ToolObject.transform.position;
            foreach (DamagingObject obj in cloned_on_hit_vfx.GetComponentsInChildren<DamagingObject>(true))
            {
                obj.SourceAbility = ItemUseAbility;
            }
        }  
        MonoBehaviour.Destroy(ToolObject.gameObject);
        GameController.Instance.WaitAndRunMethod(0.5f, DamageSecondPhase);
        PlaySound();
    }

    public void DamageSecondPhase()
    {
        if (ItemUseAbility.AffectedEnemies.Count == 1)
        {
            new DamageInstance(ItemUseAbility.AffectedEnemies.First().Key, ItemUseAbility, null).SetDamageSource(0, 20 * GetMultiplierForGrade()).CalculateAndApplyDamage();
        }
        else
        {
            foreach (Unit enemy in ItemUseAbility.AffectedEnemies.Keys)
            {
                enemy.AddEffect(new Effect_Stun(new(this)), 5);
            }
        }
    }

    private void PlaySound() {
        if(_soundPlayed == false) {
            _soundPlayed = true;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/VacuumGrenade_Explosion", 0.7f);
        }
    }

    public void ExplodeIfNotHittingEnemy()
    {
        if (ToolObject != null && EnemyWasHit == false)
        {
            Activate();
        }
    }
}
