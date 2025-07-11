using System.Transactions;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BerserkerRage : Ability {

    public static new bool DoesNotRequireTarget = true; 
    public static float Cooldown = 45;
    public NPCAbility_BerserkerRage(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(250, 0, Constants.DamageType.Light));
        AddCustomSound("Use", "Greatsword/Greatsword_HeavySwing15", 0.8f);
        WaitTimeBeforeNextAction = 0f;
        Properties.Add(Property.ImmuneToFlinch);
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if(User.SpriteRenderers["Heavy"].SpriteRenderer.gameObject.name == "Heavy") {
            User.SetWeaponActive("Light Left", true);
            User.SetWeaponActive("Light Right", true);
            User.SetWeaponActive("Heavy", false);
            GameController.Instance.WaitAndRunMethod(0.05f, PlayAbilityAgain);
        }
    }

    public void PlayAbilityAgain() {
        User.PlayAnimation("BerserkerRage");
        User.Actions.CurrentAbilityBeingPerformed = this;
    }

    public override void CallAbilityEvent1()
    {
        PlayCustomSound("Use");
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        User.UnitAI.AvailableActions = new List<string> {
            "10,AI_Chase", "25,ThunderStep", "50,QuickLeftAndRightDown"
        };
        User.UnitAI.InitializeAvailableActions();
        User.AddEffect(new Effect_BerserkerRage(new(this)), 15);
    }
}