using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SpearCyclone : Ability {
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Anima;
    private GameObject _vfx;

    public NPCAbility_SpearCyclone(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 150, Constants.DamageType.Heavy) {Knockback = -200});
        DamageSources.Add(new DamageSource(0, 150, Constants.DamageType.Heavy, "2") {Knockback = -200});
        DamageSources.Add(new DamageSource(125, 0, Constants.DamageType.Heavy, "3") {Knockback = 500});
        HitsTriggerDamagedState = false;
        WaitTimeBeforeNextAction = 0.3f;
        HitSoundVolume = 0.6f;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_Unstoppable(new(this)) };
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