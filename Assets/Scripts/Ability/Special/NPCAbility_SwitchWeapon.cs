using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;

public class NPCAbility_SwitchWeapon : Ability {
    public static float Cooldown = 15;
    public static new bool DoesNotRequireTarget = true;

    public NPCAbility_SwitchWeapon(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 100, Constants.DamageType.Heavy));
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if(User.gameObject.name.Contains("WeaponPillager")) {
            List<string> potentialSwitches = new() { "Greataxe", "Greathammer", "Retribution", "HeavyBlade", "KnightSword", "HeavenlyHalberd"};
            potentialSwitches.Remove(User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy").GetComponent<SpriteResolver>().GetLabel());
            GameObject heavySwitch = User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy " + potentialSwitches[UnityEngine.Random.Range(0, potentialSwitches.Count)]).gameObject;
            string label = User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy").GetComponent<SpriteResolver>().GetLabel();
            User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy").gameObject.name = "Heavy " + label;
            User.BaseInjury = 
                heavySwitch.gameObject.name.Contains("Greataxe") ? 80:
                heavySwitch.gameObject.name.Contains("Greathammer") ? 50:
                heavySwitch.gameObject.name.Contains("Retribution") ? 100:
                heavySwitch.gameObject.name.Contains("HeavyBlade") ? 125:
                heavySwitch.gameObject.name.Contains("KnightSword") ? 150:
                heavySwitch.gameObject.name.Contains("HeavenlyHalberd") ? 165 : 125;
            User.BaseStagger = 
                heavySwitch.gameObject.name.Contains("Greataxe") ? 170:
                heavySwitch.gameObject.name.Contains("Greathammer") ? 200:
                heavySwitch.gameObject.name.Contains("Retribution") ? 150:
                heavySwitch.gameObject.name.Contains("HeavyBlade") ? 125:
                heavySwitch.gameObject.name.Contains("KnightSword") ? 100:
                heavySwitch.gameObject.name.Contains("HeavenlyHalberd") ? 85 : 125;
            User.BaseAttackSpeed = 
                heavySwitch.gameObject.name.Contains("Greataxe") ? 0.9f:
                heavySwitch.gameObject.name.Contains("Greathammer") ? 0.8f:
                heavySwitch.gameObject.name.Contains("Retribution") ? 0.95f:
                heavySwitch.gameObject.name.Contains("HeavyBlade") ? 1:
                heavySwitch.gameObject.name.Contains("KnightSword") ? 1.1f:
                heavySwitch.gameObject.name.Contains("HeavenlyHalberd") ? 1.2f : 1;
            heavySwitch.gameObject.name = "Heavy";
            User.InitializeSpriteRenderers();
            User.Animator.Rebind();
            User.PlayAnimation("SwitchWeapon", 0, 0);
        }
    }

    public override void CallAbilityEvent1()
    {
        User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy/Heavy Bone").GetComponent<UnitWeapon>().ChangeWeaponDealingDamage(true, this);
    }

    public override void CallAbilityEvent2()
    {
        User.SpriteRenderers["Upper Body"].Bone.transform.Find("Heavy/Heavy Bone").GetComponent<UnitWeapon>().ChangeWeaponDealingDamage(false, this);
    }
}