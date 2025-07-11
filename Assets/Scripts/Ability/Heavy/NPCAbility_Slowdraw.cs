using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class NPCAbility_Slowdraw: Ability {
    public static float Cooldown = 6;
    private Effect_ChangeStat _randomAS;
    private int _powerUps = 0;
    private bool _showedIndicator = false;

    public NPCAbility_Slowdraw(Unit ability_user) : base(ability_user) {
        AddCustomSound("Draw1", "Longblade/Longblade_Sheathe1", 1f);
        AddCustomSound("Draw2", "Longblade/Longblade_Unsheathe1", 1f);
        AddCustomSound("Indicator", "Ability/QuickdrawIndicator", 0.8f);
        _randomAS = new Effect_ChangeStat(User.HeavyAttackSpeed, new(this)) {PercentageAmount = UnityEngine.Random.Range(0, 15)};
        EffectsAffectingUserDuringAbility = new() {_randomAS};
    }

    public override void CallAbilityEvent1()
    {
        _powerUps++;
        if(_showedIndicator == false && (UnityEngine.Random.Range(0, 100) < 15 || _powerUps >= 10)) {
            Utils.CreateVisualEffect(new(this), "QuickdrawIndicator");
            User.Actions.PlayAbilityCustomSound("Indicator");
            GameController.Instance.WaitAndRunMethod(0.7f / User.HeavyAttackSpeed.Current, FastForward);
            _showedIndicator = true;
        }
    }

    public void FastForward() {
        User.PlayAnimation("Slowdraw", 0.05f, 0.70f);
    }

    public override void CallAbilityEvent2()
    {
        DamageSources.Add(new DamageSource(100 + _powerUps * 30, 50 + _powerUps * 15, User.DamageType == Constants.DamageType.Light ? Constants.DamageType.Light : Constants.DamageType.Heavy));
        ChaseCurrentTargetAtGivenDegreeAngle(3, 85);
    }
}