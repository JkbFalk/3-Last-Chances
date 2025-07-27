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
using System.Reflection;

public class Effect
{
    public bool RemainsActiveInOtherStances = false;
    public bool IsRemovable { get; set; } = true;
    public enum EffectType { Buff, Debuff, Neutral };
    protected float _flatAmount = 0;
    public virtual float FlatAmount
    {
        get => _flatAmount;
        set { _flatAmount = value; }
    }
    protected float _percentageAmount = 0;
    public virtual float PercentageAmount
    {
        get => _percentageAmount;
        set { _percentageAmount = value; }
    }
    public float FirstParameter = 0;
    public float SecondParameter = 0;
    public float ThirdParameter = 0;
    private List<Effect> _effectModifiers;
    public bool ShowsInMenu = true;
    public float NewEffectIndicatorExtraScaleTimer = Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_TIMER;
    public virtual int StackingEffectIntensityLevel
    {
        get { return 0; }
    }
    public bool CountsAsSeparateEffect = true;
    public bool HasLinearScaling = true;
    public List<Stat> ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat>();

    public Effect(SourceOfEffect source_of_effect)
    {
        SourceOfEffect = source_of_effect;
    }

    public float PowerBudget = 0;

    public enum BehaviourWhenDuplicateEffectEnum { AllowDuplicate, AddDuration, AddDecayingAmount, EndShorterDuplicateWithSameIdentifier, EndExistingEffect };
    public BehaviourWhenDuplicateEffectEnum BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AllowDuplicate;

    public List<string> DescriptionParameters = new List<string>();
    public bool TriggerOnEffectEndedEvent = true;
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
    public Image UICooldownDisplay;
    public List<Effect> AdditionalEffectsAffectingTargetDuringEffect = new List<Effect>();
    public string PathToUIGraphic;
    public Sprite UIGraphic;
    public GameObject TileInUI;
    public bool AutoPlayEffectAnimation = false;
    public Color SpecialSkinColorDuringHardCrowdControl = Color.white;
    public bool SlowlyFadeOutColor = false;
    public bool EffectEnded = false;
    public Unit TargetOfEffect { get; set; }
    public Unit UnitCreatingTheEffect
    {
        get
        {
            if (SourceOfEffect != null && SourceOfEffect.SourceUnit != null)
            {
                return SourceOfEffect.SourceUnit;
            }
            else if (SourceOfEffect != null && SourceOfEffect.SourceAbility != null)
            {
                return SourceOfEffect.SourceAbility.User;
            }
            else return TargetOfEffect;
        }
    }
    public SourceOfEffect SourceOfEffect;
    private float _baseDuration = 0;
    public string SoundEffectName;
    public string Identifier = "";
    private bool _showsInUI = false;
    public bool ShowsInUI
    {
        get => _showsInUI;
        set
        {
            bool prevValue = _showsInUI;
            _showsInUI = value;
            if (!prevValue && _showsInUI)
            {
                ShowInUI();
            }
            else if (prevValue && !_showsInUI)
            {
                MonoBehaviour.Destroy(TileInUI);
            }
        }
    }
    public string HideInUIWhileCooldownWithIdExists = "";
    private string _uiText = "";
    public string UIText
    {
        get => _uiText;
        set
        {
            if (value != _uiText)
            {
                _uiText = value;
            }
            if (UICooldownDisplay != null)
            {
                UICooldownDisplay.transform.parent.Find("Text").GetComponent<TextMeshProUGUI>().text = _uiText;
            }
        }
    }
    public List<UnityEventBase> Listeners = new List<UnityEventBase>();

    public virtual void AdditionalActionsOnSettingTargetOfEffect(Unit target_of_effect) { }
    public float BaseDuration
    {
        get => _baseDuration;
        set
        {
            if (SourceOfEffect != null && (IsHardCrowdControl() || IsSoftCrowdControl() || Type == EffectType.Debuff))
            {
                _baseDuration = ScaleWithControlAndTenacity ? value * Utils.GetEffectiveCrowdControlDuration(SourceOfEffect.User, TargetOfEffect) : value;
            }
            else
            {
                _baseDuration = value;
            }
            RemainingDuration = _baseDuration;
        }
    }

