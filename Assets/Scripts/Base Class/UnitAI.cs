using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class UnitAI : MonoBehaviour {
    public bool ReloadAvailableActions = false;

    [HideInInspector]
    private Constants.AIBehavior _currentAIBehavior = Constants.AIBehavior.None;

    public Constants.AIBehavior CurrentAIBehavior {
        get {
            return _currentAIBehavior;
        }
        set {
            _currentAIBehavior = value;
        }
    }
    public Collider2D MaxMoveRange;

    public List<string> AvailableActions = new List<string>();
    public Dictionary<Type, int> Actions = new Dictionary<Type, int>();
    public List<string> ActionsAfter1HBarBroken = new List<string>();
    public List<string> ActionsAfter2HBarsBroken = new List<string>();

    public Type PredeterminedNextAction;

    public float BaseAggressiveness = 1;
    /// <summary>
    /// Min value 0, Max value 3, Default value 1. Determines how often the unit stands around doing nothing, and how long those breaks last.
    /// </summary>
    public float Aggressiveness
    {
        get
        {
            float val = (BaseAggressiveness + AggressivenessModifier) * (SaveFile.Instance.DifficultyLevel == 0 ? 0.6f : SaveFile.Instance.DifficultyLevel == 1 ? 0.8f : 1);
            return val > 3 ? 3 : (val < 0 ? 0 : val);
        }
    }
    public float AggressivenessModifier = 0;

    public bool SkipObserve = false;

    public Dictionary<string, Unit> EnemiesInRangeForAbility = new Dictionary<string, Unit>();
    private Unit _unit;

    [HideInInspector]
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent {
        get {
            if(_navMeshAgent == null) {
                Start();
            }
            return _navMeshAgent;
        }
        set {
            _navMeshAgent = value;
        }
    }
    public int WaitTimeBeforeNextAction = 0;

    [HideInInspector]
    public bool CanMove = true;

    public enum DirectionType { AlwaysFaceTargetUnit, AlwaysFaceTargetPosition, FaceDirectionOfCurrentMovement, DontChangeFacingDirection }

    [HideInInspector]
    public DirectionType CurrentDirectionType = DirectionType.AlwaysFaceTargetUnit;

    public void OnValidate() {
        if(_unit is not Player && Application.isPlaying) {
            InitializeAvailableActions();
        }
    }

    public void Start() {
        _unit = GetComponent<Unit>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;
        if(_unit is not Player) {
            InitializeAvailableActions();
        }
    }

    private void Update() {
        if((GameController.Instance.GameplayMode != Constants.GameplayMode.InCutscene && _unit.Actions.MovingToPoint) || (GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene && _unit.Actions.MovingToPoint && Vector2.Distance(transform.position, _unit.Actions.MovingToPointDestination) < 0.25f)) {
            _unit.Actions.StopMovingToPoint();
        }
        if(GameController.Instance.GameplayMode != Constants.GameplayMode.Regular || _unit.InCombat == false) {
            return;
        }
        if(!NavMeshAgent.isOnNavMesh) {
            NavMeshHit myNavHit;
            if(NavMesh.SamplePosition(transform.position, out myNavHit, 100 , -1))
            {
            Debug.Log("UnitAI1 Player.Instance.transform.position: " + Player.Instance.transform.position);
            transform.position = myNavHit.position;
            Debug.Log("UnitAI2 Player.Instance.transform.position: " + Player.Instance.transform.position);
            }
        }
        if (CanMove && CurrentDirectionType != DirectionType.DontChangeFacingDirection) {
            if (CurrentDirectionType == DirectionType.AlwaysFaceTargetUnit && _unit.CurrentTarget != null && transform.position.x > _unit.CurrentTarget.transform.position.x && _unit.Actions.IsFlipped == false) {
                _unit.Actions.IsFlipped = true;
                _unit.UnitSpecificActionsAfterFlipDirection();
            }
            else if (CurrentDirectionType == DirectionType.AlwaysFaceTargetUnit && _unit.CurrentTarget != null && transform.position.x < _unit.CurrentTarget.transform.position.x && _unit.Actions.IsFlipped) {
                _unit.Actions.IsFlipped = false;
                _unit.UnitSpecificActionsAfterFlipDirection();
            }
            else if (CurrentDirectionType == DirectionType.FaceDirectionOfCurrentMovement) {
                _unit.Actions.FaceDirectionOfMovement();
            }
        }
    }

    private void FixedUpdate() {
        if (GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene) {
            return;
        }
        CanMove = Utils.CheckIfUnitCanMove(_unit) && CurrentAIBehavior != Constants.AIBehavior.Observing && CurrentAIBehavior != Constants.AIBehavior.None && CurrentAIBehavior != Constants.AIBehavior.UsingAbility && CurrentAIBehavior != Constants.AIBehavior.Waiting;
        if (NavMeshAgent.enabled)
        {
            if(NavMeshAgent.isOnNavMesh == false) {
                Debug.LogWarning("Unit not placed on NavMesh: " + Utils.GetGameObjectPath(gameObject));
            }
            else {
                NavMeshAgent.isStopped = !CanMove;
            }
        }
        if (!CanMove) {
            NavMeshAgent.velocity = Vector3.zero;
        }
        if (WaitTimeBeforeNextAction > 0) {
            WaitTimeBeforeNextAction--;
        }
        if (CurrentAIBehavior != Constants.AIBehavior.Waiting && CurrentAIBehavior != Constants.AIBehavior.Observing && NavMeshAgent.enabled && NavMeshAgent.isOnNavMesh && NavMeshAgent.remainingDistance < 0.35f) {
            WaitTimeBeforeNextAction = 0;
        }
        if (NavMeshAgent.enabled && _unit.CurrentTarget != null && CurrentAIBehavior == Constants.AIBehavior.ChasingCurrentTarget && (WaitTimeBeforeNextAction % 10 == 0)) {
            float horizontalAdjustment = _unit.CurrentTarget.transform.position.x > _unit.transform.position.x ? -_unit.DistanceAwayFromChaseTarget : _unit.DistanceAwayFromChaseTarget;
            Vector3 chasePosition = _unit.CurrentTarget.transform.localPosition + new Vector3(horizontalAdjustment, 0);
            if(MaxMoveRange != null && !MaxMoveRange.bounds.Contains(chasePosition) && NavMeshAgent.isOnNavMesh) {
                NavMeshAgent.SetDestination(MaxMoveRange.ClosestPoint(chasePosition));
            }
            else if(NavMeshAgent.isOnNavMesh){
                NavMeshAgent.SetDestination(chasePosition);
            }
        }
        if (_unit.CurrentTarget != null && Utils.CheckIfUnitCanPerformActions(_unit) && WaitTimeBeforeNextAction <= 0 && _unit.CheckIfUnitShouldDecideCustomAction() == false) {
            DecideOnNextAction();
        }
    }

    public int GetAdjustedWaitTime(int default_wait_time) {
        float maxObserveTime = SaveFile.Instance.DifficultyLevel == 0 ? 5 : SaveFile.Instance.DifficultyLevel == 1 ? 4 : 3;
        maxObserveTime /= Aggressiveness > 1 ? Aggressiveness : 1;
        return default_wait_time > maxObserveTime * 50 ? (int)(maxObserveTime * 50) : default_wait_time;

    }

    public void DecideOnNextAction(bool only_attacks = false) {
        if(GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene) {
            return;
        }
        if(PredeterminedNextAction != null) {
            if (EnemiesInRangeForAbility.ContainsKey(PredeterminedNextAction.ToString())) {
                _unit.CurrentTarget = EnemiesInRangeForAbility[PredeterminedNextAction.ToString()];
            }
            _unit.Actions.UseAbility(PredeterminedNextAction, false, _unit.CurrentTarget);
            PredeterminedNextAction = null;
            return;
        }
        if(only_attacks == false && SkipObserve == false && CheckIfShouldObserve())
        {
            return;
        }
        if(MaxMoveRange != null && !MaxMoveRange.bounds.Contains(_unit.transform.position) && NavMeshAgent.isOnNavMesh) {
                NavMeshAgent.SetDestination(MaxMoveRange.ClosestPoint(_unit.transform.position));
                return;
        }
        SkipObserve = false;
        int weightedChanceTotal = 0;
        SortedDictionary<int, Type> consideredActions = new SortedDictionary<int, Type>();
        foreach (Type action in Actions.Keys.Where(key => !only_attacks || key.ToString().Contains("AI_") == false)) {
            FieldInfo can_use_ability = _unit.GetType().GetField(action.ToString());
            FieldInfo does_not_require_target = action.GetField("DoesNotRequireTarget", BindingFlags.Public | BindingFlags.Static);
            if (Actions[action] > 0 && 
            (can_use_ability == null || (bool)can_use_ability.GetValue(_unit) == true) &&
            (action.IsSubclassOf(typeof(AI)) || (Ability.CheckIfCanPerformAbility(_unit, action) && 
            (EnemiesInRangeForAbility.ContainsKey(action.ToString()) || (does_not_require_target != null && (bool)does_not_require_target.GetValue(null))))))
            {
                weightedChanceTotal += Actions[action];
                consideredActions.Add(weightedChanceTotal, action);
            }
        }
        Type defaultAction = null;
        if(only_attacks && consideredActions.Count == 0) {
            if(Actions.ContainsKey(typeof(AI_RangedCombatReposition))) {
                defaultAction = typeof(AI_RangedCombatReposition);
            }
            else if(Actions.ContainsKey(typeof(AI_TryToFlank))) {
                defaultAction = typeof(AI_TryToFlank);
            }
            else {
                defaultAction = typeof(AI_Chase);
            }
        }
        int random = UnityEngine.Random.Range(1, weightedChanceTotal);
        foreach (int weightedChance in consideredActions.Keys) {
            if (random <= weightedChance) {
                PerformAction(defaultAction == null ? consideredActions[weightedChance] : defaultAction);
                break;
            }
        }
    }


    public void InitializeAvailableActions(List<string> new_actions = null)
    {
        if(_unit == null || _unit is Player || _unit?.EnemyDetection == null) {
            return;
        }
        foreach(Transform child in _unit.EnemyDetection.transform) {
            child.tag = "Marked For Destruction";
            MonoBehaviour.Destroy(child.gameObject);
        }
        Dictionary<Type, int> dict = new Dictionary<Type, int>();
        if(new_actions != null) {
            AvailableActions = new_actions;
        }
        foreach (string item in AvailableActions) {
            string action_name = "";
            try {
                action_name = item.Split(',')[1];
            }
            catch(Exception ex) {
                Debug.LogError("Incorrect action on UnitAI (" + _unit.gameObject.name + "): " + item + ": " + ex.Message);
            }
            if(!action_name.Contains("AI_")) {
                action_name = "NPCAbility_" + action_name;
            }
            if(Type.GetType(action_name) == null) {
                Debug.LogError("Could not find ability type with name: " + action_name);
            }
            dict.Add(Type.GetType(action_name), Int32.Parse(item.Split(',')[0]));
            MethodInfo extraActionsForAbility = Type.GetType(action_name).GetMethod("AdditionalActionsOnSettingsAbilityAsPotentialAction", BindingFlags.Public | BindingFlags.Static);
            if(extraActionsForAbility != null) {
                extraActionsForAbility.Invoke(null, new object[] {_unit});
            }
            FieldInfo does_not_require_target = Type.GetType(action_name).GetField("DoesNotRequireTarget", BindingFlags.Public | BindingFlags.Static);
            if(does_not_require_target == null || (bool)does_not_require_target.GetValue(null) == false) {
                if(!action_name.Contains("AI_") && (_unit.EnemyDetection.transform.Find(action_name) == null || _unit.EnemyDetection.transform.Find(action_name).tag == "Marked For Destruction")) {
                    try {
                        GameObject detection = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Enemy Detection/" + action_name)) as GameObject;
                        detection.transform.SetParent(_unit.EnemyDetection.transform, false);
                        detection.gameObject.name = action_name;
                    } catch(Exception ex) {
                        Debug.LogError("Could not find Enemy Detection prefab for action: " + action_name + " (" + ex.Message + ")");
                    }
                }
            }
        }
        Actions = dict;
    }

    public bool CheckIfShouldObserve()
    {
        float chance_to_observe, observe_time;
        if(_unit.UnitAI.Aggressiveness <= 0.5f)
        {
            chance_to_observe = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 0, 0.5f, 80, 40);
            observe_time = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 0, 0.5f, 5, 3f);
        }
        else if (_unit.UnitAI.Aggressiveness <= 1f)
        {
            chance_to_observe = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 0.5f, 1f, 40, 25);
            observe_time = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 0.5f, 1f, 3f, 2f);
        }
        else if (_unit.UnitAI.Aggressiveness <= 1.5f)
        {
            chance_to_observe = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 1f, 1.5f, 25, 15);
            observe_time = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 1f, 1.5f, 2f, 1.5f);
        }
        else
        {
            chance_to_observe = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 1.5f, 3f, 15, 0);
            observe_time = Utils.GetValueBasedOnMinAndMax(_unit.UnitAI.Aggressiveness, 1.5f, 3f, 1.5f, 1f);
        }
        int random = UnityEngine.Random.Range(1, 100);
        if(random <= chance_to_observe)
        {
            PerformAction(typeof(AI_Observe));
            WaitTimeBeforeNextAction = GetAdjustedWaitTime((int)(observe_time * 50));
            _unit.Animator.SetFloat("Observe Speed", 4f / observe_time);
            SkipObserve = true;
            return true;
        }
        return false;
    }

    public void PerformAction(Type action, Unit target = null) {
        CurrentAIBehavior = Constants.AIBehavior.UsingAbility;
        if (EnemiesInRangeForAbility.ContainsKey(action.ToString())) {
            _unit.CurrentTarget = EnemiesInRangeForAbility[action.ToString()];
        }
        _unit.Actions.UseAbility(action, false, target);
    }

    public void PerformAnAttack() {
        DecideOnNextAction(true);
    }
}