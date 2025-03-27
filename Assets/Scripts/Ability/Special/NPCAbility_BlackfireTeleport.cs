using UnityEngine;
using UnityEngine.AI;

public class NPCAbility_BlackfireTeleport : Ability {
    private Effect_Invincible _effect;
    private Vector2 _teleportPosition;
    private GameObject _visualEffect;
    public static float Cooldown = 10;
    public NPCAbility_BlackfireTeleport(Unit ability_user) : base(ability_user) {
        Cooldown = User.IsBoss ? 5 : 10;
        AddCustomSound("Teleport", "Criminal/Criminal_Daggers_MoveAbility", 0.5f);
        WaitTimeBeforeNextAction = 0.1f;
        Properties.Add(AbilityProperty.ImmuneToFlinch);
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        int rng = Random.Range(0, 3);
        Vector2 current_target_position = User.CurrentTarget.transform.position;
        bool current_target_flipped = User.CurrentTarget.Actions.IsFlipped;
        NavMeshPath path_found = new NavMeshPath();
        if (rng == 0 && User.UnitAI.NavMeshAgent.CalculatePath(new Vector2(current_target_position.x + (current_target_flipped ? 3 : -3), current_target_position.y + 3), path_found)) {
            _teleportPosition = new Vector2(current_target_position.x + (current_target_flipped ? 3 : -3), current_target_position.y + 3);
        }
        else if (rng == 1 && User.UnitAI.NavMeshAgent.CalculatePath(new Vector2(current_target_position.x + (current_target_flipped ? 3 : -3), current_target_position.y - 3), path_found)) {
            _teleportPosition = new Vector2(current_target_position.x + (current_target_flipped ? 3 : -3), current_target_position.y - 3);
        }
        else {
            _teleportPosition = new Vector2(current_target_position.x + (current_target_flipped ? 1.5f : -1.5f), current_target_position.y);
        }
        _effect = new Effect_Invincible(new(this));
        User.AddEffect(_effect);
    }

    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        User.EndEffect(_effect);
    }

    public override void CallAbilityEvent1() {
        _visualEffect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_Criminal_Daggers_MoveAbility")) as GameObject;
        _visualEffect.transform.position = new Vector2(User.transform.position.x, User.transform.position.y - 0.7f);
    }

    public override void CallAbilityEvent2() {
        User.transform.position = _teleportPosition;
        User.Actions.IsFlipped = !User.Actions.IsFlipped;
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        if (user.CurrentTarget == null)
        {
            return false;
        }
        if (user.transform.position.x > user.CurrentTarget.transform.position.x && user.CurrentTarget.Actions.IsFlipped == false)
        {
            return true;
        }
        else if (user.transform.position.x < user.CurrentTarget.transform.position.x && user.CurrentTarget.Actions.IsFlipped)
        {
            return true;
        }
        return false;
    }
}