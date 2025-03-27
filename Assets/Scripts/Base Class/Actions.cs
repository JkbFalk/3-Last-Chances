using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Actions : MonoBehaviour {
    [HideInInspector]
    public LineOfSight LineOfSight;
    private UnitWeapon _heavyWeapon;
    private UnitWeapon _lightWeaponL;
    private UnitWeapon _lightWeaponR;
    private Vector2 _heavyWeaponCheckPoint = new Vector2(999, 999);
    private Vector2 _lightWeaponLCheckPoint = new Vector2(999, 999);
    private Vector2 _lightWeaponRCheckPoint = new Vector2(999, 999);
    private Constants.ActionType _currentActionBeingPerformed;

    public Constants.ActionType CurrentActionBeingPerformed {
        get => _currentActionBeingPerformed;
        set {
            if(Unit == null) {
                Start();
            }
            if(Unit.KnockedOut)
            {
                return;
            }
            Constants.ActionType previousAction = _currentActionBeingPerformed;

            Effect HardCrowdControlEffect = Unit.CurrentEffects.Where(effect => effect.IsHardCrowdControl()).FirstOrDefault();
            if (CurrentActionBeingPerformed != Constants.ActionType.UnderHardCrowdControl && value == Constants.ActionType.UnderHardCrowdControl)
            {
                EndCurrentAbility();
            }
            else if (HardCrowdControlEffect != null) {
                return;
            }
            string animation_names = Utils.GetCurrentAndNextAnimationName(Unit.Animator);
            Unit.Animator.SetInteger("Current Action", Utils.GetCodeForGivenAction(value));
            Constants.ActionType prevValue = _currentActionBeingPerformed;
            _currentActionBeingPerformed = value;
            if (QueuedInputs.Count > 0 && Utils.CheckIfUnitCanPerformActions(Unit)) {
                PerformQueuedActions();
            }
            else if (value == Constants.ActionType.Idle && CurrentAbilityBeingPerformed == null) {
                if (TryingToMoveInDirection.Count > 0) {
                    CurrentActionBeingPerformed = Constants.ActionType.Moving;
                }
                else {
                    Unit.PlayAnimation(Unit.InCombat ? "IdleInCombat" : "Idle", _previousAbilityBeingPerformed?.TransitionOutOfAnimationDuration != null ? _previousAbilityBeingPerformed.TransitionOutOfAnimationDuration : Constants.DEFAULT_CROSSFADE_DURATION);
                }
            }
            else if (value == Constants.ActionType.Moving && previousAction != Constants.ActionType.Moving && CurrentAbilityBeingPerformed == null && CanMove) {
                Unit.PlayAnimation(IsWalking || !Unit.CanRun ? "Walk" : "Run", _previousAbilityBeingPerformed?.TransitionOutOfAnimationDuration != null ? _previousAbilityBeingPerformed.TransitionOutOfAnimationDuration : 0.04f);
            }
            if(Unit is not Player && prevValue == Constants.ActionType.UnderHardCrowdControl && value != Constants.ActionType.UnderHardCrowdControl) {
                LineOfSight.HandleExitCrowdControl();
            }
            if (Utils.CheckIfUnitCanPerformActions(Unit)) {
                CheckIfFlipDirection();
            }
            if (DebugController.WorldspaceDebugEnabled) {
                Unit.WorldSpaceDebugAction.GetComponent<TextMeshProUGUI>().text = "Action: " + CurrentActionBeingPerformed.ToString();
            }
        }
    }

    private Ability _previousAbilityBeingPerformed;
    private Ability _currentAbilityBeingPerformed = null;

    public Ability CurrentAbilityBeingPerformed {
        get => _currentAbilityBeingPerformed;
        set {
            if (_currentAbilityBeingPerformed != null) {
                _currentAbilityBeingPerformed.OnAbilityEnd();
            }
            _previousAbilityBeingPerformed = _currentAbilityBeingPerformed;
            _currentAbilityBeingPerformed = value;
            if (_currentAbilityBeingPerformed != null) {
                CurrentActionBeingPerformed = Constants.ActionType.UsingAbility;
                if (_currentAbilityBeingPerformed.AutoPlayAbilityAnimation) {
                    Unit.PlayAnimation(String.IsNullOrEmpty(_currentAbilityBeingPerformed.NameOfAnimationToAutoPlay) ? _currentAbilityBeingPerformed.GetType().Name : _currentAbilityBeingPerformed.NameOfAnimationToAutoPlay, _currentAbilityBeingPerformed.TransitionIntoAnimationDuration);
                }
                _currentAbilityBeingPerformed.OnAbilityStart();
            }
            else {
                if(Unit is Player) {
                    PerformQueuedActions();
                }
                SavedAimDirection = Vector2.zero;
                CurrentActionBeingPerformed = Constants.ActionType.Idle;
            }
            if (DebugController.WorldspaceDebugEnabled) {
                Unit.WorldSpaceDebugAbility.GetComponent<TextMeshProUGUI>().text = "Ability: " + (_currentAbilityBeingPerformed != null ? _currentAbilityBeingPerformed.GetType().ToString() : "");
            }
        }
    }

    public void EndCurrentAbility() {
        if (CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed = null;
        }
    }

    public void DisableGameObject(GameObject game_object)
    {
        game_object.SetActive(false);
    }

    public List<QueuedInput> QueuedInputs = new List<QueuedInput>();

    [HideInInspector]
    public List<Constants.AttackType> CurrentBasicAttackCombo = new List<Constants.AttackType>();
    [HideInInspector]
    public Vector2 SavedAimDirection;
    public bool MovingToPoint = false;
    public Vector2 MovingToPointDestination;
    public bool CanMove
    {
        get
        {
            return Unit.MovementSpeed.Current > 0;
        }
    }
    public List<string> TryingToMoveInDirection { get; set; } = new List<string>();
    public bool IsWalking { get; set; }
    [HideInInspector]
    public Unit Unit;

    private Vector2 _previousPosition;

    public void Start() {
        Unit = GetComponent<Unit>();
        _heavyWeapon = Unit.SpriteRenderers.ContainsKey("Heavy") ? Unit.SpriteRenderers["Heavy"].Weapon : null;
        _lightWeaponL = Unit.SpriteRenderers.ContainsKey("Light Left") ? Unit.SpriteRenderers["Light Left"].Weapon : null;
        _lightWeaponR = Unit.SpriteRenderers.ContainsKey("Light Right") ? Unit.SpriteRenderers["Light Right"].Weapon : null;
        if(GetComponent<NavMeshAgent>() != null) {
            GetComponent<NavMeshAgent>().updateUpAxis = false;
            GetComponent<NavMeshAgent>().updateRotation = false;
        }
    }

    public void DestroyThisUnit() {
        MonoBehaviour.Destroy(gameObject);
    }


    private void FixedUpdate() {
        _previousAbilityBeingPerformed = null;
        if (Unit is Player) {
            DecrementQueuedInputTimers();
            if (TryingToMoveInDirection.Count > 0 &&
                (CurrentActionBeingPerformed == Constants.ActionType.Moving ||
                (CurrentActionBeingPerformed == Constants.ActionType.UsingAbility && CurrentAbilityBeingPerformed != null && CurrentAbilityBeingPerformed.CanMoveWhileUsing)))
            {
                CalculateMovement();
            }
        }
    }

    public void PushUnitForward(int force)
    {
        if(Unit == null) {
            Start();
        }
        Unit.ApplyForce(IsFlipped ? Vector2.left * force : Vector2.right * force, _currentAbilityBeingPerformed);
    }

    public void PushUnitForwardDuringRiposteOrCounter()
    {
        if(Unit.CurrentWeaponClass == Constants.WeaponClass.Daggers || Unit.CurrentWeaponClass == Constants.WeaponClass.Gauntlets) {
            Utils.PushUnitIntoPosition(Unit, CurrentAbilityBeingPerformed.Target.transform.position, CurrentAbilityBeingPerformed, 80);
        }
        else if(Unit.CurrentWeaponClass == Constants.WeaponClass.TwinBlades || Unit.CurrentWeaponClass == Constants.WeaponClass.Magic) {
            Utils.PushUnitIntoPosition(Unit, CurrentAbilityBeingPerformed.Target.transform.position, CurrentAbilityBeingPerformed, 60);
        }
        else if(Unit.CurrentWeaponClass == Constants.WeaponClass.Greatsword || Unit.CurrentWeaponClass == Constants.WeaponClass.Longblade) {
            Utils.PushUnitIntoPosition(Unit, CurrentAbilityBeingPerformed.Target.transform.position, CurrentAbilityBeingPerformed, 45);
        }
        else if(Unit.CurrentWeaponClass == Constants.WeaponClass.Polearm) {
            Utils.PushUnitIntoPosition(Unit, CurrentAbilityBeingPerformed.Target.transform.position, CurrentAbilityBeingPerformed, 30);
        }
        else {
            Utils.PushUnitIntoPosition(Unit, CurrentAbilityBeingPerformed.Target.transform.position, CurrentAbilityBeingPerformed, 10);
        }
    }

    public void PushUnitBackDuringRipostedOrCountered()
    {
        Unit.ApplyForce(IsFlipped ? Vector2.right * 200 : Vector2.left * 200, _currentAbilityBeingPerformed);
    }

    public void ConsumeEnergyAndCooldownForTheAbility()
    {
        if(CurrentAbilityBeingPerformed != null)
        {
            if(Player.Instance.PreparingForUltimate) {
                CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.Ultimate);
                SaveFile.Instance.UltimatesUsedInCurrentCombat++;
                Player.Instance.Energy.Current = 0;
                GameController.Instance.PlayerControls.StopPreparingUltimate();
            }
            CurrentAbilityBeingPerformed.AddOrUpdateCooldown();
            if (Unit is Player)
            {
                float cost = Ability.GetEnergyCost(CurrentAbilityBeingPerformed.GetType());
                if (cost > 0)
                {
                    Unit.Energy.Current -= cost;
                    EventManager.AbilityEnergyConsumed.Invoke(CurrentAbilityBeingPerformed, cost);
                }
            }
        }
    }

    public void TurnOnWeaponCollision(string weapon_category) {
        ChangeWeaponCollision(weapon_category, true);
        ChangeWeaponTrail(weapon_category, true);
    }
    public void TurnOffWeaponCollision(string weapon_category)
    {
        if(weapon_category == "Heavy") {
            _heavyWeaponCheckPoint = new Vector2(999, 999);;
        }
        if(weapon_category == "Light") {
            _lightWeaponLCheckPoint = new Vector2(999, 999);
            _lightWeaponRCheckPoint = new Vector2(999, 999);
        }
        ChangeWeaponCollision(weapon_category, false);
        ChangeWeaponTrail(weapon_category, false);
    }

    public void TurnOnWeaponTrail(string weapon_category)
    {
        if(weapon_category == "Light")
        {
            ChangeWeaponTrail("Light Left", true);
            ChangeWeaponTrail("Light Right", true);
        }
        else
        {
            ChangeWeaponTrail(weapon_category, true);
        }
    }
    public void TurnOffWeaponTrail(string weapon_category)
    {
        if (weapon_category == "Light")
        {
            ChangeWeaponTrail("Light Left", false);
            ChangeWeaponTrail("Light Right", false);
        }
        else
        {
            ChangeWeaponTrail(weapon_category, false);
        }
    }

    public void ChangeWeaponCollision(string weapon_category, bool deal_damage)
    {
        if (_currentAbilityBeingPerformed == null || String.IsNullOrEmpty(weapon_category) || (weapon_category != "Light" && !Unit.SpriteRenderers.ContainsKey(weapon_category)))
        {
            return;
        }
        if (weapon_category == "Heavy" || weapon_category == "Ranged" || weapon_category == "Projectile")
        {
            Unit.SpriteRenderers[weapon_category].Weapon.ChangeWeaponDealingDamage(deal_damage, _currentAbilityBeingPerformed);
        }
        else if (weapon_category == "Light")
        {
            if(Unit.SpriteRenderers.ContainsKey("Light Right")) {
                Unit.SpriteRenderers["Light Right"].Weapon.ChangeWeaponDealingDamage(deal_damage, _currentAbilityBeingPerformed);
            }
            if(Unit.SpriteRenderers.ContainsKey("Light Left")) {
                Unit.SpriteRenderers["Light Left"].Weapon.ChangeWeaponDealingDamage(deal_damage, _currentAbilityBeingPerformed);
            }
        }
        if(deal_damage && _currentAbilityBeingPerformed.TurningOnCollisionClearsAffectedEnemyList)
        {
            _currentAbilityBeingPerformed.ResetPotentialTargets();
        }
    }

    public bool CheckIfWeaponDealingDamage(Constants.DamageType type) {
        switch(type) {
            case Constants.DamageType.Heavy: return Unit.SpriteRenderers["Heavy"].Weapon.DealingDamage;
            case Constants.DamageType.Light: return Unit.SpriteRenderers["Light Right"].Weapon.DealingDamage || Unit.SpriteRenderers["Light Left"].Weapon.DealingDamage;
            case Constants.DamageType.Ranged: return Unit.SpriteRenderers["Ranged"].Weapon.DealingDamage || Unit.SpriteRenderers["Projectile"].Weapon.DealingDamage;
            default: return false;
        }
    }

    public void ChangeWeaponTrail(string weapon_category, bool show_trail)
    {
        if (_currentAbilityBeingPerformed == null || String.IsNullOrEmpty(weapon_category) || !Unit.SpriteRenderers.ContainsKey(weapon_category) ||Unit.SpriteRenderers[weapon_category] == null ||Unit.SpriteRenderers[weapon_category]?.Weapon?.WeaponTrail == null)
        {
            return;
        }
        if (weapon_category == "Heavy" || weapon_category == "Ranged")
        {
            ParticleSystem system = Unit.SpriteRenderers[weapon_category]?.Weapon?.WeaponTrail.GetComponent<ParticleSystem>();
            if (show_trail)
            {
                system.Play();
            }
            else
            {
                system.Stop();
            }
        }
        else if(weapon_category == "Light")
        {
            ParticleSystem system1 = Unit.SpriteRenderers["Light Left"]?.Weapon?.WeaponTrail.GetComponent<ParticleSystem>();
            ParticleSystem system2 = Unit.SpriteRenderers["Light Right"]?.Weapon?.WeaponTrail.GetComponent<ParticleSystem>();
            if (show_trail)
            {
                system1.Play();
                system2.Play();
            }
            else
            {
                system1.Stop();
                system2.Stop();
            }
        }
    }

    public void DisplayDangerSign() {
        if (_currentAbilityBeingPerformed != null && _currentAbilityBeingPerformed.CheckIfShouldShowDangerSign() && Unit.WorldSpaceCanvas != null) {
            Transform leftover_danger_sign = Unit.WorldSpaceCanvas.transform.Find("UI_DangerSign");
            if (leftover_danger_sign != null) {
                MonoBehaviour.Destroy(leftover_danger_sign.gameObject);
            }
            Transform leftover_danger_sign2 = Unit.WorldSpaceCanvas.transform.Find("UI_ExtremeDangerSign");
            if (leftover_danger_sign2 != null) {
                MonoBehaviour.Destroy(leftover_danger_sign2.gameObject);
            }
            GameObject danger_sign = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_DangerSign")) as GameObject;
            danger_sign.gameObject.name = "UI_DangerSign";
            danger_sign.transform.SetParent(Unit.WorldSpaceCanvas.transform, false);
            danger_sign.transform.localPosition = new Vector2(0, 0);
            danger_sign.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(0.35f);
            Utils.PlaySoundEffect(Unit.AudioSource, "Generic/Generic_DangerSign", 1f);
            GameController.Instance.WaitAndRunMethod(1, HideDangerSign);
        }
    }

    public void DisplayExtremeDangerSign() {
        if (_currentAbilityBeingPerformed != null) {
            Transform leftover_danger_sign = Unit.WorldSpaceCanvas.transform.Find("UI_DangerSign");
            if (leftover_danger_sign != null) {
                MonoBehaviour.Destroy(leftover_danger_sign.gameObject);
            }
            Transform leftover_danger_sign2 = Unit.WorldSpaceCanvas.transform.Find("UI_ExtremeDangerSign");
            if (leftover_danger_sign2 != null) {
                MonoBehaviour.Destroy(leftover_danger_sign2.gameObject);
            }
            _currentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.Unstoppable);
            GameObject danger_sign = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ExtremeDangerSign")) as GameObject;
            danger_sign.gameObject.name = "UI_ExtremeDangerSign";
            danger_sign.transform.SetParent(Unit.WorldSpaceCanvas.transform, false);
            danger_sign.transform.localPosition = new Vector2(0, 0);
            danger_sign.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(0.35f);
            Utils.PlaySoundEffect(Unit.AudioSource, "Generic/Generic_ExtremeDangerSign", 1f);
            GameController.Instance.WaitAndRunMethod(1, HideExtremeDangerSign);
        }
    }

    public void HideDangerSign() {
        Transform danger_sign = Unit.WorldSpaceCanvas.transform.Find("UI_DangerSign");
        if(danger_sign != null)
        {
            danger_sign.GetComponent<HideOrShowOverTime>().HideOverTimeFromFull(0.2f);
            danger_sign.GetComponent<HideOrShowOverTime>().DestroyAfterHiding = true;
        }
    }

    public void HideExtremeDangerSign() {
        Transform danger_sign = Unit.WorldSpaceCanvas.transform.Find("UI_ExtremeDangerSign");
        if(danger_sign != null)
        {
            danger_sign.GetComponent<HideOrShowOverTime>().HideOverTimeFromFull(0.2f);
            danger_sign.GetComponent<HideOrShowOverTime>().DestroyAfterHiding = true;
        }
    }

    public void PlaySwingSound() {
        Utils.PlaySoundEffect(Unit.AudioSource, "Swing/" + Utils.DetermineSwingSoundBasedOnAbilityWeaponClass(Unit.CurrentWeaponClass) + "_Swing" + UnityEngine.Random.Range(1, 7), 1.1f);
    }

    public void PlayHeavySwingSound() {
        Utils.PlaySoundEffect(Unit.AudioSource, "HeavySwing/" + Utils.DetermineSwingSoundBasedOnAbilityWeaponClass(Unit.CurrentWeaponClass) + "_HeavySwing" + UnityEngine.Random.Range(1, 4), 1.3f);
    }

    public void PerformQueuedActions() {
        for (int i = QueuedInputs.Count - 1; i >= 0; i--) {
            if (QueuedInputs[i].ButtonWasReleased && CurrentAbilityBeingPerformed != null) {
                CurrentAbilityBeingPerformed.OnAbilityButtonRelease();
            }
            if(QueuedInputs[i].MethodName == "UseAbility" && Ability.CheckIfCanPerformAbility(Player.Instance, QueuedInputs[i].AbilityToPerform, QueuedInputs[i].ItemToUse)) {
                UseAbility(QueuedInputs[i].AbilityToPerform, false, null, QueuedInputs[i].ItemToUse);
                QueuedInputs.Clear();
                break;
            }
            else if(QueuedInputs[i].MethodName == "PerformBasicAttack") {
                PerformBasicAttack();
                QueuedInputs.Clear();
                break;
            }
            else if(QueuedInputs[i].MethodName == "PerformQueuedDodge" && 
            (CurrentAbilityBeingPerformed == null || 
            CurrentAbilityBeingPerformed.CanInterruptCurrentAbility ||
            CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(Ability.AbilityInterruptType.Dodge))) {
                PerformQueuedDodge(QueuedInputs[i]);
                QueuedInputs.Clear();
                break;
            }
        }
    }

    public void SetCurrentAbilityInterruptable() {
        if (_currentAbilityBeingPerformed != null) {
            _currentAbilityBeingPerformed.CanInterruptCurrentAbility = true;
        }
    }

    public void SetCurrentAbilityNotInterruptable() {
        if (_currentAbilityBeingPerformed != null) {
            _currentAbilityBeingPerformed.CanInterruptCurrentAbility = false;
        }
    }

    public void AddCounterMethod(string name)
    {
        if(CurrentAbilityBeingPerformed == null)
        {
            return;
        }
        if(name == "Backstep")
        {
            CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.CounteredByBackstep);
        }
        else if (name == "Roll")
        {
            CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.CounteredByRoll);
        }
        else if (name == "Riposte")
        {
            CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.CounteredByRiposte);
        }
        else if (name == "Block")
        {
            CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.CounteredByBlock);
        }
    }

    public void ChanceToBlink(int chance) {
        int random = UnityEngine.Random.Range(0, 100);
        if(random < chance) {
            GameController.Instance.WaitAndRunMethod(0.1f, SetFaceVariantCoroutine, new string[] {"Eyes Squinted"});
            GameController.Instance.WaitAndRunMethod(0.2f, SetFaceVariantCoroutine, new string[] {"Eyes Closed"});
            GameController.Instance.WaitAndRunMethod(0.3f, SetFaceVariantCoroutine, new string[] {"Eyes Squinted"});
            GameController.Instance.WaitAndRunMethod(0.4f, SetFaceVariantCoroutine, new string[] {"Regular"});
        }
    }

    public void SetFaceVariantCoroutine(string[] string_params) {
        SetFaceVariant(string_params[0]);
    }

    public void RemoveCounterMethod(string name)
    {
        if (CurrentAbilityBeingPerformed == null)
        {
            return;
        }
        if (name == "Backstep")
        {
            CurrentAbilityBeingPerformed.Properties.Remove(Ability.AbilityProperty.CounteredByBackstep);
        }
        else if (name == "Roll")
        {
            CurrentAbilityBeingPerformed.Properties.Remove(Ability.AbilityProperty.CounteredByRoll);
        }
        else if (name == "Riposte")
        {
            CurrentAbilityBeingPerformed.Properties.Remove(Ability.AbilityProperty.CounteredByRiposte);
        }
        else if (name == "Block")
        {
            CurrentAbilityBeingPerformed.Properties.Remove(Ability.AbilityProperty.CounteredByBlock);
        }
    }

    private void DecrementQueuedInputTimers() {
        List<QueuedInput> filteredQueuedInputs = new List<QueuedInput>();
        foreach (QueuedInput queuedInput in QueuedInputs) {
            if (queuedInput.RemainingTime > 1) {
                queuedInput.RemainingTime--;
                filteredQueuedInputs.Add(queuedInput);
            }
        }
        QueuedInputs = filteredQueuedInputs;
    }

    public Ability_Dodge GetDodgeTypeThatShouldBeUsed() {
        if (TryingToMoveInDirection.Count > 0) {
            if (TryingToMoveInDirection.Count == 1 && TryingToMoveInDirection[0] == "Up") {
                return new Ability_Roll(Unit) {Direction = Vector2.up};
            }
            else if (TryingToMoveInDirection.Count == 1 && TryingToMoveInDirection[0] == "Down") {
                return new Ability_Roll(Unit) {Direction = Vector2.down};
            }
            else {
                return new Ability_Roll(Unit)  {Direction = IsFlipped ? Vector2.left : Vector2.right};
            }
        }
        else {
            return new Ability_BackStep(Unit);
        }
    }

    public void FaceCurrentTarget() {
        if(Unit.CurrentTarget != null) {
            IsFlipped = Unit.CurrentTarget.transform.position.x < Unit.transform.position.x;
        }
    }

    public void PerformQueuedDodge(QueuedInput dodge) {
        Ability_Dodge dodge_instance = (Ability_Dodge)Activator.CreateInstance(dodge.AbilityToPerform, new object[] { Unit });
        dodge_instance.Direction = dodge.DodgeDirection;
        Player.Instance.Actions.CurrentAbilityBeingPerformed = dodge_instance;
    }

    public void SetWeaponCollisionName(string name)
    {
        if(CurrentAbilityBeingPerformed != null)
        {
            CurrentAbilityBeingPerformed.WeaponCollisionName = name;
        }
    }

    public void PerformBasicAttack() {
        if (Player.Instance.UnitsInRangeForBackstab.Count > 0 && Player.Instance.UnitsInRangeForBackstab[0] != null && Player.Instance.UnitsInRangeForBackstab[0].KnockedOut == false && ((Player.Instance.transform.position.x > Player.Instance.UnitsInRangeForBackstab[0].transform.position.x && Player.Instance.Actions.IsFlipped && Player.Instance.UnitsInRangeForBackstab[0].Actions.IsFlipped) ||
            (Player.Instance.transform.position.x < Player.Instance.UnitsInRangeForBackstab[0].transform.position.x && !Player.Instance.Actions.IsFlipped && !Player.Instance.UnitsInRangeForBackstab[0].Actions.IsFlipped)))
        {
            PerformBackstab();
        }
        else {
            PerformRegularBasicAttack();
        }
    }


    public void PerformBackstab()
    {
        Unit closest_unit = Player.Instance.UnitsInRangeForBackstab[0];
        foreach (Unit unit in Player.Instance.UnitsInRangeForBackstab)
        {
            if (closest_unit != null && unit != null && !unit.KnockedOut && Vector2.Distance(Player.Instance.transform.position, unit.transform.position) < Vector2.Distance(Player.Instance.transform.position, closest_unit.transform.position))
            {
                closest_unit = unit;
            }
        }
        if (closest_unit == null || closest_unit.CheckIfUnderEffect(typeof(Effect_ImmunityToBackstabs)))
        {
            PerformRegularBasicAttack();
        }
        else
        {
            Type attackType = Type.GetType("BA_" + Unit.CurrentWeaponClass + "_Backstab");
            Ability backstab = (Backstab)Activator.CreateInstance(attackType, new object[] { Unit });
            if (Ability.CheckIfCanPerformAbility(Player.Instance, attackType) && (CurrentAbilityBeingPerformed == null && Utils.CheckIfUnitCanPerformActions(Unit)) || (CurrentAbilityBeingPerformed != null && CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(Ability.AbilityInterruptType.BasicAttack)))
            {
                Unit.Actions.CurrentAbilityBeingPerformed = backstab;
                backstab.Target = closest_unit;
                closest_unit.AddEffect(new Effect_Backstabbed(new(backstab)), 3.5f);
                closest_unit.AddEffect(new Effect_ImmunityToBackstabs(new(backstab)), Player.Instance.BackstabCooldown);
            }
        }
    }

    public void PerformRegularBasicAttack()
    {
        Type basicAttackType;
        if(Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_PowerWithoutLimit)) {
            basicAttackType = typeof(BA_MagicRanged_F);
        }
        else if(Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_MindOverMatter)) {
            basicAttackType = typeof(BA_MagicMelee_F);
        }
        else {
            basicAttackType = Type.GetType("BA_" + Unit.CurrentWeaponClass + "_F");
        }
        BasicAttack basicAttack = (BasicAttack)Activator.CreateInstance(basicAttackType, new object[] { Unit });
        if (Ability.CheckIfCanPerformAbility(Player.Instance, basicAttackType))
        {
            Unit.Actions.CurrentAbilityBeingPerformed = basicAttack;
        }
        else if (CurrentAbilityBeingPerformed == null || (CurrentAbilityBeingPerformed != null && CurrentAbilityBeingPerformed.IsNot(Ability.AbilityProperty.BasicAttack)))
        {
            QueuedInputs.Add(new QueuedInput("PerformBasicAttack", basicAttackType, null, 10));
        }
    }

    public void SetItemSprite(string item_path) {
        if(Unit?.SpriteRenderers != null && Unit.SpriteRenderers.ContainsKey("Consumable")) {
            Unit.SpriteRenderers["Consumable"].SpriteRenderer.enabled = true;
            Constants.ItemCategory category = Constants.ItemCategory.Quest;
            Enum.TryParse(item_path.Split("/")[0], out category);
            Utils.CopyItemAppearanceForPlayer(category, item_path.Split("/")[1]);
        }
    }

    public void EnableConsumable() {
        if(Unit?.SpriteRenderers != null && Unit.SpriteRenderers.ContainsKey("Consumable")) {
            Unit.SpriteRenderers["Consumable"].SpriteRenderer.enabled = true;
        }
    }

    public void DisableConsumable() {
        if(Unit?.SpriteRenderers != null && Unit.SpriteRenderers.ContainsKey("Consumable")) {
            Unit.SpriteRenderers["Consumable"].SpriteRenderer.enabled = false;
        }
    }
    
    public void MoveInYAxis(float amount) {
        gameObject.transform.Translate(new Vector2(0, amount));
    }

    public void MoveInXAxis(float amount) {
        gameObject.transform.Translate(new Vector2(amount, 0));
    }

    public void StartWalking() {
        IsWalking = true;
        if (CurrentActionBeingPerformed == Constants.ActionType.Moving) {
            Unit.PlayAnimation("Walk");
        }
    }

    public void StopWalking() {
        IsWalking = false;
    }

    public void PlayAbilityCustomSound(string sound_name) {
        if (CurrentAbilityBeingPerformed != null && CurrentAbilityBeingPerformed.CustomSounds.ContainsKey(sound_name) && (Unit.Animator.IsInTransition(0) == false || CurrentAbilityBeingPerformed.IsNot(Ability.AbilityProperty.BasicAttack))) {
            CurrentAbilityBeingPerformed.PlayCustomSound(sound_name);
        }
    }

    public void PlaySoundEffect(string sound_name) {
        Utils.PlaySoundEffect(Unit.AudioSource, sound_name, 0.6f);
    }

    public void AddDirection(string direction) {
        TryingToMoveInDirection.Remove(direction);
        TryingToMoveInDirection.Add(direction);
        if(CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.AdditionalAbilitySpecificActionsOnTryingToMove();
        }
        if (CurrentAbilityBeingPerformed is Ability_Block && !Utils.GetCurrentAndNextAnimationName(Unit.Animator).Contains("BlockMove")) {
            Unit.PlayAnimation(Unit.CurrentWeaponClass + "_BlockMove", CurrentAbilityBeingPerformed.TransitionIntoAnimationDuration);
        }
        if (Utils.CheckIfUnitCanPerformActions(Unit)) {
            CheckIfFlipDirection();
            CurrentActionBeingPerformed = Constants.ActionType.Moving;
        }
        Unit.Animator.SetBool("Moving", TryingToMoveInDirection.Count > 0);
    }

    public void RemoveDirection(string direction) {
        TryingToMoveInDirection.Remove(direction);
        if(CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.AdditionalAbilitySpecificActionsOnTryingToMove();
        }
        if (TryingToMoveInDirection.Count == 0 && CurrentActionBeingPerformed == Constants.ActionType.Moving) {
            CurrentActionBeingPerformed = Constants.ActionType.Idle;
        }
        if (CurrentAbilityBeingPerformed is Ability_Block && TryingToMoveInDirection.Count == 0) {
            Unit.PlayAnimation(Unit.CurrentWeaponClass + "_Block", CurrentAbilityBeingPerformed.TransitionIntoAnimationDuration);
        }
        if (Utils.CheckIfUnitCanPerformActions(Unit)) {
            CheckIfFlipDirection();
        }
        Unit.Animator.SetBool("Moving", TryingToMoveInDirection.Count > 0);
    }

    public void PlayCustomAnimationSound(string name) {
        if(name == "TurnBookPage") {
            Utils.PlaySoundEffect(Unit.AudioSource, "Dialogue/TurnBookPage" + Utils.GetRandomSoundNumber("TurnBookPage"), 0.4f);
        }
        else if(name == "ShatterEnergyCore") {
            Utils.PlaySoundEffect(Unit.AudioSource, "Dialogue/ShatterEnergyCore", 0.9f);
        }
    }

    public void PlayFootstepsSound() {
        GameObject[] tilemaps = GameObject.FindGameObjectsWithTag("Tilemap");
        int highestPriority = -1;
        Area.FootstepsType footstepsType = Area.FootstepsType.None;
        foreach(Tilemap tilemap in Area.ComponentInstance.TilemapsWithFootstepOverrides.Keys) {
            TileBase tile = tilemap.GetTile(tilemap.WorldToCell(transform.position));
            if(tile != null && Area.ComponentInstance.TilemapsWithFootstepOverrides[tilemap].Priority > highestPriority) {
                highestPriority = Area.ComponentInstance.TilemapsWithFootstepOverrides[tilemap].Priority;
                footstepsType = Area.ComponentInstance.TilemapsWithFootstepOverrides[tilemap].Footsteps;
            }
        }
        Utils.PlaySoundEffect(Unit.AudioSource, "Footsteps/Footsteps_" + (footstepsType == Area.FootstepsType.None ? Area.ComponentInstance.Footsteps : footstepsType) + Utils.GetRandomSoundNumber("Footsteps_" + (footstepsType == Area.FootstepsType.None ? Area.ComponentInstance.Footsteps : footstepsType) ), Unit is Player ? 0.1f : 0.07f);
        if((footstepsType == Area.FootstepsType.None ? Area.ComponentInstance.Footsteps : footstepsType).ToString().Contains("Water") && Unit.SpriteRenderers["Right Foot"].SpriteRenderer.enabled) {
            GameObject vfx = Utils.CreateVisualEffect(new(Player.Instance), "WaterSplash1", transform.position.x + (IsFlipped ? -0.15f : 0.15f), transform.position.y - 1.3f);
            vfx.transform.SetParent(Unit.transform);
        }
        else if((footstepsType == Area.FootstepsType.None ? Area.ComponentInstance.Footsteps : footstepsType).ToString().Contains("Lava") && Unit.SpriteRenderers["Right Foot"].SpriteRenderer.enabled) {
            GameObject vfx = Utils.CreateVisualEffect(new(Player.Instance), "LavaSplash1", transform.position.x + (IsFlipped ? -0.15f : 0.15f), transform.position.y - 1.3f);
            vfx.transform.SetParent(Unit.transform);
        }
    }

    public void FaceUnit(Unit target)
    {
        if(target != null && Unit != null && target.transform.position.x > Unit.transform.position.x)
        {
            IsFlipped = false;
        }
        else if (target != null && Unit != null && target.transform.position.x < Unit.transform.position.x)
        {
            IsFlipped = true;
        }
    }

    public void PlayOtherCrowdControlAnimations()
    {
        Effect HardCrowdControlEffect = Unit.CurrentEffects.Where(effect => effect.IsHardCrowdControl()).FirstOrDefault();
        if(HardCrowdControlEffect != null)
        {
            Unit.PlayAnimation(HardCrowdControlEffect.GetType().Name);
        }
    } 

    private void CalculateMovement() {
        bool isBlocking = Unit.CheckIfUnderEffect(typeof(Effect_Block));
        Vector3 primary_vector = GetDirectionVector(TryingToMoveInDirection[0]);
        Vector3 main_move_direction = (IsWalking ? Constants.PLAYER_WALK_SPEED : isBlocking ? Constants.PLAYER_BLOCK_MOVE_SPEED : Constants.PLAYER_RUN_SPEED) * Unit.MovementSpeed.Current * primary_vector * Time.fixedDeltaTime;
        if (TryingToMoveInDirection.Count > 1 && !Utils.GetAreOppositeDirections(TryingToMoveInDirection[0], TryingToMoveInDirection[1])) {
            Vector3 secondaryVector = GetDirectionVector(TryingToMoveInDirection[1]) * (IsWalking ? Constants.PLAYER_WALK_SPEED : isBlocking ? Constants.PLAYER_BLOCK_MOVE_SPEED : Constants.PLAYER_RUN_SPEED) * Unit.MovementSpeed.Current * Time.fixedDeltaTime;
            Unit.Rigidbody2D.MovePosition(Unit.transform.position + main_move_direction + secondaryVector);
        }
        else {
            Unit.Rigidbody2D.MovePosition(Unit.transform.position + main_move_direction);
        }
    }

    private Vector3 GetDirectionVector(string direction) {
        return direction switch {
            "Up" => Vector3.up,
            "Right" => Vector3.right,
            "Down" => Vector3.down,
            "Left" => Vector3.left,
            _ => Vector3.zero,
        };
    }

    private void ConsiderStoppingCurrentAction(int chance_to_stop) {
        float random = Random.Range(1, 100);
        if (random <= chance_to_stop && _currentAbilityBeingPerformed != null) {
            EndCurrentAbility();
        }
    }

    public void Update() {
        if(_heavyWeapon != null && _heavyWeapon.WeaponHitbox != null && _heavyWeapon.DealingDamage && _heavyWeaponCheckPoint == new Vector2(999, 999)) {
            _heavyWeaponCheckPoint = new Vector2(_heavyWeapon.WeaponHitbox.transform.position.x - _heavyWeapon.WeaponHitbox.offset.x - (_heavyWeapon.WeaponHitbox.size.x / 2) - 0.1f, _heavyWeapon.WeaponHitbox.transform.position.y - _heavyWeapon.WeaponHitbox.offset.y - (_heavyWeapon.WeaponHitbox.size.y / 2) - 0.1f);
        }
        else if(_heavyWeapon != null && _heavyWeapon.WeaponHitbox != null && _heavyWeapon.DealingDamage && _heavyWeaponCheckPoint != new Vector2(999, 999)) {
            Vector2 secondCheckPoint = new Vector2(_heavyWeapon.WeaponHitbox.transform.position.x + _heavyWeapon.WeaponHitbox.offset.x + (_heavyWeapon.WeaponHitbox.size.x / 2) + 0.1f, _heavyWeapon.WeaponHitbox.transform.position.y + _heavyWeapon.WeaponHitbox.offset.y + (_heavyWeapon.WeaponHitbox.size.y / 2)+ 0.1f);
            List<Collider2D> hits = new();
            Physics2D.OverlapArea(_heavyWeaponCheckPoint, secondCheckPoint, new ContactFilter2D() {layerMask = LayerMask.GetMask("Environment") | LayerMask.GetMask("Units") | LayerMask.GetMask("Projectiles")}, hits);
            _heavyWeaponCheckPoint = new Vector2(_heavyWeapon.WeaponHitbox.transform.position.x - _heavyWeapon.WeaponHitbox.offset.x - (_heavyWeapon.WeaponHitbox.size.x / 2) - 0.1f, _heavyWeapon.WeaponHitbox.transform.position.y - _heavyWeapon.WeaponHitbox.offset.y - (_heavyWeapon.WeaponHitbox.size.y / 2) - 0.1f);
            foreach(Collider2D hit in hits) {
                _heavyWeapon.HandleCollision(hit);
            }
        }
        if(_lightWeaponL != null && _lightWeaponL.WeaponHitbox != null && _lightWeaponL.DealingDamage && _lightWeaponLCheckPoint == new Vector2(999, 999)) {
            _lightWeaponLCheckPoint = new Vector2(_lightWeaponL.WeaponHitbox.transform.position.x - _lightWeaponL.WeaponHitbox.offset.x - (_lightWeaponL.WeaponHitbox.size.x / 2) - 0.05f, _lightWeaponL.WeaponHitbox.transform.position.y - _lightWeaponL.WeaponHitbox.offset.y - (_lightWeaponL.WeaponHitbox.size.y / 2) - 0.05f);
        }
        else if(_lightWeaponL != null && _lightWeaponL.WeaponHitbox != null &&_lightWeaponL.DealingDamage && _lightWeaponLCheckPoint != new Vector2(999, 999)) {
            Vector2 secondCheckPoint = new Vector2(_lightWeaponL.WeaponHitbox.transform.position.x + _lightWeaponL.WeaponHitbox.offset.x + (_lightWeaponL.WeaponHitbox.size.x / 2) + 0.05f, _lightWeaponL.WeaponHitbox.transform.position.y + _lightWeaponL.WeaponHitbox.offset.y + (_lightWeaponL.WeaponHitbox.size.y / 2)+ 0.05f);
            List<Collider2D> hits = new();
            Physics2D.OverlapArea(_lightWeaponLCheckPoint, secondCheckPoint, new ContactFilter2D() {layerMask = LayerMask.GetMask("Environment") | LayerMask.GetMask("Units") | LayerMask.GetMask("Projectiles")}, hits);
            _lightWeaponLCheckPoint = new Vector2(_lightWeaponL.WeaponHitbox.transform.position.x - _lightWeaponL.WeaponHitbox.offset.x - (_lightWeaponL.WeaponHitbox.size.x / 2) - 0.05f, _lightWeaponL.WeaponHitbox.transform.position.y - _lightWeaponL.WeaponHitbox.offset.y - (_lightWeaponL.WeaponHitbox.size.y / 2) - 0.05f);
            foreach(Collider2D hit in hits) {
                _lightWeaponL.HandleCollision(hit);
            }
        }
        if(_lightWeaponR != null && _lightWeaponR.WeaponHitbox != null && _lightWeaponR.DealingDamage && _lightWeaponRCheckPoint == new Vector2(999, 999)) {
            _lightWeaponRCheckPoint = new Vector2(_lightWeaponR.WeaponHitbox.transform.position.x - _lightWeaponR.WeaponHitbox.offset.x - (_lightWeaponR.WeaponHitbox.size.x / 2) - 0.05f, _lightWeaponR.WeaponHitbox.transform.position.y - _lightWeaponR.WeaponHitbox.offset.y - (_lightWeaponR.WeaponHitbox.size.y / 2) - 0.05f);
        }
        else if(_lightWeaponR != null && _lightWeaponR.WeaponHitbox != null && _lightWeaponR.DealingDamage && _lightWeaponRCheckPoint != new Vector2(999, 999)) {
            Vector2 secondCheckPoint = new Vector2(_lightWeaponR.WeaponHitbox.transform.position.x + _lightWeaponR.WeaponHitbox.offset.x + (_lightWeaponR.WeaponHitbox.size.x / 2) + 0.05f, _lightWeaponR.WeaponHitbox.transform.position.y + _lightWeaponR.WeaponHitbox.offset.y + (_lightWeaponR.WeaponHitbox.size.y / 2)+ 0.05f);
            List<Collider2D> hits = new();
            Physics2D.OverlapArea(_lightWeaponRCheckPoint, secondCheckPoint, new ContactFilter2D() {layerMask = LayerMask.GetMask("Environment") | LayerMask.GetMask("Units") | LayerMask.GetMask("Projectiles")}, hits);
            _lightWeaponRCheckPoint = new Vector2(_lightWeaponR.WeaponHitbox.transform.position.x - _lightWeaponR.WeaponHitbox.offset.x - (_lightWeaponR.WeaponHitbox.size.x / 2) - 0.05f, _lightWeaponR.WeaponHitbox.transform.position.y - _lightWeaponR.WeaponHitbox.offset.y - (_lightWeaponR.WeaponHitbox.size.y / 2) - 0.05f);
            foreach(Collider2D hit in hits) {
                _lightWeaponR.HandleCollision(hit);
            }
        }
    }

    public void FaceDirectionOfMovement() {
        Vector2 CurrentPosition = transform.localPosition;
        if (_previousPosition != null) {
            float distance_x = CurrentPosition.x - _previousPosition.x;
            float distance_y = CurrentPosition.y - _previousPosition.y;
            if (Math.Abs(distance_x) == 0 && Math.Abs(distance_y) == 0) {
                CurrentActionBeingPerformed = Constants.ActionType.Idle;
            }
            else if (distance_x > 0.01f && IsFlipped) {
                CurrentActionBeingPerformed = Constants.ActionType.Moving;
                IsFlipped = false;
            }
            else if (distance_x < -0.01f && !IsFlipped) {
                CurrentActionBeingPerformed = Constants.ActionType.Moving;
                IsFlipped = true;
            }
        }
        _previousPosition = CurrentPosition;
    }

    public void UseAbility(Type ability_to_use, bool allow_to_queue_ability = true, Unit target = null, Item item_to_use = null) {
        if(ability_to_use != null && ability_to_use.GetField("CanBeUsedDuringOtherAbilities",  BindingFlags.Public | BindingFlags.Static) != null && !(ability_to_use == typeof(Ability_CuttingWind) && Player.Instance.PreparingForUltimate)) {
            Ability instantCast = (Ability)Activator.CreateInstance(ability_to_use, new object[] { Unit });
            instantCast.ActionsToPerformDuringAnotherAbility();
        }
        else if (Ability.CheckIfCanPerformAbility(Unit, ability_to_use, item_to_use)) {
            CurrentAbilityBeingPerformed = (Ability)Activator.CreateInstance(ability_to_use, ability_to_use.IsSubclassOf(typeof(AI)) ? new object[] { Unit, target } : item_to_use != null ? new object[] { Unit, item_to_use } : new object[] { Unit });
        }
        else if (allow_to_queue_ability && Ability.CheckIfEnoughResourceToUseAbility(Player.Instance, ability_to_use)) {
            QueuedInputs.Add(new QueuedInput("UseAbility", ability_to_use, item_to_use));
        }
    }

    public void CleanseAllHardCrowdControl() {
        foreach(Effect e in Unit.CurrentEffects.ToList()) {
            if(e.IsHardCrowdControl()) {
                e.EndThisEffect();
            }
        }
        CurrentActionBeingPerformed = Constants.ActionType.Idle;
    }

    public void ReleaseAbilityButton(Ability ability) {
        if (CurrentAbilityBeingPerformed != null && CurrentAbilityBeingPerformed.GetType() == ability.GetType()) {
            CurrentAbilityBeingPerformed.OnAbilityButtonRelease();
        }
        else {
            if (QueuedInputs.Count > 0) {
                foreach (QueuedInput input in QueuedInputs) {
                    if (input.AbilityToPerform.GetType() == ability.GetType()) {
                        input.ButtonWasReleased = true;
                    }
                }
            }
        }
    }

    public virtual void CallAbilityEvent1() {
        if (CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.CallAbilityEvent1();
        }
    }

    public virtual void CallAbilityEvent2() {
        if (CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.CallAbilityEvent2();
        }
    }

    public virtual void CallAbilityEvent3() {
        if (CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.CallAbilityEvent3();
        }
    }

    public virtual void CallAbilityEvent4() {
        if (CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.CallAbilityEvent4();
        }
    }

    public Vector2 GetCurrentAimVector() {
        if (Player.Instance.CurrentTarget == null) {
            if (GameController.Instance.PlayerInput.currentControlScheme == "Mouse and Keyboard") {
                Vector3 target_vector = GameController.Instance.PlayerControls.CurrentWorldspacePointerPosition;
                return (target_vector - Player.Instance.ProjectileSpawnLocation.transform.position).normalized;
            }
            else if(GameController.Instance.PlayerInput.currentControlScheme == "Gamepad") {
                Vector3 target_vector = GameController.Instance.PlayerControls.CurrentLeftStickPosition;
                if(target_vector == Vector3.zero)
                {
                    target_vector = Player.Instance.Actions.IsFlipped ? Vector3.left : Vector3.right;
                }
                return target_vector;
            }
            return Vector2.zero;
        }
        else {
            return (Player.Instance.CurrentTarget.transform.position - Player.Instance.ProjectileSpawnLocation.transform.position).normalized;
        }
    }
    [HideInInspector]
    public bool IsWalkingToPoint = false;

    public void MoveToPoint(float x, float y, bool walk = false) {
        Unit.GetComponent<NavMeshAgent>().enabled = true;
        if(Unit.GetComponent<NavMeshAgent>() != null && Unit.UnitAI.NavMeshAgent.isOnNavMesh) {
            if(walk) {
                Unit.CanRun = false;
                IsWalkingToPoint = true;
            }
            IsFlipped = x < transform.position.x;
            MovingToPoint = true;
            MovingToPointDestination = new Vector2(x, y);
            Unit.PlayAnimation(walk ? "Walk" : "Run");
            Unit.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
            Unit.GetComponent<NavMeshAgent>().SetDestination(new Vector2(x, y));
            Unit.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Repositioning;
            Unit.UnitAI.CurrentDirectionType = UnitAI.DirectionType.FaceDirectionOfCurrentMovement;
        }
        else {
            Unit.GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    public void StopMovingToPoint() {
        if(Unit.GetComponent<NavMeshAgent>() != null) {
            if(IsWalkingToPoint) {
                Unit.CanRun = true;
                IsWalkingToPoint = false;
            }
            MovingToPoint = false;
            Unit.GetComponent<NavMeshAgent>().enabled = false;
            Unit.Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
            Unit.UnitAI.CurrentAIBehavior = Constants.AIBehavior.None;
        }
    }

    public void SaveAimDirection() {
        SavedAimDirection = Unit.Actions.GetCurrentAimVector();
    }

    public void CreateProjectile(string projectile_name) {
        if(_currentAbilityBeingPerformed != null)
        {
            Projectile proj = Utils.CreateProjectile(new(CurrentAbilityBeingPerformed), projectile_name);
        }
    }

    public void CreateVisualEffect(string vfx_name)
    {
        if (_currentAbilityBeingPerformed != null)
        {
            Utils.CreateVisualEffect(new(CurrentAbilityBeingPerformed), vfx_name);
        }
    }

    public void CreateAreaOfEffect(string aoe_name)
    {
        if (_currentAbilityBeingPerformed != null)
        {
            Utils.CreateAreaOfEffect(new(CurrentAbilityBeingPerformed), aoe_name);
        }
    }

    public void SetKnockedOutStatus()
    {
        if(Unit?.Animator != null) {
            Unit.Animator.SetBool("Knocked Out", true);
        }
    }

    public void SetFaceVariant(String variant)
    {
        if(Unit != null && Unit.SpriteRenderers["Head"] != null && Unit.SpriteRenderers["Head"].SpriteResolver != null)
        {
            Unit.SpriteRenderers["Head"].SpriteResolver.SetCategoryAndLabel("Head", variant);
            Unit.SpriteRenderers["Head"].SpriteResolver.ResolveSpriteToSpriteRenderer();
        }
    }

    public void ActivateConsumableUsage()
    {
        if (CurrentAbilityBeingPerformed != null && CurrentAbilityBeingPerformed.ItemBeingUsed != null)
        {
            CurrentAbilityBeingPerformed.ItemBeingUsed.OnUse();
            CurrentAbilityBeingPerformed.AddOrUpdateCooldown();
        }
    }

    public void CheckIfFlipDirection() {
        if (Unit.UnitCanFlipDirection == false) {
            return;
        }
        if (TryingToMoveInDirection.Contains("Right") && !TryingToMoveInDirection.Contains("Left") && IsFlipped) {
            IsFlipped = false;
            Unit.UnitSpecificActionsAfterFlipDirection();
        }
        else if (TryingToMoveInDirection.Contains("Left") && !TryingToMoveInDirection.Contains("Right") && !IsFlipped) {
            IsFlipped = true;
            Unit.UnitSpecificActionsAfterFlipDirection();
        }
        else if (TryingToMoveInDirection.Contains("Left") && TryingToMoveInDirection.Contains("Right") && IsFlipped) {
            int indexOfLeft = TryingToMoveInDirection.IndexOf("Left");
            int indexOfRight = TryingToMoveInDirection.IndexOf("Right");
            if (indexOfRight < indexOfLeft) {
                IsFlipped = false;
                Unit.UnitSpecificActionsAfterFlipDirection();
            }
        }
        else if (TryingToMoveInDirection.Contains("Left") && TryingToMoveInDirection.Contains("Right") && !IsFlipped) {
            int indexOfLeft = TryingToMoveInDirection.IndexOf("Left");
            int indexOfRight = TryingToMoveInDirection.IndexOf("Right");
            if (indexOfLeft < indexOfRight) {
                IsFlipped = true;
                Unit.UnitSpecificActionsAfterFlipDirection();
            }
        }
    }

    private void OnValidate() {

    }

    public bool StartWithFlippedDirection = true;
    [field: SerializeField]
    private bool _flipDirection;
    public bool IsFlipped {
        get => _flipDirection;
        set {
            bool previous_value = _flipDirection;
            _flipDirection = value;
            if (_flipDirection && transform.localEulerAngles.y == 0) {
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else if (!_flipDirection && transform.localEulerAngles.y != 0) {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            if(previous_value != value)
            {
                EventManager.UnitChangedDirection.Invoke(Unit);
                if(Unit is Player)
                {
                    foreach (string element in new List<string> { "Hand", "Arm", "Leg", "Foot", "Light" })
                    {
                        Utils.FlipNonSymmetricSpritePlacement(element, Player.Instance);
                    }
                }
                else if (Unit?.NonSymmetricSpriteRenderers != null && Unit.NonSymmetricSpriteRenderers.Count > 0)
                {
                    foreach (string element in Unit.NonSymmetricSpriteRenderers)
                    {
                        Utils.FlipNonSymmetricSpritePlacement(element, Unit);
                    }
                }
            }

        }
    }

    public void AllowToFinishThisAbilityEarlyByAnotherAbility() {
        if (CurrentAbilityBeingPerformed != null) {
            CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Add(Ability.AbilityInterruptType.EnergyAbility);
        }
    }

    public class QueuedInput {
        public Type AbilityToPerform;
        public int RemainingTime;
        public string MethodName;
        public Vector2 DodgeDirection;
        public Item ItemToUse;
        public bool ButtonWasReleased = false;

        public QueuedInput(string method_name, Item item_to_use = null, int time_to_stay_in_queue = Constants.ACTION_QUEUE_DURATION) {
            MethodName = method_name;
            ItemToUse = item_to_use;
            RemainingTime = time_to_stay_in_queue;
        }

        public QueuedInput(string method_name, Type ability_to_perform, Item item_to_use = null, int time_to_stay_in_queue = Constants.ACTION_QUEUE_DURATION) {
            MethodName = method_name;
            AbilityToPerform = ability_to_perform;
            ItemToUse = item_to_use;
            RemainingTime = time_to_stay_in_queue;
        }
    }
}