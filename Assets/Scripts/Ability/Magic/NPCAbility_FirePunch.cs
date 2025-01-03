using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FirePunch : Ability {

    public static float Cooldown = 7;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;
    public NPCAbility_FirePunch(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/Inflame", 0.4f);
        AddCustomSound("Swing", "Fire/Fire1", 0.4f);
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(200, 200, Constants.DamageType.Magic));
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByBackstep);
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "FirePunch");
        _aoe.GetComponent<AttachObjectToBodyPart>().Initialize(User);
    }

    public override void CallAbilityEvent2()
    {
        _aoe.transform.GetChild(0).gameObject.SetActive(true);
        ChaseCurrentTargetAtGivenDegreeAngle(125, 35, 25);
    }

    public override void CallAbilityEvent3()
    {
        _aoe.DealingDamage = true;
    }

    public override void CallAbilityEvent4()
    {
        if(_aoe != null && _aoe.gameObject != null && _aoe.IsDestroyed() == false) {
            _aoe.DealingDamage = false;
            _aoe.MakeObjectDisappear(0.5f);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        CallAbilityEvent4();
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(40 * User.MagicStagger.Current / 100, new(this)));
    }
}