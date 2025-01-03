using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BlastDash : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;

    public NPCAbility_BlastDash(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.1f;
        DamageSources.Add(new DamageSource(200, 200, Constants.DamageType.Heavy));
        AddCustomSound("Blast", "Fire/Fire14", 0.5f);
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByBackstep);
    }

    public override void CallAbilityEvent1()
    {
        GameObject vfx = Utils.CreateVisualEffect(new(this), "BlastDash");
        vfx.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        vfx.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? -90 : 90);
        ChaseCurrentTargetAtGivenDegreeAngle(350, 10, 30);
        User.Actions.PlayAbilityCustomSound("Blast");
    }
    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(60 * User.HeavyStagger.Current / 100, new(this)));
    }
}