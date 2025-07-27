using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_TectonicPull : Ability {

    public static float Cooldown = 25;
    public static AbilityFamily Family = AbilityFamily.Molis;

    private GameObject _aoe;
    public NPCAbility_TectonicPull(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 500, Constants.DamageType.Light));
        AddCustomSound("Crack", "Earth/Earth_Crack4", 0.8f);
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
        WaitTimeBeforeNextAction = 0f;
    }

    public override void CallAbilityEvent1()
    {
        _aoe = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AreaOfEffect/AreaOfEffect_Ryker_GroundSlam")) as GameObject;
        _aoe.transform.position = User.transform.position + new Vector3(User.Actions.IsFlipped ? -6f : 6f, 0);
        _aoe.transform.Find("AoE").GetComponent<AreaOfEffect>().SourceAbility = this;
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        float random = UnityEngine.Random.Range(0, 100);
        if(User.UnitAI.Actions.ContainsKey(typeof(NPCAbility_ChargedPunch)) && (User.CurrentHealthBars > 2 && random < 25) || (User.CurrentHealthBars == 2 && random < 50) || (User.CurrentHealthBars == 1 && random < 75))
        {
            User.UnitAI.PredeterminedNextAction = typeof(NPCAbility_ChargedPunch);
        } 
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.PushIntoPosition(User.transform.position - new Vector3(0.5f * (User.Actions.IsFlipped ? -1 : 1), 0), this);
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 2.5f);
    }
}