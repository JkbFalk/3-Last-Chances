using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BA_Daggers_S : BasicAttack {
    public static Unit ValidTarget;

    public BA_Daggers_S(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.StrongBasicAttack);
        DamageSources.Add(new DamageSource(250 / 2, 350 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        AddCustomSound("Teleport1", "Ability/Light Sword Swing 10", 0.7f);
        AddCustomSound("Teleport2", "Footsteps/Footsteps_Gravel10", 0.8f);
    }

    public override void CallAbilityEvent1()
    {
        Unit target = ValidTarget != null && !ValidTarget.KnockedOut 
            ? ValidTarget 
            : (User.CurrentTarget != null ? User.CurrentTarget : User.GetClosestValidTarget());
        if (target == null)
        {
            return;
        }
        Vector3 destination = GetSafeTeleportPosition(target);
        User.Rigidbody2D.linearVelocity = Vector2.zero;
        User.transform.position = destination;
        User.Actions.FaceUnit(target);
    }

    private Vector3 GetSafeTeleportPosition(Unit target)
    {
        Vector3 targetPos = target.transform.position;
        float behindDir = target.Actions.IsFlipped ? 1f : -1f;
        int envLayerMask = LayerMask.GetMask("Environment");
        List<Vector3> candidateOffsets = new List<Vector3>
        {
            new Vector3(behindDir * 2.0f, 0f, 0f),
            new Vector3(behindDir * 1.5f, 0f, 0f),
            new Vector3(behindDir * 1.2f, 0f, 0f),
            new Vector3(behindDir * 1.6f, 0.6f, 0f),
            new Vector3(behindDir * 1.6f, -0.6f, 0f),
            new Vector3(behindDir * 1.2f, 0.6f, 0f),
            new Vector3(behindDir * 1.2f, -0.6f, 0f),
            new Vector3(-behindDir * 1.5f, 0f, 0f), // Front fallback
            new Vector3(-behindDir * 1.2f, 0f, 0f)
        };
        foreach (Vector3 offset in candidateOffsets)
        {
            Vector3 candidatePos = targetPos + offset;
            RaycastHit2D wallHit = Physics2D.Linecast(targetPos, candidatePos, envLayerMask);
            if (wallHit.collider != null)
            {
                continue;
            }
            Collider2D overlap = Physics2D.OverlapCircle(candidatePos, 0.35f, envLayerMask);
            if (overlap != null)
            {
                continue;
            }
            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit navHit, 0.6f, NavMesh.AllAreas))
            {
                return navHit.position;
            }
        }
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit fallbackHit, 2f, NavMesh.AllAreas))
        {
            return fallbackHit.position;
        }
        return User.transform.position;
    }

    public static bool CheckIfAnyValidTargetInRange() {
        Vector2 check = Player.Instance.Actions.IsFlipped 
            ? (Player.Instance.transform.position + new Vector3(-3, 0)) 
            : (Player.Instance.transform.position + new Vector3(3, 0));

        if (Player.Instance.CurrentTarget != null) {
            ValidTarget = Player.Instance.CurrentTarget;
            return Vector2.Distance(check, Player.Instance.CurrentTarget.transform.position) < 5;
        }

        List<Unit> enemies = Utils.GetAllUnits(true, true)
            .OrderBy(u => Vector2.Distance(u.transform.position, check))
            .ToList();

        ValidTarget = enemies.Count > 0 ? enemies[0] : null;
        return enemies != null && enemies.Count > 0 && Vector2.Distance(enemies[0].transform.position, check) < 5;
    }
}