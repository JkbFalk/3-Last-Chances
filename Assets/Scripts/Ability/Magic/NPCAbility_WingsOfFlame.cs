using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCAbility_WingsOfFlame : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;

    public AreaOfEffect Wing1;
    public AreaOfEffect Wing2;

    public NPCAbility_WingsOfFlame(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/FlamethrowerLoop1", 0.2f);
        DamageSources.Add(new DamageSource(250, 350, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        CanBeInterruptedByFlinching = false;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)), new Effect_Unstoppable(new(this)) };
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        Wing1 = Utils.CreateAreaOfEffect(new(this), "FlameWing");
        Wing1.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        Wing2 = Utils.CreateAreaOfEffect(new(this), "FlameWing");
        Wing2.GetComponent<AttachObjectToBodyPart>().BodyPartName = "Left Arm";
        Wing2.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        GameController.Instance.WaitAndRunMethod(0.01f, AdjustTransform);
    }

    public void AdjustTransform() {
        Wing1.transform.localPosition = new Vector2(4, -0.2f);
        Wing1.transform.localEulerAngles = new Vector3(0, 0, 90);
        Wing2.transform.localPosition = new Vector2(4, 0.2f);
        Wing2.transform.localEulerAngles = new Vector3(0, 180, -90);
    }

    public override void CallAbilityEvent2()
    {
        Wing1.DealingDamage = true;
        Wing2.DealingDamage = true;
    }

    public override void CallAbilityEvent3()
    {
        if(Wing1 != null && Wing1.gameObject.IsDestroyed() == false) {
            Wing1.MakeObjectDisappear();
        }
        if(Wing2 != null && Wing2.gameObject.IsDestroyed() == false) {
            Wing2.MakeObjectDisappear();
        }
        
        
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(30 * User.MagicStagger.Current / 100, new(this)));
    }
}