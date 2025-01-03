using UnityEngine;

public class NPCAbility_ArrowRay : Ability {
    public static float Cooldown = 6;

    public NPCAbility_ArrowRay(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(50, 100, Constants.DamageType.Ranged));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Release4", 0.9f);
        Projectile proj1 = Utils.CreateProjectile(new(this), "BowmasterArrowFake");
        proj1.DealingDamage = false;
        proj1.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public override void CallAbilityEvent2()
    {
        for(int i = 0; i < 20; i++) {
            GameController.Instance.WaitAndRunMethod(0.2f * i, CreateArrowAoE);
        }
    }

    public void CreateArrowAoE() {
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "ArrowStrikeTheGround", Player.Instance.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f), Player.Instance.transform.position.y + UnityEngine.Random.Range(-0.7f, -0.3f));
    }
}