using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_UpperCut : Ability {

    public static float Cooldown = 6;
    private AreaOfEffect _aoe;
    public NPCAbility_UpperCut(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Light));
        HitSoundType = Constants.HitSoundTypeEnum.SmallBlunt;
        WaitTimeBeforeNextAction = 0.1f;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(3.5f, 10);
        _aoe = Utils.CreateAreaOfEffect(new(this), "SmallCircleAoE");
        _aoe.transform.SetParent(User.SpriteRenderers["Right Arm"].Bone);
        _aoe.GetComponent<DestroyGameObjectAfterGivenTime>().DestroyAfterSeconds = 1.5f;
        _aoe.transform.localPosition = new Vector3(0, 0);
        _aoe.DealingDamage = true;
    }

    public override void CallAbilityEvent2()
    {
        _aoe.DealingDamage = false;
    }
}