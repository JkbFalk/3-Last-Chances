using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Net.Http.Headers;
using Unity.VisualScripting;

public class Effect {

    public override string ToString()
    {
        if(EffectTypeName=="NoDescription") {
            return "";
        }
        if (GetDescriptionParameters().Count > 0)
        {
            return string.Format((String.IsNullOrWhiteSpace(DescriptionLabel) ?  Label.Get(GetEffectType() + "_Description") : Label.Get(DescriptionLabel)), DescriptionParameters.ToArray()) + (Label.ContainsKey(GetEffectType() + "_DescriptionDetailed") ? " [Detailed]" : "");
        }
        if(String.IsNullOrWhiteSpace(DescriptionLabel) == false && Label.ContainsKey(DescriptionLabel)) {
            return Label.Get(DescriptionLabel);
        }
        if(Label.ContainsKey(GetEffectType() + "_Description")) {
            return Label.Get(GetEffectType() + "_Description") + (Label.ContainsKey(GetEffectType() + "_DescriptionDetailed") ? " [Detailed]" : "");
        }
        Debug.LogError("Effect does not have proper description: " + GetType().ToString() + " (Source: " + SourceOfEffect + ", Target: " + TargetOfEffect + ")");
        return GetType().ToString();
    }

    public string ToStringDetailed()
    {
        if (Label.ContainsKey(GetEffectType() + "_DescriptionDetailed") == false) {
            return ToString();
        }
        if (GetDescriptionParameters().Count > 0)
        {
            return  (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + string.Format(Label.Get(GetEffectType() + "_DescriptionDetailed"), GetDescriptionParameters().ToArray());
        }
        return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + Label.Get(GetEffectType() + "_DescriptionDetailed");
    }

    public bool RemainsActiveInOtherStances = false;
    public bool IsRemovable { get; set; } = true;
    public enum EffectType { Buff, Debuff, Neutral };
    public enum EffectSourceType {Enemy, Tool, Technique, Unknown}
    public EffectSourceType SourceType = Effect.EffectSourceType.Unknown;
    public string Id;
    private List<Effect> _effectModifiers;
    public bool ShowsInMenu = true;
    public string DescriptionLabel;
    public bool CountsAsSeparateEffect = true;
    public bool HasLinearScaling = true;

    public Effect(SourceOfEffect source_of_effect) {
        SourceOfEffect = source_of_effect;
    }

    public String EffectTypeName;

    public virtual String GetEffectType() {
        return String.IsNullOrWhiteSpace(EffectTypeName) ? GetType().ToString() : "Effect_" + EffectTypeName;
    }
    public float LinearEffectValue = 0;

    public float NonLinearEffectValue = 0;

    public virtual void OnEffectValueChanged(){}

    public enum BehaviourWhenDuplicateEffectEnum { AllowDuplicate, AddDuration, AddDecayingAmount, EndShorterDuplicateWithSameIdentifier };
    public BehaviourWhenDuplicateEffectEnum BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AllowDuplicate;

    public List<string> DescriptionParameters = new List<string>();
    public bool TriggersOncePerAbility = false;
    public float SoundEffectVolume = 1;
    public bool PlaySoundEffect = false;
    public string NameOfAnimationToAutoPlay = null;
    public bool CanBeNegatedByImmunityToCrowdControl = true;
    public int PriorityLevel = 0;
    public bool ScaleWithControlAndTenacity = true;
    public float AnimationSpeed = 1;
    public bool InstantlyTransitionIntoAnimation = false;
    public EffectType Type = EffectType.Neutral;
    public GameObject VisualEffectOnTarget;
    public Image EffectIndicatorCooldownDisplay;
    public List<Effect> AdditionalEffectsAffectingTargetDuringEffect = new List<Effect>();
    public string PathToEffectGraphic;
    public Sprite EffectGraphic;
    public bool AutoPlayEffectAnimation = false;
    public Color SpecialSkinColorDuringHardCrowdControl = Color.white;
    public bool SlowlyFadeOutColor = false;
    public bool EffectEnded = false;
    public Unit TargetOfEffect { get; set; }
    public Unit UnitCreatingTheEffect { get; set; }
    public SourceOfEffect SourceOfEffect;
    private float _baseDuration = 0;
    public string SoundEffectName;
    public string Identifier = "";
    public bool ShowsInUI = false;
    private string _effectIndicatorText = "";
    public string EffectIndicatorText {
        get => _effectIndicatorText;
        set {
            if(value != _effectIndicatorText) {
                _effectIndicatorText = value;
            }
            if(EffectIndicatorCooldownDisplay != null) {
                EffectIndicatorCooldownDisplay.transform.parent.Find("Text").GetComponent<TextMeshProUGUI>().text = _effectIndicatorText;
            }
        }
    }
    public float GetOriginalBaseDuration() {
        return _baseDuration;
    }
    public List<UnityEventBase> Listeners = new List<UnityEventBase>();

    public virtual void AdditionalActionsOnSettingTargetOfEffect(Unit target_of_effect) { }
    public float BaseDuration {
        get => _baseDuration;
        set {
            if (SourceOfEffect != null && (IsHardCrowdControl() || IsSoftCrowdControl() || Type == EffectType.Debuff))
            {
                _baseDuration = ScaleWithControlAndTenacity ? value * SourceOfEffect.User.Control.Current / TargetOfEffect.Tenacity.Current : value;
            }
            else
            {
                _baseDuration = value;
            }
            RemainingDuration = _baseDuration;
        }
    }

    public virtual List<String> GetDescriptionParameters() {
        return DescriptionParameters;
    }

    public bool IsHardCrowdControl()
    {
        return GetType().IsSubclassOf(typeof(Effect_HardCrowdControl));
    }

    public bool IsSoftCrowdControl()
    {
        return GetType().IsSubclassOf(typeof(Effect_SoftCrowdControl));
    }

    
    public Effect SetTargetOfEffect(Unit target_of_effect) {
        TargetOfEffect = target_of_effect;
        return this;
    }

    public Effect SetSourceOfEffect(Unit source_of_effect) {
        UnitCreatingTheEffect = source_of_effect;
        return this;
    }

    public Effect SetAbilityCreatingThisEffect(Ability ability_creating_this_effect) {
        SourceOfEffect.SourceAbility = ability_creating_this_effect;
        return this;
    }

    public float RemainingDuration { get; set; } = 0;
    protected float _initialDecayingAmount = 0;
    private float _decayingAmount = 0;
    public float DecayingAmount { 
        get => _decayingAmount;
        protected set {
            float oldValue = _decayingAmount;
            _decayingAmount = value;
            if(oldValue != value) {
                EventManager.EffectDecayingAmountChanged.Invoke(this);
            }
        }
    }
    public float DefaultDecaySpeed = Constants.DEFAULT_STACKING_EFFECT_DECAY_PER_SECOND;
    public float MaxDecayingAmount = 0;
    public virtual void ChangeDecayingAmount(float amount_changed, bool include_effect_power = true) {
        float calculatedAmountAdded = amount_changed * (include_effect_power ? EffectPowerModifier : 1);
        DecayingAmount = MaxDecayingAmount == 0 ? (DecayingAmount + calculatedAmountAdded) : (DecayingAmount + calculatedAmountAdded) > MaxDecayingAmount ? MaxDecayingAmount : (DecayingAmount + calculatedAmountAdded);
        ExtraBehaviourOnDecayingAmountChange();
    }
    public virtual void ExtraBehaviourOnDecayingAmountChange() {}

    public virtual void ActivateEffectAmountDecay() {
        Debug.Log($"ACTIVATING DECAY: GetType {GetType()}, EffectEnded {EffectEnded}, DecayingAmount {DecayingAmount}, EffectDecaySpeedModifier {EffectDecaySpeedModifier}, DefaultDecaySpeed {DefaultDecaySpeed}");
        if(EffectEnded) {
            return;
        }
        if(DecayingAmount > -1 && DecayingAmount < 1) {
            EndThisEffect();
        }
        else {
            Debug.Log($"Analysis: EffectDecaySpeedModifier {EffectDecaySpeedModifier}, DecayingAmount {DecayingAmount}, DefaultDecaySpeed {DefaultDecaySpeed}, (1 / Math.Abs(EffectDecaySpeedModifier)) {(1 / Math.Abs(EffectDecaySpeedModifier))}, mod: {(1 / Math.Abs(EffectDecaySpeedModifier)) / 5}, result: {(DecayingAmount * DefaultDecaySpeed / (1 / Math.Abs(EffectDecaySpeedModifier)) / 5)}");
            DecayingAmount = EffectDecaySpeedModifier >= 0 ? (DecayingAmount - (DecayingAmount * DefaultDecaySpeed / (1 + EffectDecaySpeedModifier) / 5)) : (DecayingAmount - (DecayingAmount * DefaultDecaySpeed / (1 / Math.Abs(EffectDecaySpeedModifier)) / 5));
            ExtraBehaviourOnDecayingAmountChange();
        }
    }

    public float EffectPowerModifier { get {
            if(_effectModifiers == null) {
                _effectModifiers = GetEffectModifiers();
            }
            float modifier = 1;
            int count = 0;
            int count2 = 0;
            foreach(Effect e in _effectModifiers.ToArray()) {
                count++;
                if(((Effect_ChangeEffectPower)e).ChangeType == Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded) {
                    count2++;
                    modifier += ((Effect_ChangeEffectPower)e).PercentageChange / 100;
                }
            }
            return modifier;
        }
    }

    public float EffectDecaySpeedModifier { get {
            if(_effectModifiers == null) {
                _effectModifiers = GetEffectModifiers();
            }
            float modifier = 0;
            foreach(Effect e in _effectModifiers.ToArray()) {
                if(((Effect_ChangeEffectPower)e).ChangeType == Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed) {
                    Debug.Log("APPLYING DECAY MOD: " + (-((Effect_ChangeEffectPower)e).PercentageChange / 100)); 
                    modifier += -((Effect_ChangeEffectPower)e).PercentageChange / 100;
                }
            }
            return modifier;
        }
    }

    public List<Effect> GetEffectModifiers() {
        return TargetOfEffect.CurrentEffects.Concat(TargetOfEffect == SourceOfEffect.User ? new(){} : SourceOfEffect.User.CurrentEffects).Where(effect => 
            effect.GetType() == typeof(Effect_ChangeEffectPower) 
            && 
            ((Effect_ChangeEffectPower)effect).AffectedEffectType == GetType() 
            && 
            ((((Effect_ChangeEffectPower)effect).AffectedUnitsType == Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies && TargetOfEffect != Player.Instance) 
                || 
            (((Effect_ChangeEffectPower)effect).AffectedUnitsType == Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player  && TargetOfEffect == Player.Instance))
            && 
            (((Effect_ChangeEffectPower)effect).ConditionForEffectPowerChange == null || ((Effect_ChangeEffectPower)effect).ConditionForEffectPowerChange(TargetOfEffect))).ToList();
         
    }

    public void UpdateEffectModifiers(Effect effect) {
        if(effect is Effect_ChangeEffectPower) {
            _effectModifiers = GetEffectModifiers();
        }
    }

    public virtual bool CheckIfEffectAlreadyAppliedAndHandleBehaviour() {
        ChangeDecayingAmount(_initialDecayingAmount);
        Effect existingEffect = TargetOfEffect.CurrentEffects.FirstOrDefault(effect => effect.GetType() == GetType() && effect != this);
        if(existingEffect != null) {
            if(BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.AddDecayingAmount) {
                existingEffect.ChangeDecayingAmount(DecayingAmount, false);
                existingEffect.RemainingDuration = existingEffect.BaseDuration;
            }
            if(BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.AddDuration) {
                existingEffect.RemainingDuration += BaseDuration;
            }
            if(BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier) {
                if(existingEffect.Identifier == Identifier && existingEffect.RemainingDuration > RemainingDuration) {
                    EndThisEffect();
                    return true;
                }
                else if(existingEffect.Identifier == Identifier && existingEffect.RemainingDuration <= RemainingDuration) {
                    existingEffect.EndThisEffect();
                }
                return false;
            }
            EndThisEffect();
            EventManager.EffectEmpowered.Invoke(existingEffect, this);
            return true;
        }
        return false;
    }

    public void DisplayEffectIndicatorAboveTarget() {
        if (PathToEffectGraphic == null) { 
            PathToEffectGraphic = "Effect/" + GetEffectType().ToString().Replace("Effect_", "");
        }
        if(TargetOfEffect is Player) {
            Transform effectsDisplay = CanvasElements.UICanvas.Effects.transform;
            GameObject effectIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_EffectIcon")) as GameObject;
            effectIndicator.GetComponent<Image>().sprite = Type == EffectType.Buff ? Resources.Load("Sprites/UI/BuffEffectIcon", typeof(Sprite)) as Sprite : Type == EffectType.Debuff ? Resources.Load("Sprites/UI/DebuffEffectIcon", typeof(Sprite)) as Sprite : Resources.Load("Sprites/UI/EffectIcon", typeof(Sprite)) as Sprite;
            effectIndicator.transform.SetParent(effectsDisplay, false);
            effectIndicator.transform.Find("EffectImage").GetComponent<Image>().sprite = EffectGraphic == null ? Resources.Load("Sprites/" + PathToEffectGraphic, typeof(Sprite)) as Sprite : EffectGraphic;
            effectIndicator.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = EffectIndicatorText;
            EffectIndicatorCooldownDisplay = effectIndicator.transform.Find("CooldownDisplay").GetComponent<Image>();
            EffectIndicatorCooldownDisplay.fillAmount = 0;
        }
        else {
            Transform effectsDisplay = TargetOfEffect.EffectDisplay.transform;
            GameObject effectIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_EffectIcon")) as GameObject;
            effectIndicator.GetComponent<Image>().sprite = Type == EffectType.Buff ? Resources.Load("Sprites/UI/BuffEffectIcon", typeof(Sprite)) as Sprite : Type == EffectType.Debuff ? Resources.Load("Sprites/UI/DebuffEffectIcon", typeof(Sprite)) as Sprite : Resources.Load("Sprites/UI/EffectIcon", typeof(Sprite)) as Sprite;
            effectIndicator.transform.SetParent(effectsDisplay, false);
            effectIndicator.transform.Find("EffectImage").GetComponent<Image>().sprite = EffectGraphic == null ? Resources.Load("Sprites/" + PathToEffectGraphic, typeof(Sprite)) as Sprite : EffectGraphic;
            effectIndicator.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = EffectIndicatorText;
            EffectIndicatorCooldownDisplay = effectIndicator.transform.Find("CooldownDisplay").GetComponent<Image>();
            EffectIndicatorCooldownDisplay.fillAmount = 0;
        }
    }

    public virtual void AddVisualEffectOnTarget(string visual_effect_prefab_name) {
        VisualEffectOnTarget = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/" + visual_effect_prefab_name)) as GameObject;
        VisualEffectOnTarget.transform.SetParent(TargetOfEffect.VisualEffects.transform, false);
    }

    public virtual void OnStart() {
        Utils.CreateAuditLog("Unit (" + TargetOfEffect?.GetType() + ") starting effect: " + GetEffectType() + " from unit " + SourceOfEffect?.User?.GetType() + " and source " + SourceOfEffect?.GetType() + " for seconds: " + BaseDuration + ":" + Utils.GetStackTrace());
        if (IsHardCrowdControl()) {
            TargetOfEffect.Actions.CurrentActionBeingPerformed = Constants.ActionType.UnderHardCrowdControl;
            if (AutoPlayEffectAnimation && (TargetOfEffect.EffectAnimationBeingPlayed == null || PriorityLevel > TargetOfEffect.EffectAnimationBeingPlayed.PriorityLevel) && GameController.Instance.GameplayMode == Constants.GameplayMode.Regular) {
                TargetOfEffect.EffectAnimationBeingPlayed = this;
                TargetOfEffect.PlayAnimation(NameOfAnimationToAutoPlay == null ? GetEffectType() : NameOfAnimationToAutoPlay, InstantlyTransitionIntoAnimation ? 0 : 0.1f);
            }
        }
        if (SpecialSkinColorDuringHardCrowdControl != Color.white && TargetOfEffect != null) {
            TargetOfEffect.UnitColorChange.SpecialSkinColor = SpecialSkinColorDuringHardCrowdControl;
            TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        }
        if(AnimationSpeed != 1)
        {
            TargetOfEffect.Animator.speed = AnimationSpeed;
        }
        if(PlaySoundEffect)
        {
            Utils.PlaySoundEffect(TargetOfEffect.AudioSource, "Effect/" + (SoundEffectName != null ? SoundEffectName : GetEffectType()), SoundEffectVolume);
        }
        if(ShowsInUI) {
            DisplayEffectIndicatorAboveTarget();
        }
        AddListeners();
        if(BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.AddDecayingAmount) {
            _effectModifiers = GetEffectModifiers();
            EventManager.OneFifthSecondElapsedNotRealtime.AddListener(ActivateEffectAmountDecay);
            EventManager.EffectStarted.AddListener(UpdateEffectModifiers);
        }
        else if(BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier) {
            Effect e = TargetOfEffect.CurrentEffects.FirstOrDefault(effect => effect != this && effect.Identifier == Identifier);
            if(e != null) {
                e.EndThisEffect();
            }
        }
        EventManager.EffectStarted.Invoke(this);
    }

    
    public virtual void OnFixedUpdate() {}

    public virtual void OnUpdate() {
        if (SpecialSkinColorDuringHardCrowdControl != Color.white && SlowlyFadeOutColor) {
            float percentage_of_effect_elapsed = (BaseDuration - RemainingDuration) / BaseDuration;
            TargetOfEffect.UnitColorChange.SpecialSkinColor = new Color(SpecialSkinColorDuringHardCrowdControl.r + (1 - SpecialSkinColorDuringHardCrowdControl.r) * percentage_of_effect_elapsed, SpecialSkinColorDuringHardCrowdControl.g + (1 - SpecialSkinColorDuringHardCrowdControl.g) * percentage_of_effect_elapsed, SpecialSkinColorDuringHardCrowdControl.b + (1 - SpecialSkinColorDuringHardCrowdControl.b) * percentage_of_effect_elapsed);
            TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        }
    }

    public virtual void OnEnd() {
        Utils.CreateAuditLog("Unit (" + TargetOfEffect + ") ending effect: " + GetType() + " from unit " + SourceOfEffect?.User + " and source " + SourceOfEffect?.GetType() + ", original duration: " + BaseDuration);
        if (EffectIndicatorCooldownDisplay != null) {
            MonoBehaviour.Destroy(EffectIndicatorCooldownDisplay.transform.parent.gameObject);
        }
        if (AutoPlayEffectAnimation || IsHardCrowdControl()) {
            Player.Instance.Actions.SetFaceVariant("Regular");
            List<Effect> HardCrowdControlEffects = TargetOfEffect.CurrentEffects.Where(effect => effect.AutoPlayEffectAnimation && effect != this).ToList();
            Effect HardCrowdControlEffect = null;
            if(HardCrowdControlEffects != null && HardCrowdControlEffects.Count > 0)
            {
                HardCrowdControlEffect = HardCrowdControlEffects.Aggregate((e1, e2) => e1.PriorityLevel > e2.PriorityLevel ? e1 : e2);
            }
            if (HardCrowdControlEffect != null && TargetOfEffect.EffectAnimationBeingPlayed != HardCrowdControlEffect && HardCrowdControlEffect.AutoPlayEffectAnimation && HardCrowdControlEffect.RemainingDuration > 0.1f && TargetOfEffect.Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && TargetOfEffect.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != HardCrowdControlEffect.GetEffectType().Replace("Effect_", "") && GameController.Instance.GameplayMode == Constants.GameplayMode.Regular) {
                TargetOfEffect.EffectAnimationBeingPlayed = HardCrowdControlEffect;
                TargetOfEffect.PlayAnimation(String.IsNullOrWhiteSpace(HardCrowdControlEffect.NameOfAnimationToAutoPlay) == false ? HardCrowdControlEffect.NameOfAnimationToAutoPlay : HardCrowdControlEffect.GetEffectType(), HardCrowdControlEffect.InstantlyTransitionIntoAnimation ? 0 : 0.1f);
            }
            else if(TargetOfEffect.Actions.CurrentAbilityBeingPerformed == null || IsHardCrowdControl())
            {
                TargetOfEffect.Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
                TargetOfEffect.UnitColorChange.SpecialSkinColor = Color.white;
                TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
                if (TargetOfEffect.EffectAnimationBeingPlayed == this)
                {
                    TargetOfEffect.EffectAnimationBeingPlayed = null;
                }
            }
        }
        if (VisualEffectOnTarget != null) {
            MonoBehaviour.Destroy(VisualEffectOnTarget);
        }
        if (AnimationSpeed != 1)
        {
            TargetOfEffect.Animator.speed = 1;
        }
        if (AdditionalEffectsAffectingTargetDuringEffect.Count > 0) {
            foreach (Effect e2 in AdditionalEffectsAffectingTargetDuringEffect) {
                if(e2 != null) {
                    e2.EndThisEffect();
                }
            }
        }
        EffectEnded = true;
        RemoveListeners();
        EventManager.EffectEnded.Invoke(this);
        if(SaveFile.Instance.DifficultyLevel > 1 && IsHardCrowdControl() && TargetOfEffect.UnitAI != null && TargetOfEffect.InCombat) {
            TargetOfEffect.UnitAI.DecideOnNextAction(true);
        }
    }

    public void EndThisEffect()
    {
        if(TargetOfEffect != null) {
            TargetOfEffect.EndEffect(this);
        }
    }

    public Effect SetTypeOfEffect(EffectType type_of_effect) {
        Type = type_of_effect;
        return this;
    }

    public void AddListeners() {
        if(Listeners.Contains(EventManager.OneFifthSecondElapsedNotRealtime)) {
            EventManager.OneFifthSecondElapsedNotRealtime.AddListener(OnInvokeOneFifthSecondElapsedNotRealtime);
        }
        if(Listeners.Contains(EventManager.OneFifthSecondElapsedRealtime)) {
            EventManager.OneFifthSecondElapsedRealtime.AddListener(OnInvokeOneFifthSecondElapsedRealtime);
        }
        if(Listeners.Contains(EventManager.HitDealt)) {
            EventManager.HitDealt.AddListener(OnInvokeHitDealt);
        }
        if(Listeners.Contains(EventManager.AfterHitDamageCalculation)) {
            EventManager.AfterHitDamageCalculation.AddListener(OnInvokeAfterHitDamageCalculation);
        }
        if(Listeners.Contains(EventManager.DamageDealt)) {
            EventManager.DamageDealt.AddListener(OnInvokeDamageDealt);
        }
        if(Listeners.Contains(EventManager.DamageWasDodged)) {
            EventManager.DamageWasDodged.AddListener(OnInvokeDamageWasDodged);
        }
        if(Listeners.Contains(EventManager.AbilityUsed)) {
            EventManager.AbilityUsed.AddListener(OnInvokeAbilityUsed);
        }
        if(Listeners.Contains(EventManager.AbilityEnded)) {
            EventManager.AbilityEnded.AddListener(OnInvokeAbilityEnded);
        }
        if(Listeners.Contains(EventManager.AbilityEnergyConsumed)) {
            EventManager.AbilityEnergyConsumed.AddListener(OnInvokeAbilityEnergyConsumed);
        }
        if(Listeners.Contains(EventManager.UnitKnockedOut)) {
            EventManager.UnitKnockedOut.AddListener(OnInvokeUnitKnockedOut);
        }
        if(Listeners.Contains(EventManager.HealthBarBroken)) {
            EventManager.HealthBarBroken.AddListener(OnInvokeHealthBarBroken);
        }
        if(Listeners.Contains(EventManager.AmmoAmountChanged)) {
            EventManager.AmmoAmountChanged.AddListener(OnInvokeAmmoAmountChanged);
        }
        if(Listeners.Contains(EventManager.HealthBarBroken)) {
            EventManager.HealthBarBroken.AddListener(OnInvokeHealthBarBroken);
        }
        if(Listeners.Contains(EventManager.ProjectileCreated)) {
            EventManager.ProjectileCreated.AddListener(OnInvokeProjectileCreated);
        }
        if(Listeners.Contains(EventManager.UnitStatCurrentAmountChanged)) {
            EventManager.UnitStatCurrentAmountChanged.AddListener(OnInvokeUnitStatCurrentAmountChanged);
        }
        if(Listeners.Contains(EventManager.EffectStarted)) {
            EventManager.EffectStarted.AddListener(OnInvokeEffectStarted);
        }
        if(Listeners.Contains(EventManager.EffectEmpowered)) {
            EventManager.EffectEmpowered.AddListener(OnInvokeEffectEmpowered);
        }
        if(Listeners.Contains(EventManager.EffectDecayingAmountChanged)) {
            EventManager.EffectDecayingAmountChanged.AddListener(OnInvokeEffectDecayingAmountChanged);
        }
        if(Listeners.Contains(EventManager.EffectEnded)) {
            EventManager.EffectEnded.AddListener(OnInvokeEffectEnded);
        }
        if(Listeners.Contains(EventManager.CooldownAdded)) {
            EventManager.CooldownAdded.AddListener(OnInvokeCooldownAdded);
        }
        if(Listeners.Contains(EventManager.ItemEquipped)) {
            EventManager.ItemEquipped.AddListener(OnInvokeItemEquipped);
        }
        if(Listeners.Contains(EventManager.StanceSwitched)) {
            EventManager.StanceSwitched.AddListener(OnInvokeStanceSwitched);
        }
        if(Listeners.Contains(EventManager.ExitCombat)) {
            EventManager.ExitCombat.AddListener(OnInvokeExitCombat);
        }
    }

    public void RemoveListeners() {
        if(Listeners.Contains(EventManager.OneFifthSecondElapsedNotRealtime)) {
            EventManager.OneFifthSecondElapsedNotRealtime.RemoveListener(OnInvokeOneFifthSecondElapsedNotRealtime);
        }
        if(Listeners.Contains(EventManager.OneFifthSecondElapsedRealtime)) {
            EventManager.OneFifthSecondElapsedRealtime.RemoveListener(OnInvokeOneFifthSecondElapsedRealtime);
        }
        if(Listeners.Contains(EventManager.HitDealt)) {
            EventManager.HitDealt.RemoveListener(OnInvokeHitDealt);
        }
        if(Listeners.Contains(EventManager.AfterHitDamageCalculation)) {
            EventManager.AfterHitDamageCalculation.RemoveListener(OnInvokeAfterHitDamageCalculation);
        }
        if(Listeners.Contains(EventManager.DamageDealt)) {
            EventManager.DamageDealt.RemoveListener(OnInvokeDamageDealt);
        }
        if(Listeners.Contains(EventManager.DamageWasDodged)) {
            EventManager.DamageWasDodged.RemoveListener(OnInvokeDamageWasDodged);
        }
        if(Listeners.Contains(EventManager.AbilityUsed)) {
            EventManager.AbilityUsed.RemoveListener(OnInvokeAbilityUsed);
        }
        if(Listeners.Contains(EventManager.AbilityEnded)) {
            EventManager.AbilityEnded.RemoveListener(OnInvokeAbilityEnded);
        }
        if(Listeners.Contains(EventManager.AbilityEnergyConsumed)) {
            EventManager.AbilityEnergyConsumed.RemoveListener(OnInvokeAbilityEnergyConsumed);
        }
        if(Listeners.Contains(EventManager.UnitKnockedOut)) {
            EventManager.UnitKnockedOut.RemoveListener(OnInvokeUnitKnockedOut);
        }
        if(Listeners.Contains(EventManager.HealthBarBroken)) {
            EventManager.HealthBarBroken.RemoveListener(OnInvokeHealthBarBroken);
        }
        if(Listeners.Contains(EventManager.AmmoAmountChanged)) {
            EventManager.AmmoAmountChanged.RemoveListener(OnInvokeAmmoAmountChanged);
        }
        if(Listeners.Contains(EventManager.HealthBarBroken)) {
            EventManager.HealthBarBroken.RemoveListener(OnInvokeHealthBarBroken);
        }
        if(Listeners.Contains(EventManager.ProjectileCreated)) {
            EventManager.ProjectileCreated.RemoveListener(OnInvokeProjectileCreated);
        }
        if(Listeners.Contains(EventManager.UnitStatCurrentAmountChanged)) {
            EventManager.UnitStatCurrentAmountChanged.RemoveListener(OnInvokeUnitStatCurrentAmountChanged);
        }
        if(Listeners.Contains(EventManager.EffectStarted)) {
            EventManager.EffectStarted.RemoveListener(OnInvokeEffectStarted);
        }
        if(Listeners.Contains(EventManager.EffectEmpowered)) {
            EventManager.EffectEmpowered.RemoveListener(OnInvokeEffectEmpowered);
        }
        if(Listeners.Contains(EventManager.EffectDecayingAmountChanged)) {
            EventManager.EffectDecayingAmountChanged.RemoveListener(OnInvokeEffectDecayingAmountChanged);
        }
        if(Listeners.Contains(EventManager.EffectEnded)) {
            EventManager.EffectEnded.RemoveListener(OnInvokeEffectEnded);
        }
        if(Listeners.Contains(EventManager.CooldownAdded)) {
            EventManager.CooldownAdded.RemoveListener(OnInvokeCooldownAdded);
        }
        if(Listeners.Contains(EventManager.ItemEquipped)) {
            EventManager.ItemEquipped.RemoveListener(OnInvokeItemEquipped);
        }
        if(Listeners.Contains(EventManager.StanceSwitched)) {
            EventManager.StanceSwitched.RemoveListener(OnInvokeStanceSwitched);
        }
        if(Listeners.Contains(EventManager.ExitCombat)) {
            EventManager.ExitCombat.RemoveListener(OnInvokeExitCombat);
        }
    }

    public virtual void OnInvokeOneFifthSecondElapsedNotRealtime() {
        EventManager.EffectActivated.Invoke(this);
    } 

    public virtual void OnInvokeOneFifthSecondElapsedRealtime() {
        EventManager.EffectActivated.Invoke(this);
    } 

    public virtual void OnInvokeHitDealt(Damage damage) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAfterHitDamageCalculation(Damage damage) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeDamageDealt(Damage damage) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeDamageWasDodged(Damage damage) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAbilityUsed(Ability ability){
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && ability.TriggeredEffects.Contains(this) == false)
        {
            ability.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAbilityEnded(Ability ability){
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && ability.TriggeredEffects.Contains(this) == false)
        {
            ability.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAbilityEnergyConsumed(Ability ability, float amount){
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && ability.TriggeredEffects.Contains(this) == false)
        {
            ability.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeUnitKnockedOut(Damage damage){
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAmmoAmountChanged(){
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeHealthBarBroken(Damage damage){
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeProjectileCreated(Projectile projectile){
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && projectile.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            projectile.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount) {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeEffectStarted(Effect effect) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeEffectEmpowered(Effect existing_effect, Effect new_effect) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && existing_effect.SourceOfEffect.SourceAbility != null && existing_effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            existing_effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeEffectDecayingAmountChanged(Effect effect) {
        EventManager.EffectDecayingAmountChanged.Invoke(this);
        if (TriggersOncePerAbility && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeEffectEnded(Effect effect) {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeCooldownAdded(Cooldown cooldown) {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeItemEquipped(Item item1, Item item2) {
        EventManager.EffectActivated.Invoke(this);
    } 

    public virtual void OnInvokeStanceSwitched() {
        EventManager.EffectActivated.Invoke(this);
    } 

    public virtual void OnInvokeExitCombat(Unit unit) {
        EventManager.EffectActivated.Invoke(this);
    } 
}