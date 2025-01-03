using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_TripleEruption : Ability {

    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Molis;
    private List<GameObject> _aoeList;
    private int _areaIncreaseCounter = 0;
    public NPCAbility_TripleEruption(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(20, 60, Constants.DamageType.Light) {Knockback = 70});
        AddCustomSound("Crack", "Earth/Earth_Crack2", 0.6f);
        WaitTimeBeforeNextAction = 0.2f;
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        PlayCustomSound("Crack");
        GameController.Instance.WaitAndRunMethod(0.15f / User.LightAttackSpeed.Current, CreateArea);
        GameController.Instance.WaitAndRunMethod(0.4f / User.LightAttackSpeed.Current, IncreaseArea);
    }

    public void CreateArea()
    {
        GameObject shockwave = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AreaOfEffect/AreaOfEffect_Ryker_Shockwave" + (User.Actions.IsFlipped ? " Flipped" : ""))) as GameObject;
        shockwave.transform.position = User.ProjectileSpawnLocation.transform.position;
        _aoeList = new List<GameObject>()
        {
            shockwave.transform.Find("AreaOfEffect_Ryker_Shockwave 0").gameObject,
            shockwave.transform.Find("AreaOfEffect_Ryker_Shockwave 45").gameObject,
            shockwave.transform.Find("AreaOfEffect_Ryker_Shockwave -45").gameObject
        };
        foreach (AreaOfEffect aoe in shockwave.GetComponentsInChildren<AreaOfEffect>(true))
        {
            aoe.SourceAbility = this;
        }
    }

    public void IncreaseArea()
    {
        _areaIncreaseCounter++;
        PlayCustomSound("Crack");
        foreach (GameObject aoe in _aoeList)
        {
            aoe.transform.Find("AreaOfEffect_Ryker_Shockwave_" + _areaIncreaseCounter).gameObject.SetActive(true);
        }
        if (_areaIncreaseCounter < 4)
        {
            GameController.Instance.WaitAndRunMethod(0.4f / User.LightAttackSpeed.Current, IncreaseArea);
        }
    }
}