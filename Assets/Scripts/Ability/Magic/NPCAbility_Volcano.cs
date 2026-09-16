using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAbility_Volcano : Ability {

    public static float Cooldown = 25;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public int PlannedEruptionCount;
    private int _counter = 0;
    private Vector2 _startPosition;

    public NPCAbility_Volcano(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 100, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        PlannedEruptionCount = UnityEngine.Random.Range(50, 100);
        WaitTimeBeforeNextAction = 0.5f;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        _startPosition = User.transform.position;
        CreateEruption();
    }

    public void CreateEruption() {
        if(_counter < PlannedEruptionCount) {
            if (NavMesh.SamplePosition(_startPosition + UnityEngine.Random.insideUnitCircle * new Vector2(8f, 8f), out NavMeshHit repositionHit, 8f, NavMesh.AllAreas))
            {
                Utils.CreateAreaOfEffect(new(this), "Volcano", repositionHit.position.x, repositionHit.position.y);
            }
            _counter++;
            GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(0.02f, 0.1f), CreateEruption);
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(25 * User.MagicStagger.Current / 100, new(this)));
    }
}