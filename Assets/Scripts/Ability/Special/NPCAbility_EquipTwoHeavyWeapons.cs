using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;

public class NPCAbility_EquipTwoHeavyWeapons : Ability {
    public static float Cooldown = 20;
    public static new bool DoesNotRequireTarget = true;

    public NPCAbility_EquipTwoHeavyWeapons(Unit ability_user) : base(ability_user) {
        AddCustomSound("Swing", "Heavy Object/HeavyObject_Swing1", 0.6f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        DamageSources.Add(new DamageSource(200 / 2, 200 / 2, Constants.DamageType.Light));
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if(User.gameObject.name.Contains("WeaponPillager")) {
            foreach(Transform child in User.SpriteRenderers["Upper Body"].Bone.transform) {
                if(child.gameObject.name.Contains("Heavy") || child.gameObject.name.Contains("Light")) {
                    child.gameObject.name = "Heavy " + child.GetComponent<SpriteResolver>().GetLabel();
                }
            }
            int random = UnityEngine.Random.Range(0, 100);
            string lightRight = random < 30 ? "Greataxe" : random < 70 ? "HeavyBlade" : "HeavenlyHalberd";
            string lightLeft = random < 30 ? "Greathammer" : random < 70 ? "Retribution" : "KnightSword";
            User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy " + lightRight).GetChild(0).gameObject.name = "Light Right Bone";
            User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy " + lightRight).gameObject.name = "Light Right";
            User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy " + lightLeft).GetChild(0).gameObject.name = "Light Left Bone";
            User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy " + lightLeft).gameObject.name = "Light Left";
            User.Animator.Rebind();
            User.InitializeSpriteRenderers();
            User.DamageCategory = Constants.DamageType.Light;
            User.UnitAI.InitializeAvailableActions(new() {{"50,AI_Chase"},{"100,EquipTwoHeavyWeapons"},{"15,ScissorSlash"},{"0,DoubleEarthRipper"},{"10,WindStep"},{"10,DashAndDualSlash"},{"10,RunningAndSlashing"},{"10,TwinBladesZigZag"}});
            User.BaseInjury = random < 30 ? 80 : random < 70 ? 140 : 180 ;
            User.BaseStagger = random < 30 ? 200 : random < 70 ? 140 : 100;
            User.BaseAttackSpeed = random < 30 ? 0.85f : random < 70 ? 1 : 1.15f;
            User.PlayAnimation("EquipTwoHeavyWeapons", 0, 0);
        }
    }

    public override void CallAbilityEvent1()
    {
        User.SpriteRenderers["Upper Body"].Bone.transform.Find("Light Right/Light Right Bone").GetComponent<UnitWeapon>().ChangeWeaponDealingDamage(true, this);
        User.SpriteRenderers["Upper Body"].Bone.transform.Find("Light Left/Light Left Bone").GetComponent<UnitWeapon>().ChangeWeaponDealingDamage(true, this);
    }

    public override void CallAbilityEvent2()
    {
        User.SpriteRenderers["Upper Body"].Bone.transform.Find("Light Right/Light Right Bone").GetComponent<UnitWeapon>().ChangeWeaponDealingDamage(false, this);
        User.SpriteRenderers["Upper Body"].Bone.transform.Find("Light Left/Light Left Bone").GetComponent<UnitWeapon>().ChangeWeaponDealingDamage(false, this);
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        return true;
    }
}