    public virtual List<String> GetDescriptionParameters()
    {
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

    public float RemainingDuration { get; set; } = 0;
    protected float _initialDecayingAmount = 0;
    private float _decayingAmount = 0;
    public float DecayingAmount
    {
        get => _decayingAmount;
        protected set
        {
            float oldValue = _decayingAmount;
            _decayingAmount = value;
            if (oldValue != value)
            {
                EventManager.EffectDecayingAmountChanged.Invoke(this);
            }
        }
    }
    public float DefaultDecaySpeed = Constants.DEFAULT_STACKING_EFFECT_DECAY_PER_SECOND;
    public float MaxDecayingAmount = 0;
    public virtual void ChangeDecayingAmount(float amount_changed, bool include_effect_power = true)
    {
        float calculatedAmountAdded = amount_changed * (include_effect_power ? EffectPowerModifier : 1);
        DecayingAmount = MaxDecayingAmount == 0 ? (DecayingAmount + calculatedAmountAdded) : (DecayingAmount + calculatedAmountAdded) > MaxDecayingAmount ? MaxDecayingAmount : (DecayingAmount + calculatedAmountAdded);
        ExtraBehaviourOnDecayingAmountChange();
    }
    public virtual void ExtraBehaviourOnDecayingAmountChange() { }

    public virtual void ActivateEffectAmountDecay()
    {
        if (EffectEnded)
        {
            return;
        }
        if (DecayingAmount > -1 && DecayingAmount < 1)
        {
            EndThisEffect();
        }
        else
        {
            DecayingAmount = EffectDecaySpeedModifier >= 0 ? (DecayingAmount - (DecayingAmount * DefaultDecaySpeed / (1 + EffectDecaySpeedModifier) / 5)) : (DecayingAmount - (DecayingAmount * DefaultDecaySpeed / (1 / Math.Abs(EffectDecaySpeedModifier)) / 5));
            ExtraBehaviourOnDecayingAmountChange();
        }
    }

    public float EffectPowerModifier
    {
        get
        {
            if (_effectModifiers == null)
            {
                _effectModifiers = GetEffectModifiers();
            }
            float modifier = 1;
            int count = 0;
            int count2 = 0;
            foreach (Effect e in _effectModifiers.ToArray())
            {
                count++;
                if (((Effect_ChangeEffectPower)e).ChangeType == Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded)
                {
                    count2++;
                    modifier += ((Effect_ChangeEffectPower)e).PercentageChange / 100;
                }
            }
            return modifier;
        }
    }

    public float EffectDecaySpeedModifier
    {
        get
        {
            if (_effectModifiers == null)
            {
                _effectModifiers = GetEffectModifiers();
            }
            float modifier = 0;
            foreach (Effect e in _effectModifiers.ToArray())
            {
                if (((Effect_ChangeEffectPower)e).ChangeType == Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed)
                {
                    modifier += -((Effect_ChangeEffectPower)e).PercentageChange / 100;
                }
            }
            return modifier;
        }
    }

    public List<Effect> GetEffectModifiers()
    {
        if (TargetOfEffect?.CurrentEffects == null)
        {
            return new List<Effect>();
        }
        return TargetOfEffect.CurrentEffects.Concat(TargetOfEffect == SourceOfEffect.User ? new() { } : SourceOfEffect.User.CurrentEffects).Where(effect =>
            effect.GetType() == typeof(Effect_ChangeEffectPower)
            &&
            ((Effect_ChangeEffectPower)effect).AffectedEffectType == GetType()
            &&
            ((((Effect_ChangeEffectPower)effect).AffectedUnitsType == Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies && TargetOfEffect != Player.Instance)
                ||
            (((Effect_ChangeEffectPower)effect).AffectedUnitsType == Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player && TargetOfEffect == Player.Instance))
            &&
            (((Effect_ChangeEffectPower)effect).ConditionForEffectPowerChange == null || ((Effect_ChangeEffectPower)effect).ConditionForEffectPowerChange(TargetOfEffect))).ToList();

    }

    public void UpdateEffectModifiers(Effect effect)
    {
        if (effect is Effect_ChangeEffectPower)
        {
            _effectModifiers = GetEffectModifiers();
        }
    }

    public virtual bool CheckIfEffectAlreadyAppliedAndHandleBehaviour()
    {
        if (_initialDecayingAmount != 0)
        {
            ChangeDecayingAmount(_initialDecayingAmount);
        }
        Effect existingEffect = TargetOfEffect.CurrentEffects.FirstOrDefault(effect => effect.GetType() == GetType() && effect != this);
        if (existingEffect != null)
        {
            if (BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.EndExistingEffect)
            {
                existingEffect.EndThisEffect();
                return false;
            }
            else if (BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.AddDecayingAmount)
            {
                existingEffect.ChangeDecayingAmount(DecayingAmount, false);
                existingEffect.RemainingDuration = existingEffect.BaseDuration;
            }
            else if (BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.AddDuration)
            {
                existingEffect.RemainingDuration += BaseDuration;
            }
            else if (BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier)
            {
                if (existingEffect.Identifier == Identifier && existingEffect.RemainingDuration > RemainingDuration)
                {
                    TriggerOnEffectEndedEvent = false;
                    EndThisEffect();
                    return true;
                }
                else if (existingEffect.Identifier == Identifier && existingEffect.RemainingDuration <= RemainingDuration)
                {
                    existingEffect.TriggerOnEffectEndedEvent = false;
                    existingEffect.EndThisEffect();
                }
                return false;
            }
            TriggerOnEffectEndedEvent = false;
            EndThisEffect();
            EventManager.EffectEmpowered.Invoke(existingEffect, this);
            return true;
        }
        return false;
    }

    public void ShowInUI()
    {
        if (TargetOfEffect == null || (TileInUI != null && !TileInUI.IsDestroyed()))
        {
            return;
        }
        if (PathToUIGraphic == null)
        {
            PathToUIGraphic = "Effect/" + GetType().ToString().Replace("Effect_", "");
        }
        if (TargetOfEffect is Player || TargetOfEffect.IsBoss == false || TargetOfEffect.OnScreenBars != null)
        {
            Transform effectsDisplay =
                TargetOfEffect is Player ? UIManager.Objects.Effects.transform :
                TargetOfEffect.IsBoss ? TargetOfEffect.OnScreenBars.transform.Find("Effects") :
                TargetOfEffect.transform.Find("World Space Canvas/Effects");
            TileInUI = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_Effect" + Type.ToString())) as GameObject;
            TileInUI.transform.SetParent(effectsDisplay, false);
            TileInUI.transform.Find("EffectImage").GetComponent<Image>().sprite = UIGraphic == null ? Resources.Load("Sprites/" + PathToUIGraphic, typeof(Sprite)) as Sprite : UIGraphic;
            TileInUI.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = UIText;
            TileInUI.transform.localScale = new Vector3(Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_SIZE, Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_SIZE, 1);
            NewEffectIndicatorExtraScaleTimer = Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_TIMER;
            UICooldownDisplay = TileInUI.transform.Find("CooldownDisplay").GetComponent<Image>();
            UICooldownDisplay.fillAmount = 0;
        }
    }

    public virtual void AddVisualEffectOnTarget(string visual_effect_prefab_name)
    {
        VisualEffectOnTarget = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/" + visual_effect_prefab_name)) as GameObject;
        VisualEffectOnTarget.transform.SetParent(TargetOfEffect.VisualEffects.transform, false);
    }

    public virtual void OnStart()
    {
        Utils.CreateAuditLog($"{TargetOfEffect.gameObject.name.Replace("(Clone)", "")} starting effect: {GetType()} from {SourceOfEffect?.User.gameObject.name.Replace("(Clone)", "")} (Source: {SourceOfEffect?.Source}), Duration: {(BaseDuration == 0 ? "-" : BaseDuration.ToString())}");
        if (IsHardCrowdControl())
        {
            TargetOfEffect.Actions.CurrentActionBeingPerformed = Constants.ActionType.UnderHardCrowdControl;
            if (AutoPlayEffectAnimation && (TargetOfEffect.EffectAnimationBeingPlayed == null || PriorityLevel > TargetOfEffect.EffectAnimationBeingPlayed.PriorityLevel) && GameController.Instance.GameplayMode == Constants.GameplayMode.Regular)
            {
                TargetOfEffect.EffectAnimationBeingPlayed = this;
                TargetOfEffect.PlayAnimation(NameOfAnimationToAutoPlay == null ? GetType().ToString() : NameOfAnimationToAutoPlay, InstantlyTransitionIntoAnimation ? 0 : 0.1f);
            }
        }
        if (SpecialSkinColorDuringHardCrowdControl != Color.white && TargetOfEffect != null)
        {
            TargetOfEffect.UnitColorChange.SpecialSkinColor = SpecialSkinColorDuringHardCrowdControl;
            TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        }
        if (AnimationSpeed != 1)
        {
            TargetOfEffect.Animator.speed = AnimationSpeed;
        }
        if (PlaySoundEffect)
        {
            Utils.PlaySoundEffect(TargetOfEffect.AudioSource, "Effect/" + (SoundEffectName != null ? SoundEffectName : GetType()), SoundEffectVolume);
        }
        if (ShowsInUI && (HideInUIWhileCooldownWithIdExists == "" || !TargetOfEffect.CheckIfEffectIsOnCooldown(HideInUIWhileCooldownWithIdExists)))
        {
            ShowInUI();
        }
        AddListeners();
        if (BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.AddDecayingAmount)
        {
            _effectModifiers = GetEffectModifiers();
            EventManager.OneTenthSecondElapsedInGame.AddListener(ActivateEffectAmountDecay);
            EventManager.EffectStarted.AddListener(UpdateEffectModifiers);
        }
        else if (BehaviourWhenDuplicateEffect == BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier)
        {
            Effect e = TargetOfEffect.CurrentEffects.FirstOrDefault(effect => effect != this && effect.Identifier == Identifier);
            if (e != null)
            {
                e.EndThisEffect();
            }
        }
        EventManager.EffectStarted.Invoke(this);
    }


    public virtual void OnFixedUpdate() { }

    public virtual void OnUpdate()
    {
        if (SpecialSkinColorDuringHardCrowdControl != Color.white && SlowlyFadeOutColor)
        {
            float percentage_of_effect_elapsed = (BaseDuration - RemainingDuration) / BaseDuration;
            TargetOfEffect.UnitColorChange.SpecialSkinColor = new Color(SpecialSkinColorDuringHardCrowdControl.r + (1 - SpecialSkinColorDuringHardCrowdControl.r) * percentage_of_effect_elapsed, SpecialSkinColorDuringHardCrowdControl.g + (1 - SpecialSkinColorDuringHardCrowdControl.g) * percentage_of_effect_elapsed, SpecialSkinColorDuringHardCrowdControl.b + (1 - SpecialSkinColorDuringHardCrowdControl.b) * percentage_of_effect_elapsed);
            TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        }
    }

    public virtual void OnEnd()
    {
        Utils.CreateAuditLog($"{TargetOfEffect.gameObject.name.Replace("(Clone)", "")} ending effect: {GetType()} from {SourceOfEffect?.User.gameObject.name.Replace("(Clone)", "")} (Source: {SourceOfEffect?.Source}), Duration: {(BaseDuration == 0 ? "-" : BaseDuration.ToString())}");
        if (UICooldownDisplay != null)
        {
            MonoBehaviour.Destroy(UICooldownDisplay.transform.parent.gameObject);
        }
        if (AutoPlayEffectAnimation || IsHardCrowdControl())
        {
            Player.Instance.Actions.SetFaceVariant("Regular");
            List<Effect> HardCrowdControlEffects = TargetOfEffect.CurrentEffects.Where(effect => effect.AutoPlayEffectAnimation && effect != this).ToList();
            Effect HardCrowdControlEffect = null;
            if (HardCrowdControlEffects != null && HardCrowdControlEffects.Count > 0)
            {
                HardCrowdControlEffect = HardCrowdControlEffects.Aggregate((e1, e2) => e1.PriorityLevel > e2.PriorityLevel ? e1 : e2);
            }
            if (HardCrowdControlEffect != null && TargetOfEffect.EffectAnimationBeingPlayed != HardCrowdControlEffect && HardCrowdControlEffect.AutoPlayEffectAnimation && HardCrowdControlEffect.RemainingDuration > 0.1f && TargetOfEffect.Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && TargetOfEffect.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != HardCrowdControlEffect.GetType().ToString().Replace("Effect_", "") && GameController.Instance.GameplayMode == Constants.GameplayMode.Regular)
            {
                TargetOfEffect.EffectAnimationBeingPlayed = HardCrowdControlEffect;
                TargetOfEffect.PlayAnimation(String.IsNullOrWhiteSpace(HardCrowdControlEffect.NameOfAnimationToAutoPlay) == false ? HardCrowdControlEffect.NameOfAnimationToAutoPlay : HardCrowdControlEffect.GetType().ToString(), HardCrowdControlEffect.InstantlyTransitionIntoAnimation ? 0 : 0.1f);
            }
            else if (TargetOfEffect.Actions.CurrentAbilityBeingPerformed == null || IsHardCrowdControl())
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
        if (VisualEffectOnTarget != null)
        {
            MonoBehaviour.Destroy(VisualEffectOnTarget);
        }
        if (AnimationSpeed != 1)
        {
            TargetOfEffect.Animator.speed = 1;
        }
        if (AdditionalEffectsAffectingTargetDuringEffect.Count > 0)
        {
            foreach (Effect e2 in AdditionalEffectsAffectingTargetDuringEffect)
            {
                if (e2 != null)
                {
                    e2.EndThisEffect();
                }
            }
        }
        EffectEnded = true;
        RemoveListeners();
        if (TriggerOnEffectEndedEvent)
        {
            EventManager.EffectEnded.Invoke(this);
        }
        if (SaveFile.Instance.DifficultyLevel > 1 && IsHardCrowdControl() && TargetOfEffect.UnitAI != null && TargetOfEffect.InCombat)
        {
            TargetOfEffect.UnitAI.DecideOnNextAction(true);
        }
    }

    public void EndThisEffect()
    {
        if (TargetOfEffect != null)
        {
            TargetOfEffect.EndEffect(this);
        }
    }

    public void AddListeners()
    {
        if (Listeners.Contains(EventManager.OneTenthSecondElapsedInGame))
        {
            EventManager.OneTenthSecondElapsedInGame.AddListener(OnInvokeOneTenthSecondElapsedInGame);
        }
        if (Listeners.Contains(EventManager.OneTenthSecondElapsedRealtime))
        {
            EventManager.OneTenthSecondElapsedRealtime.AddListener(OnInvokeOneTenthSecondElapsedRealtime);
        }
        if (Listeners.Contains(EventManager.HitDealt))
        {
            EventManager.HitDealt.AddListener(OnInvokeHitDealt);
        }
        if (Listeners.Contains(EventManager.AfterHitDamageCalculation))
        {
            EventManager.AfterHitDamageCalculation.AddListener(OnInvokeAfterHitDamageCalculation);
        }
        if (Listeners.Contains(EventManager.AboutToHandleFatalBlow))
        {
            EventManager.AboutToHandleFatalBlow.AddListener(OnInvokeAboutToHandleFatalBlow);
        }
        if (Listeners.Contains(EventManager.DamageDealt))
        {
            EventManager.DamageDealt.AddListener(OnInvokeDamageDealt);
        }
        if (Listeners.Contains(EventManager.DamageWasDodged))
        {
            EventManager.DamageWasDodged.AddListener(OnInvokeDamageWasDodged);
        }
        if (Listeners.Contains(EventManager.AbilityUsed))
        {
            EventManager.AbilityUsed.AddListener(OnInvokeAbilityUsed);
        }
        if (Listeners.Contains(EventManager.AbilityEnded))
        {
            EventManager.AbilityEnded.AddListener(OnInvokeAbilityEnded);
        }
        if (Listeners.Contains(EventManager.AbilityEnergyConsumed))
        {
            EventManager.AbilityEnergyConsumed.AddListener(OnInvokeAbilityEnergyConsumed);
        }
        if (Listeners.Contains(EventManager.UnitKnockedOut))
        {
            EventManager.UnitKnockedOut.AddListener(OnInvokeUnitKnockedOut);
        }
        if (Listeners.Contains(EventManager.HealthBarBroken))
        {
            EventManager.HealthBarBroken.AddListener(OnInvokeHealthBarBroken);
        }
        if (Listeners.Contains(EventManager.AmmoAmountChanged))
        {
            EventManager.AmmoAmountChanged.AddListener(OnInvokeAmmoAmountChanged);
        }
        if (Listeners.Contains(EventManager.HealthBarBroken))
        {
            EventManager.HealthBarBroken.AddListener(OnInvokeHealthBarBroken);
        }
        if (Listeners.Contains(EventManager.ProjectileCreated))
        {
            EventManager.ProjectileCreated.AddListener(OnInvokeProjectileCreated);
        }
        if (Listeners.Contains(EventManager.UnitStatCurrentAmountChanged))
        {
            EventManager.UnitStatCurrentAmountChanged.AddListener(OnInvokeUnitStatCurrentAmountChanged);
        }
        if (Listeners.Contains(EventManager.EffectStarted))
        {
            EventManager.EffectStarted.AddListener(OnInvokeEffectStarted);
        }
        if (Listeners.Contains(EventManager.EffectEmpowered))
        {
            EventManager.EffectEmpowered.AddListener(OnInvokeEffectEmpowered);
        }
        if (Listeners.Contains(EventManager.EffectDecayingAmountChanged))
        {
            EventManager.EffectDecayingAmountChanged.AddListener(OnInvokeEffectDecayingAmountChanged);
        }
        if (Listeners.Contains(EventManager.EffectEnded))
        {
            EventManager.EffectEnded.AddListener(OnInvokeEffectEnded);
        }
        if (Listeners.Contains(EventManager.AboutToAddCooldown))
        {
            EventManager.AboutToAddCooldown.AddListener(OnInvokeAboutToAddCooldown);
        }
        if (Listeners.Contains(EventManager.CooldownAdded))
        {
            EventManager.CooldownAdded.AddListener(OnInvokeCooldownAdded);
        }
        if (Listeners.Contains(EventManager.ItemEquipped))
        {
            EventManager.ItemEquipped.AddListener(OnInvokeItemEquipped);
        }
        if (Listeners.Contains(EventManager.StanceSwitched))
        {
            EventManager.StanceSwitched.AddListener(OnInvokeStanceSwitched);
        }
        if (Listeners.Contains(EventManager.ExitCombat))
        {
            EventManager.ExitCombat.AddListener(OnInvokeExitCombat);
        }
        if (Listeners.Contains(EventManager.EnterCombat))
        {
            EventManager.EnterCombat.AddListener(OnInvokeEnterCombat);
        }
    }

    public void RemoveListeners()
    {
        if (Listeners.Contains(EventManager.OneTenthSecondElapsedInGame))
        {
            EventManager.OneTenthSecondElapsedInGame.RemoveListener(OnInvokeOneTenthSecondElapsedInGame);
        }
        if (Listeners.Contains(EventManager.OneTenthSecondElapsedRealtime))
        {
            EventManager.OneTenthSecondElapsedRealtime.RemoveListener(OnInvokeOneTenthSecondElapsedRealtime);
        }
        if (Listeners.Contains(EventManager.HitDealt))
        {
            EventManager.HitDealt.RemoveListener(OnInvokeHitDealt);
        }
        if (Listeners.Contains(EventManager.AfterHitDamageCalculation))
        {
            EventManager.AfterHitDamageCalculation.RemoveListener(OnInvokeAfterHitDamageCalculation);
        }
        if (Listeners.Contains(EventManager.AboutToHandleFatalBlow))
        {
            EventManager.AboutToHandleFatalBlow.RemoveListener(OnInvokeAboutToHandleFatalBlow);
        }
        if (Listeners.Contains(EventManager.DamageDealt))
        {
            EventManager.DamageDealt.RemoveListener(OnInvokeDamageDealt);
        }
        if (Listeners.Contains(EventManager.DamageWasDodged))
        {
            EventManager.DamageWasDodged.RemoveListener(OnInvokeDamageWasDodged);
        }
        if (Listeners.Contains(EventManager.AbilityUsed))
        {
            EventManager.AbilityUsed.RemoveListener(OnInvokeAbilityUsed);
        }
        if (Listeners.Contains(EventManager.AbilityEnded))
        {
            EventManager.AbilityEnded.RemoveListener(OnInvokeAbilityEnded);
        }
        if (Listeners.Contains(EventManager.AbilityEnergyConsumed))
        {
            EventManager.AbilityEnergyConsumed.RemoveListener(OnInvokeAbilityEnergyConsumed);
        }
        if (Listeners.Contains(EventManager.UnitKnockedOut))
        {
            EventManager.UnitKnockedOut.RemoveListener(OnInvokeUnitKnockedOut);
        }
        if (Listeners.Contains(EventManager.HealthBarBroken))
        {
            EventManager.HealthBarBroken.RemoveListener(OnInvokeHealthBarBroken);
        }
        if (Listeners.Contains(EventManager.AmmoAmountChanged))
        {
            EventManager.AmmoAmountChanged.RemoveListener(OnInvokeAmmoAmountChanged);
        }
        if (Listeners.Contains(EventManager.HealthBarBroken))
        {
            EventManager.HealthBarBroken.RemoveListener(OnInvokeHealthBarBroken);
        }
        if (Listeners.Contains(EventManager.ProjectileCreated))
        {
            EventManager.ProjectileCreated.RemoveListener(OnInvokeProjectileCreated);
        }
        if (Listeners.Contains(EventManager.UnitStatCurrentAmountChanged))
        {
            EventManager.UnitStatCurrentAmountChanged.RemoveListener(OnInvokeUnitStatCurrentAmountChanged);
        }
        if (Listeners.Contains(EventManager.EffectStarted))
        {
            EventManager.EffectStarted.RemoveListener(OnInvokeEffectStarted);
        }
        if (Listeners.Contains(EventManager.EffectEmpowered))
        {
            EventManager.EffectEmpowered.RemoveListener(OnInvokeEffectEmpowered);
        }
        if (Listeners.Contains(EventManager.EffectDecayingAmountChanged))
        {
            EventManager.EffectDecayingAmountChanged.RemoveListener(OnInvokeEffectDecayingAmountChanged);
        }
        if (Listeners.Contains(EventManager.EffectEnded))
        {
            EventManager.EffectEnded.RemoveListener(OnInvokeEffectEnded);
        }
        if (Listeners.Contains(EventManager.AboutToAddCooldown))
        {
            EventManager.AboutToAddCooldown.RemoveListener(OnInvokeAboutToAddCooldown);
        }
        if (Listeners.Contains(EventManager.CooldownAdded))
        {
            EventManager.CooldownAdded.RemoveListener(OnInvokeCooldownAdded);
        }
        if (Listeners.Contains(EventManager.ItemEquipped))
        {
            EventManager.ItemEquipped.RemoveListener(OnInvokeItemEquipped);
        }
        if (Listeners.Contains(EventManager.StanceSwitched))
        {
            EventManager.StanceSwitched.RemoveListener(OnInvokeStanceSwitched);
        }
        if (Listeners.Contains(EventManager.ExitCombat))
        {
            EventManager.ExitCombat.RemoveListener(OnInvokeExitCombat);
        }
        if (Listeners.Contains(EventManager.EnterCombat))
        {
            EventManager.EnterCombat.RemoveListener(OnInvokeEnterCombat);
        }
    }

    public virtual void OnInvokeOneTenthSecondElapsedInGame()
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeOneTenthSecondElapsedRealtime()
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeHitDealt(Damage damage)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAfterHitDamageCalculation(Damage damage)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAboutToHandleFatalBlow(Damage damage)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }


    public virtual void OnInvokeDamageDealt(Damage damage)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeDamageWasDodged(Damage damage, Ability dodge)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAbilityUsed(Ability ability)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && ability.TriggeredEffects.Contains(this) == false)
        {
            ability.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAbilityEnded(Ability ability)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && ability.TriggeredEffects.Contains(this) == false)
        {
            ability.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAbilityEnergyConsumed(Ability ability, float amount)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && ability.TriggeredEffects.Contains(this) == false)
        {
            ability.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeUnitKnockedOut(Damage damage)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAmmoAmountChanged()
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeHealthBarBroken(Damage damage)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeProjectileCreated(Projectile projectile)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && projectile.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            projectile.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount)
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeEffectStarted(Effect effect)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeEffectEmpowered(Effect existing_effect, Effect new_effect)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && existing_effect.SourceOfEffect.SourceAbility != null && existing_effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            existing_effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeEffectDecayingAmountChanged(Effect effect)
    {
        EventManager.EffectDecayingAmountChanged.Invoke(this);
        if (TriggersOncePerAbility && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeEffectEnded(Effect effect)
    {
        EventManager.EffectActivated.Invoke(this);
        if (TriggersOncePerAbility && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.TriggeredEffects.Contains(this) == false)
        {
            effect.SourceOfEffect.SourceAbility.TriggeredEffects.Add(this);
        }
    }

    public virtual void OnInvokeAboutToAddCooldown(Cooldown cooldown)
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeCooldownAdded(Cooldown cooldown)
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeItemEquipped(Item item1, Item item2)
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeStanceSwitched()
    {
        EventManager.EffectActivated.Invoke(this);
    }

    public virtual void OnInvokeExitCombat(Unit unit)
    {
        EventManager.EffectActivated.Invoke(this);
    } 
    
    public virtual void OnInvokeEnterCombat(Unit unit) {
        EventManager.EffectActivated.Invoke(this);
    } 
}