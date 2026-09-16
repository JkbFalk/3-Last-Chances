using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_WindmillStab : Ability {
    public static float Cooldown = 8;
    private int _charge = 0;
    GameObject _vfx;
    public NPCAbility_WindmillStab(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.5f;
        AddCustomSound("ChargeUp", "Greatsword/Greatsword_HeavySwing9", 0.3f);
        AddCustomSound("Explosion", "Explosion/Ground Explosion", 0.8f);
        AddCustomSound("Stab", "Greatsword/Greatsword_HeavySwing15", 0.7f);
        Properties.Add(Property.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
        Properties.Add(Property.CounteredByRoll);
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }

    public override void CallAbilityEvent1()
    {
        _charge++;
        PlayCustomSound("ChargeUp");
        /*_charge++;
        if(_charge >= 3 && UnityEngine.Random.Range(0, 100) < 25) {
            User.PlayAnimation("NPCAbility_WindmillStab", 0.05f, 0.63f);
            CallAbilityEvent2();
        }
        else {
            PlayCustomSound("ChargeUp");
        }*/
    }

    public override void CallAbilityEvent2()
    {
        foreach(Effect e in EffectsAffectingUserDuringAbility) {
            e.EndThisEffect();
        }
        PlayCustomSound("Stab");
        PlayCustomSound("Explosion");
        DamageSources.Add(new DamageSource(150, 300 + _charge * 100, Constants.DamageType.Heavy));
        ChaseCurrentTargetAtGivenDegreeAngle(1 + 0.2f * _charge, 50 - 5 * _charge);
    }
    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        _vfx = Utils.CreateVisualEffect(new(this), "AnimaTrail");
        _vfx.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        _vfx.transform.localEulerAngles = Vector3.zero;
        _vfx.GetComponent<ParticleSystem>().Play();
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        _vfx.GetComponent<TemporaryObject>().MakeObjectDisappear();
    }
}