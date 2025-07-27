using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_Quickdraw: Ability {
    public static float Cooldown = 6;
    private Effect_ChangeStat _randomAS;
    private int _powerUps = 0;
    private bool _showedIndicator = false;

    public NPCAbility_Quickdraw(Unit ability_user) : base(ability_user) {
        AddCustomSound("Draw1", "Longblade/Longblade_Sheathe1", 1f);
        AddCustomSound("Draw2", "Ability/Ability_DeathSentence_Swing", 1f);
        AddCustomSound("Indicator", "Ability/QuickdrawIndicator", 0.8f);
        AddCustomSound("Wind", "Ability/Ability_WindBlast_Dash", 0.8f);
        _randomAS = new Effect_ChangeStat(User.HeavyAttackSpeed, new(this)) {PercentageAmount = UnityEngine.Random.Range(0, 25)};
        EffectsAffectingUserDuringAbility = new() {_randomAS};
    }

    public override void CallAbilityEvent1()
    {
        _powerUps++;
        if(_showedIndicator == false && (UnityEngine.Random.Range(0, 100) < 15 || _powerUps >= 10)) {
            Utils.CreateVisualEffect(new(this), "QuickdrawIndicator");
            User.Actions.PlayAbilityCustomSound("Indicator");
            GameController.Instance.WaitAndRunMethod(0.55f / User.HeavyAttackSpeed.Current, FastForward);
            _showedIndicator = true;
        }
    }

    public void FastForward() {
        User.PlayAnimation("Quickdraw", 0.05f, 0.70f);
    }

    public override void CallAbilityEvent2()
    {
        DamageSources.Add(new DamageSource(50 + _powerUps * 20, 200 + _powerUps * 60, Constants.DamageType.Heavy));
        ChaseCurrentTargetAtGivenDegreeAngle(3, 75);
    }
}