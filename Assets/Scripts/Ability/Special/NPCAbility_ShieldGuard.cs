using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCAbility_ShieldGuard : Ability {
    public static float Cooldown = 15;
    public static new bool DoesNotRequireTarget = true;
    public static AbilityFamily Family = AbilityFamily.Molis;

    private int _checkCounter = 0;
    private readonly float _moveSpeedIncrease = 50;
    private Effect_ShieldedByAlly _effectOnTarget;
    private Effect_ShieldingAnAlly _effectOnCaster;

    public NPCAbility_ShieldGuard(Unit ability_user) : base(ability_user) {
        CanBePlundered = false;
        WaitTimeBeforeNextAction = Random.Range(0.3f, 1f);
        AutoPlayAbilityAnimation = false;
        CanMoveWhileUsing = true;
        WaitTimeBeforeNextAction = 0.1f;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_Unstoppable(new(this)) };
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        User.PlayAnimation("Run");
        User.MovementSpeed.AddPercentageModifier(this, _moveSpeedIncrease);
        Target = User.CurrentTarget.MostRecentEnemyHit;
        float horizontalAdjustment = Target.transform.position.x > User.CurrentTarget.transform.position.x ? -0.5f : 0.5f;
        if(User.UnitAI.NavMeshAgent.isOnNavMesh) {
            User.GetComponent<UnitAI>().NavMeshAgent.SetDestination(User.CurrentTarget.MostRecentEnemyHit.transform.position + new Vector3(horizontalAdjustment, 0));
            User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Repositioning;
            GameController.Instance.WaitAndRunMethod(0.1f, CheckIfArrivedNearAlly);
        }
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        return
            !user.CheckIfUnderEffect(typeof(Effect_ShieldedByAlly)) &&
            user.CurrentTarget != null &&
            user.CurrentTarget.MostRecentEnemyHit != null &&
            user.CurrentTarget.MostRecentEnemyHit != user &&
            !user.CurrentTarget.MostRecentEnemyHit.KnockedOut &&
            Vector2.Distance(user.CurrentTarget.MostRecentEnemyHit.transform.position, user.transform.position) < 5;
    }

    public void CheckIfArrivedNearAlly() {
        if(User.KnockedOut)
        {
            return;
        }
        if(User.CheckIfUnderEffect(typeof(Effect_ShieldedByAlly)))
        {
            EndThisAbility();
        }
        if (User.GetComponent<UnitAI>().NavMeshAgent.remainingDistance < 0.15f) {
            CanMoveWhileUsing = false;
            User.MovementSpeed.RemovePercentageModifier(this);
            User.PlayAnimation("ShieldGuard");
            _effectOnCaster = new Effect_ShieldingAnAlly(new(this));
            _effectOnTarget = new Effect_ShieldedByAlly(new(this));
            User.AddEffect(_effectOnCaster);
            Target.AddEffect(_effectOnTarget);
        }
        else if (_checkCounter < 50) {
            float horizontalAdjustment = Target.transform.position.x > User.CurrentTarget.transform.position.x ? -0.5f : 0.5f;
            User.GetComponent<UnitAI>().NavMeshAgent.SetDestination(User.CurrentTarget.MostRecentEnemyHit.transform.position + new Vector3(horizontalAdjustment, 0));
            GameController.Instance.WaitAndRunMethod(0.1f, CheckIfArrivedNearAlly);
            _checkCounter++;
        }
        else {
            EndThisAbility();
        }
    }

    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        if(Target != null && _effectOnTarget != null && Target.CheckIfUnderEffect(_effectOnTarget.GetType()))
        {
            Target.EndEffect(_effectOnTarget);
        }
        if (User != null && _effectOnCaster != null &&  User.CheckIfUnderEffect(_effectOnCaster.GetType()))
        {
            User.EndEffect(_effectOnCaster);
        }
    }
}