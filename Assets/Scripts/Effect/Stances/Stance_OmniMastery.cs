using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Stance_OmniMastery : Effect_Stance
{
    public static float DamageBuffDuration = 5;
    public static float DamageBuffAmount = 50;

    public static float CleanseCooldownDuration = 15;
    private Effect_Empowered _damageBuff;

    private Image _damageGauge;
    private Image _cleanseGauge;

    public Stance_OmniMastery(SourceOfEffect source_of_effect) : base(source_of_effect) {
        TriggersOncePerAbility = true;
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnStart()
    {
        base.OnStart();
        EventManager.StanceSwitched.AddListener(Activate);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        EventManager.StanceSwitched.RemoveListener(Activate);
    }

    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Anima;

    public override void OnInvokeEffectStarted(Effect e) {
        if(Player.Instance.CurrentStance.StanceEffect == e) {
            Activate();
        }
    }

    public void Activate()
    {
        if(_damageBuff != null && !IsActive && Player.Instance.CurrentEffects.Contains(_damageBuff)) {
            _damageBuff.EndThisEffect();
        }
        else if(IsActive){
            _damageBuff = new Effect_Empowered(DamageBuffAmount, SourceOfEffect) {
                ShowsInUI = false, 
                ShowsInMenu = false
            };
            Player.Instance.AddEffect(_damageBuff, DamageBuffDuration);
        }
        Cooldown cd = Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Type == typeof(Stance_OmniMastery) && cd.Identifier == AssignedStance.WeaponType.ToString());
        if(IsActive && UnlockedUpgrade1 && cd == null) {
            if(_cleanseGauge == null) {
                CreateStanceDisplay();
            }
            _cleanseGauge.GetComponent<Image>().fillAmount = 0;
        }
        if(IsActive && UnlockedUpgrade2) {
            Ability ba = (BasicAttack)Activator.CreateInstance(typeof(BA_OmniMastery), new object[] { Player.Instance });
            Player.Instance.Actions.CurrentAbilityBeingPerformed = ba;
        }
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(IsActive&& UnlockedUpgrade3 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)  && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false ) {
            List<Type> affected_abilities = new List<Type>();
            List<Type> unaffectable_abilities = new List<Type>();
            foreach(Stance.EquippedAbility a in Player.Instance.CurrentStance.Abilities) {
                unaffectable_abilities.Add(a.Type);
            }
            for(int i = 0; i < 3; i++) {
                foreach(Stance.EquippedAbility a in SaveFile.Instance.Stances[i].Abilities) {
                    if(!unaffectable_abilities.Contains(a.Type) && !affected_abilities.Contains(a.Type) && Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == a.Type) != null) {
                        Cooldown cd = Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == a.Type);
                        cd.RemainingDuration = cd.RemainingDuration * 0.75f;
                        affected_abilities.Add(a.Type);
                    }
                }
            }
            base.OnInvokeDamageDealt(damage);
        }
    }


    public override void CreateStanceDisplay() {
        base.CreateStanceDisplay();
        _damageGauge = Player.Instance.CurrentStanceGauge.transform.Find("Damage/Fill").GetComponent<Image>();
        if(UnlockedUpgrade1 == false) {
            Player.Instance.CurrentStanceGauge.transform.Find("Cleanse").gameObject.SetActive(false);
        }
        else {
            _cleanseGauge = Player.Instance.CurrentStanceGauge.transform.Find("Cleanse/Fill").GetComponent<Image>();
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if(_damageGauge != null && IsActive && _damageBuff != null && _damageBuff.RemainingDuration > 0) {
            _damageGauge.GetComponent<Image>().fillAmount = 1 - _damageBuff.RemainingDuration / _damageBuff.BaseDuration;
        }
        Cooldown cd = Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Type == typeof(Stance_OmniMastery) && cd.Identifier == AssignedStance.WeaponType.ToString());
        if(_cleanseGauge != null && IsActive && UnlockedUpgrade1 && cd != null) {
            _cleanseGauge.GetComponent<Image>().fillAmount = cd.RemainingDuration / cd.TotalDuration;
        }
    }
}
