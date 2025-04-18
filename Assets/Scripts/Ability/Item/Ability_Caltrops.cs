using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

public class Ability_Caltrops : Ability
{
    public static int[] CaltropsAmount =  {20, 25, 30, 35, 40};
    public List<Projectile> Caltrops;
    public float SlowApplied;
    public float IncisionApplied;
    public Ability_Caltrops(Unit ability_user, Item item) : base(ability_user)
    {
        if(item.DamageSources != null && item.DamageSources.Count > 0)
        {
            DamageSources = item.DamageSources;
        }
        ItemBeingUsed = item;
        ItemBeingUsed.ItemUseAbility = this;
        CustomHitSound = "Blade/Blade_BloodStab1";
        HitSoundVolume = 0.35f;
        IncisionApplied = 0.125f * ItemBeingUsed.GetItemFirstEffectPB();
        SlowApplied = 2 + 0.03125f * ItemBeingUsed.GetItemFirstEffectPB();
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemType.Tool, "Caltrops");
    }

    public override void CallAbilityEvent1()
    {
        Utils.PlaySoundEffect(User.AudioSource, "Item/ThrowCaltrops", 0.2f);
        Caltrops = new();
        for(int i = 0; i < CaltropsAmount[ItemBeingUsed.GradeIndex]; i++) {
            Caltrops.Add(Utils.CreateProjectile(new(this), "Caltrop"));
            Caltrops[i].transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(30, 130 + ItemBeingUsed.GradeIndex * 10) * (User.Actions.IsFlipped ? 1 : -1));
            Caltrops[i].FlightSpeed = UnityEngine.Random.Range(3f, 5f + ItemBeingUsed.GradeIndex * 1);
        }
        GameController.Instance.WaitAndRunMethod(1, ArmCaltrops);
        GameController.Instance.WaitAndRunMethod(20, DestroyCaltrops);
    }

    public void ArmCaltrops() {
        foreach(Projectile p in Caltrops) {
            p.DealingDamage = true;
        }
    }

    public void DestroyCaltrops() {
        foreach(Projectile p in Caltrops) {
            if(p != null && p.IsDestroyed() == false) {
                p.MakeObjectDisappear(0);
            }
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        unit_getting_attacked.AddEffect(new Effect_Slow(SlowApplied, new(this)));
        unit_getting_attacked.AddEffect(new Effect_Incision(IncisionApplied, new(this)));
        object_hitting.MakeObjectDisappear(0);
    }
}