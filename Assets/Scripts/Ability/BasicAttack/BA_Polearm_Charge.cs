using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BA_Polearm_Charge : BasicAttack {

    public List<Unit> SkeweredEnemies = new();
    public bool IsDashing = false;
    private int _powerLevel = 0;
    public BA_Polearm_Charge(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.StrongBasicAttack);
        AddCustomSound("Charge", "Ability/Ability_ChargedShot_Charge", 0.5f);
        DamageSources.Add(new DamageSource(200, 200, Constants.DamageType.Heavy));
        TransitionIntoAnimationDuration = 0;
        EffectsAffectingUserDuringAbility = new() {new Effect_Immovable(new(this)), new Effect_Unstunnable(new(this))};
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public override void CallAbilityEvent1()
    {
        if(!Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithChargeAndImproveDamage") || User.Energy.Current < 2 || PlayerControls.BasicAttackButtonHoldDuration == 0) {
            User.PlayAnimation("Polearm_Charge", 0f, 0.52f);
        }
        else {
            User.Energy.Current -= 2;
            _powerLevel++;
        }
    }

    public override void CallAbilityEvent2()
    {
        User.Actions.TurnOnWeaponCollision("Heavy");
        for(int i = 0; i < 5 + _powerLevel; i++) {
            GameController.Instance.WaitAndRunMethod((i * 0.04f) / User.HeavyAttackSpeed.Current, PushUser, 200 + _powerLevel * 6 - i * 12 );
        }
        IsDashing = true;
        if(_powerLevel > 0) {
            GameController.Instance.WaitAndRunMethod(0.5f / User.HeavyAttackSpeed.Current, PlayExtendedAnimation);
        }        
        GameController.Instance.WaitAndRunMethod((0.5f + _powerLevel * 0.04f) / User.HeavyAttackSpeed.Current, StopDashing);
    }

    public void PlayExtendedAnimation() {
        User.PlayAnimation("Polearm_Charge_Extended", 0.05f);
    }

    public void PushUser(int force) {
        if(force > 0) {
            User.Rigidbody2D.AddForce((User.Actions.IsFlipped ? Vector2.left : Vector2.right) * force, ForceMode2D.Force);
        }
    }

    public void StopDashing()
    {
        User.PlayAnimation("Polearm_Charge", 0.05f, 0.77f);
        IsDashing = false;
        foreach(Unit u in SkeweredEnemies) {
            u.GetEffect(typeof(Effect_KnockedBack)).EndThisEffect();
        }
        SkeweredEnemies.Clear();
    }

    public override void AdditionalActionsOnUpdate()
    {
        if(SkeweredEnemies.Count > 0) {
            foreach(Unit u in SkeweredEnemies) {
                u.PushIntoPosition(User.Actions.IsFlipped ? (User.transform.position + new Vector3(-2 - SkeweredEnemies.IndexOf(u) * 0.3f, 0)) : (User.transform.position + new Vector3(2 + SkeweredEnemies.IndexOf(u) * 0.3f, 0)), this);
            }
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        SkeweredEnemies.Add(damage.TargetOfDamage);
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        if(_powerLevel > 0) {
            damage.Stagger += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 3f * (_powerLevel / 20);
            damage.Injury += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 3f * (_powerLevel / 20);
        }
    }
}