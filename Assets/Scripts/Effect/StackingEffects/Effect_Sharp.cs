using System.Collections.Generic;
using UnityEngine;

public class Effect_Sharp : Effect
{
    public int Stacks = 1;
    public const int MAX_STACKS = 5;
    public const float STAGGER_MULTIPLIER_BONUS_PER_STACK = 0.2f; 
    public GameObject Vfx;

    public Effect_Sharp(float amount, SourceOfEffect source_of_effect) : this(Mathf.Max(1, Mathf.RoundToInt(amount)), source_of_effect)
    {
    }

    public Effect_Sharp(int stacks_to_apply, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        UsesStacks = true;
        MaxStacks = MAX_STACKS;
        _currentStacks = Mathf.Clamp(stacks_to_apply, 1, MaxStacks); 
        ShowsInUI = true;
        
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount; 
        
        Listeners.Add(EventManager.HitDealt);
        Listeners.Add(EventManager.AfterHitDamageCalculation); 
    }
    
    public override void ExtraBehaviourOnStacksChanged(int oldStacks, int newStacks)
    {
        UIText = Utils.GetRomanNumeral(newStacks);

        // 1. SOUND FIX: Always play audio on stack increase, regardless of visual stage
        if (newStacks > oldStacks && TargetOfEffect != null)
        {
            if (newStacks < MAX_STACKS)
            {
                // Temporarily raise the player's audio pitch to build musical tension
                TargetOfEffect.AudioSource.pitch = 1.0f + (newStacks * 0.05f);
                Utils.PlaySoundEffect(TargetOfEffect.AudioSource, "Effect/Effect_Sharp", 0.6f);
                TargetOfEffect.AudioSource.pitch = 1.0f; // Reset immediately
            }
            else
            {
                // Max stack cinematic sound
                Utils.PlaySoundEffect(TargetOfEffect.AudioSource, "Hit/LongSharp_CriticalHit1", 1.5f);
            }
        }

        // 2. VISUAL FIX: Only update the VFX if the tier actually changes
        // Stage 1 = 1 Stack | Stage 2 = 2, 3, 4 Stacks | Stage 3 = 5 Stacks
        int newStage = newStacks == 1 ? 1 : newStacks <= 4 ? 2 : 3;
        
        if (StackingEffectIntensityLevel != newStage)
        {
            StackingEffectIntensityLevel = newStage;
            if (TargetOfEffect != null) 
            {
                AddVisualEffect();
            }
        }

        if (newStacks == MAX_STACKS && UICooldownDisplay != null) {
            UICooldownDisplay.color = Colors.GetColorFromCode(Colors.Emission); // Glowing UI at max
        }
    }

    public void AddVisualEffect() 
    {
        if (TargetOfEffect == null || !TargetOfEffect.SpriteRenderers.ContainsKey("Upper Body")) return;

        RemoveVFXs();
        Vfx = Utils.CreateVisualEffect(SourceOfEffect, "Sharp" + StackingEffectIntensityLevel);
        if (Vfx != null)
        {
            Vfx.gameObject.name = "VisualEffect_Sharp" + StackingEffectIntensityLevel;
            Vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
            Vfx.transform.localPosition = Vector2.zero;
            
            AudioSource vfxAudio = Vfx.GetComponent<AudioSource>();
            
            if (CurrentStacks < MAX_STACKS)
            {
                vfxAudio.pitch = 1.0f + (CurrentStacks * 0.05f);
                Utils.PlaySoundEffect(vfxAudio, "Effect/Effect_Sharp_Build", 0.6f); 
            }
            else
            {
                vfxAudio.pitch = 1.0f;
                Utils.PlaySoundEffect(vfxAudio, "Effect/Effect_Sharp_Max", 1.0f); // Louder!
            }
        }
    }

    public void RemoveVFXs() 
    {
        // Safety check
        if (TargetOfEffect == null || !TargetOfEffect.SpriteRenderers.ContainsKey("Upper Body")) return;

        for (int i = 1; i <= 3; i++) 
        {
            Transform existingVfx = TargetOfEffect.SpriteRenderers["Upper Body"]?.Bone?.transform.Find("VisualEffect_Sharp" + i);
            if (existingVfx != null) 
            {
                MonoBehaviour.Destroy(existingVfx.gameObject);
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = 15f;
        ExtraBehaviourOnStacksChanged(0, CurrentStacks); 
    }

    public override void OnEnd()
    {
        base.OnEnd();
        RemoveVFXs();
    }

    public override bool CheckIfEffectAlreadyAppliedAndHandleBehaviour()
    {
        Effect_Sharp existingSharp = (Effect_Sharp)TargetOfEffect.GetEffect(typeof(Effect_Sharp));
        if (existingSharp != null)
        {
            existingSharp.CurrentStacks += this.CurrentStacks; // Relies on the Clamp in CurrentStacks setter
            existingSharp.RemainingDuration = existingSharp.BaseDuration; // Refresh duration
            return true; 
        }
        return false;
    }

    public override void OnInvokeHitDealt(DamageInstance damage) 
    {
        if (damage?.SourceOfDamage?.User == TargetOfEffect && 
           (damage.SourceOfDamage.Is(Ability.Property.Riposte) || damage.SourceOfDamage.Is(Ability.Property.Counter))) 
        {
            damage.StaggerDealtMultiplier += (CurrentStacks * STAGGER_MULTIPLIER_BONUS_PER_STACK);
            base.OnInvokeHitDealt(damage);
        }
    }
}