using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

public class NPCAbility_WildFireSerpents : Ability {
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public List<Projectile> Snakes;
    public int SnakeAmount;
    private int _cycle = 0;
    public NPCAbility_WildFireSerpents(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        Snakes = new();
        DamageSources.Add(new DamageSource(15, 10, Constants.DamageType.Magic));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        Properties.Add(Property.ImmuneToFlinch);
        AddCustomSound("Use", "Fire/Fire5", 0.65f);
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        SnakeAmount = GetSnakeAmount();
    }

    public override void CallAbilityEvent1()
    {
        List<Vector3> positions = new List<Vector3> {new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(User.Actions.IsFlipped ? 2 : -2, 0), new Vector2(User.Actions.IsFlipped ? -2 : 2, 0), new Vector2(0, 2), new Vector2(0, -2), new Vector2(0, 0)};
        for(int i = 0; i < SnakeAmount; i++) {
            Snakes.Add(Utils.CreateProjectile(new(this), "WildFireSerpent"));
            Snakes[i].transform.position = User.transform.position + positions[i];
            Snakes[i].CleanUpAfter(20);
        }
        GameController.Instance.WaitAndRunMethod(1, RemoveChangeTransform);
    }

    public void RemoveChangeTransform() {
        foreach(Projectile snake in Snakes) {
            if(snake != null) {
                MonoBehaviour.Destroy(snake.GetComponent<ChangeTransformOverTime>());
            }
        }
    }

    private int GetSnakeAmount() {
        return (User.HealthBars.Count > 1 && User.CurrentHealthBars == 1) ? (User.CurrentHealthPercentage < 25 ? 9 : User.CurrentHealthPercentage < 50 ? 8 : User.CurrentHealthPercentage < 75 ? 7 : 6) : (User.CurrentHealthPercentage < 25 ? 5 : User.CurrentHealthPercentage < 50 ? 4 : User.CurrentHealthPercentage < 75 ? 3 : 2);
    }

    public override void CallAbilityEvent2()
    {
        for(int i = 0; i < Snakes.Count; i++) {
            Snakes[i].DealingDamage = true;
            Snakes[i].IsFlying = true;
            ParticleSystem.EmissionModule emission2 = Snakes[i].GetComponent<ParticleSystem>().emission;
            emission2.rateOverTime = 50;
            ChangeToRandomDirection(Snakes[i].gameObject);
        }
        AddCollider();
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(Snakes.Count == 0 || Snakes[0] == null || Snakes[0].gameObject == null || Snakes[0].gameObject.IsDestroyed()) {
            return;
        }
        else {
            base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(2 * User.MagicStagger.Current / 100, new(this)));
    }

    public void AddCollider() {
        if(Snakes.Count > 0 && Snakes[0] != null && Snakes[0].gameObject != null && !Snakes[0].gameObject.IsDestroyed()) {
            for(int i = 0; i < Snakes.Count && Snakes[i].gameObject != null && Snakes[i].gameObject.IsDestroyed() == false; i++) {
                AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "FlameTrailCollider");
                aoe.transform.position = Snakes[i].transform.position;
                aoe.transform.localScale = new Vector2(0.6f, 0.6f);
            }
            _cycle++;
            if(_cycle == 10) {
                _cycle = 0;
                ResetPotentialTargets();
            }
            GameController.Instance.WaitAndRunMethod(0.1f, AddCollider);
        }
    }

    public void ChangeToRandomDirection(GameObject snake) {
        if(snake != null && snake.gameObject != null && !snake.gameObject.IsDestroyed()) {
            if (NavMesh.SamplePosition((Vector2)snake.transform.position + UnityEngine.Random.insideUnitCircle * new Vector2(4f, 4f), out NavMeshHit repositionHit, 2f, NavMesh.AllAreas))
            {
                snake.transform.up = (repositionHit.position - snake.transform.position).normalized;
            }
            GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(0.3F, 2.2f), ChangeToRandomDirection, snake);
        }
    }
}