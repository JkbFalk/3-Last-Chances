// FILE: Assets/Scripts/Base Class/UnitBoundsManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBoundsManager : MonoBehaviour
{
    private static UnitBoundsManager _instance;
    public static UnitBoundsManager Instance => _instance;

    [Header("Detection Thresholds")]
    [Tooltip("Distance the feet collider can be from the NavMesh edge before being considered out of bounds. Accommodates the baked Agent Radius gap near walls.")]
    [SerializeField] private float _allowedOffMeshDistance = 1.6f;

    [Tooltip("Search radius when searching for nearest valid walkable ground.")]
    [SerializeField] private float _recoverySearchRadius = 8.0f;

    [Tooltip("Amount to shrink the physical BoxCollider when testing if embedded inside a wall. Prevents touching a wall from triggering a bounce.")]
    [SerializeField] private float _wallPenetrationSkinWidth = 0.15f;

    [Tooltip("Number of consecutive failed checks required before forcing a reposition.")]
    [SerializeField] private int _consecutiveFailsToTrigger = 3;

    [Header("Performance Settings")]
    [Tooltip("How many NPC units to evaluate per fixed tick.")]
    [SerializeField] private int _npcsPerTick = 2;

    [Tooltip("How frequently (in seconds) the Player is evaluated.")]
    [SerializeField] private float _playerCheckInterval = 0.2f;

    private readonly List<Unit> _trackedUnits = new List<Unit>();
    private readonly Dictionary<Unit, Vector2> _lastValidPositions = new Dictionary<Unit, Vector2>();
    private readonly Dictionary<Unit, int> _outOfBoundsFailCounters = new Dictionary<Unit, int>();

    private int _roundRobinIndex = 0;
    private float _playerCheckTimer = 0f;
    private int _environmentLayerMask;
    private float _navMeshZ = 0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        _environmentLayerMask = LayerMask.GetMask("Environment");
    }

    private void OnEnable()
    {
        EventManager.FinishedLoadingArea.AddListener(OnAreaLoaded);
        EventManager.UnitKnockedOut.AddListener(OnUnitKnockedOut);
    }

    private void OnDisable()
    {
        EventManager.FinishedLoadingArea.RemoveListener(OnAreaLoaded);
        EventManager.UnitKnockedOut.RemoveListener(OnUnitKnockedOut);
    }

    private void OnAreaLoaded()
    {
        _trackedUnits.Clear();
        _lastValidPositions.Clear();
        _outOfBoundsFailCounters.Clear();
        _roundRobinIndex = 0;
        _playerCheckTimer = 0f;

        if (NavMesh.SamplePosition(Vector3.zero, out NavMeshHit hit, 100f, NavMesh.AllAreas))
        {
            _navMeshZ = hit.position.z;
        }

        foreach (Unit unit in Utils.GetAllUnits(false, true))
        {
            RegisterUnit(unit);
        }
    }

    private void OnUnitKnockedOut(DamageInstance damage)
    {
        if (damage.TargetOfDamage != null)
        {
            UnregisterUnit(damage.TargetOfDamage);
        }
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit == null || _trackedUnits.Contains(unit))
        {
            return;
        }

        _trackedUnits.Add(unit);
        _lastValidPositions[unit] = unit.transform.position;
        _outOfBoundsFailCounters[unit] = 0;
    }

    public void UnregisterUnit(Unit unit)
    {
        if (unit == null) return;
        _trackedUnits.Remove(unit);
        _lastValidPositions.Remove(unit);
        _outOfBoundsFailCounters.Remove(unit);
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.GameplayMode != Constants.GameplayMode.Regular)
        {
            return;
        }
        _playerCheckTimer += Time.fixedDeltaTime;
        if (_playerCheckTimer >= _playerCheckInterval)
        {
            _playerCheckTimer = 0f;
            if (Player.HasInstance() && !Player.Instance.KnockedOut)
            {
                ValidateAndEnforceBounds(Player.Instance);
            }
        }
        if (_trackedUnits.Count == 0)
        {
            return;
        }

        int checksThisFrame = Mathf.Min(_npcsPerTick, _trackedUnits.Count);
        for (int i = 0; i < checksThisFrame; i++)
        {
            if (_roundRobinIndex >= _trackedUnits.Count)
            {
                _roundRobinIndex = 0;
            }

            Unit unit = _trackedUnits[_roundRobinIndex];
            _roundRobinIndex++;

            if (unit == null || !unit.gameObject.activeInHierarchy || unit.KnockedOut || unit is Player)
            {
                continue;
            }

            ValidateAndEnforceBounds(unit);
        }
    }

    public void ValidateAndEnforceBounds(Unit unit)
    {
        if (unit == null || unit.KnockedOut)
        {
            return;
        }
        BoxCollider2D targetCollider = GetRelevantBoxCollider(unit);
        Vector2 colliderCenter = targetCollider != null ? (Vector2)targetCollider.bounds.center : (Vector2)unit.transform.position;
        Vector2 rootOffset = colliderCenter - (Vector2)unit.transform.position;

        Vector3 queryPos = new Vector3(colliderCenter.x, colliderCenter.y, _navMeshZ);

        bool foundMesh = NavMesh.SamplePosition(queryPos, out NavMeshHit hit, _recoverySearchRadius, NavMesh.AllAreas);
        Vector2 hitPos2D = foundMesh ? new Vector2(hit.position.x, hit.position.y) : colliderCenter;
        float distToMesh = foundMesh ? Vector2.Distance(colliderCenter, hitPos2D) : float.MaxValue;
        bool isEmbeddedInWall = targetCollider != null && IsColliderEmbeddedInWall(targetCollider, unit);
        bool isValid = foundMesh && (distToMesh <= _allowedOffMeshDistance) && !isEmbeddedInWall;

        if (isValid)
        {
            _lastValidPositions[unit] = unit.transform.position;
            _outOfBoundsFailCounters[unit] = 0;
            return;
        }

        if (!_outOfBoundsFailCounters.ContainsKey(unit))
        {
            _outOfBoundsFailCounters[unit] = 0;
        }
        _outOfBoundsFailCounters[unit]++;
        bool emergencyRecovery = distToMesh > (_allowedOffMeshDistance * 2.5f);

        if (_outOfBoundsFailCounters[unit] < _consecutiveFailsToTrigger && !emergencyRecovery)
        {
            return;
        }
        Vector2 targetRootPos;
        if (foundMesh && !IsPointInsideWall(hitPos2D, unit))
        {
            targetRootPos = hitPos2D - rootOffset;
        }
        else if (_lastValidPositions.TryGetValue(unit, out Vector2 lastSafe))
        {
            targetRootPos = lastSafe;
        }
        else
        {
            targetRootPos = (Vector2)unit.transform.position - rootOffset;
        }

        RepositionUnit(unit, targetRootPos);
        _lastValidPositions[unit] = targetRootPos;
        _outOfBoundsFailCounters[unit] = 0;
    }

    /// <summary>
    /// Gets exclusively the BoxCollider2D on Environment Collision or Lower Body Bone.
    /// </summary>
    private BoxCollider2D GetRelevantBoxCollider(Unit unit)
    {
        Transform lowerBody = unit.transform.Find("Lower Body/Lower Body Bone");
        if (lowerBody != null)
        {
            Transform env = lowerBody.Find("Environment Collision");
            if (env != null)
            {
                BoxCollider2D envCol = env.GetComponent<BoxCollider2D>();
                if (envCol != null && envCol.enabled)
                {
                    return envCol;
                }
            }
            BoxCollider2D bodyCol = lowerBody.GetComponent<BoxCollider2D>();
            if (bodyCol != null && bodyCol.enabled)
            {
                return bodyCol;
            }
        }
        return unit.Collider;
    }
    private static readonly Collider2D[] _overlapBuffer = new Collider2D[16];

    private bool IsColliderEmbeddedInWall(BoxCollider2D boxCol, Unit unit)
    {
        Vector2 boundsSize = boxCol.bounds.size;
        float shrinkX = Mathf.Min(_wallPenetrationSkinWidth * 2f, boundsSize.x * 0.4f);
        float shrinkY = Mathf.Min(_wallPenetrationSkinWidth * 2f, boundsSize.y * 0.4f);
        Vector2 testSize = new Vector2(boundsSize.x - shrinkX, boundsSize.y - shrinkY);

        if (testSize.x <= 0.02f || testSize.y <= 0.02f)
        {
            return false;
        }
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_environmentLayerMask);
        filter.useTriggers = false;

        int hitCount = Physics2D.OverlapBox(boxCol.bounds.center, testSize, 0f, filter, _overlapBuffer);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = _overlapBuffer[i];
            if (col == null || col.isTrigger) continue;
            if (col.transform.root == unit.transform.root) continue;

            return true;
        }
        return false;
    }

    private bool IsPointInsideWall(Vector2 point, Unit unit)
    {
        Collider2D col = Physics2D.OverlapPoint(point, _environmentLayerMask);
        return col != null && !col.isTrigger && col.transform.root != unit.transform.root;
    }

    private void RepositionUnit(Unit unit, Vector2 targetRootPos)
    {
        if (unit.Rigidbody2D != null)
        {
            unit.Rigidbody2D.linearVelocity = Vector2.zero;
            unit.Rigidbody2D.position = targetRootPos;
        }

        NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled)
        {
            if (agent.isOnNavMesh)
            {
                agent.Warp(new Vector3(targetRootPos.x, targetRootPos.y, _navMeshZ));
            }
            else
            {
                agent.enabled = false;
                unit.transform.position = new Vector3(targetRootPos.x, targetRootPos.y, unit.transform.position.z);
                agent.enabled = true;
            }
        }
        else
        {
            unit.transform.position = new Vector3(targetRootPos.x, targetRootPos.y, unit.transform.position.z);
        }
    }
}