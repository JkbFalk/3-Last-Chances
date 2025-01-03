using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_RainOfArrows : Ability {
    public static float Cooldown = 6;

    public NPCAbility_RainOfArrows(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 200, Constants.DamageType.Ranged));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Release4", 0.9f);
        Projectile proj1 = Utils.CreateProjectile(new(this), "BowmasterArrowFake");
        proj1.DealingDamage = false;
        proj1.transform.eulerAngles = new Vector3(0, 0, 0);
        Projectile proj2 = Utils.CreateProjectile(new(this), "BowmasterArrowFake");
        proj2.DealingDamage = false;
        proj2.transform.eulerAngles = new Vector3(0, 0, 0);
        proj2.transform.Rotate(new Vector3(0, 0, -4f));
        Projectile proj3 = Utils.CreateProjectile(new(this), "BowmasterArrowFake");
        proj3.DealingDamage = false;
        proj3.transform.eulerAngles = new Vector3(0, 0, 0);
        proj3.transform.Rotate(new Vector3(0, 0, 4f));
        for(int i = 0; i < 50; i++) {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "ArrowStrikeTheGround", Player.Instance.transform.position.x + UnityEngine.Random.Range(-5f, 5f), Player.Instance.transform.position.y + UnityEngine.Random.Range(-5f, 5f));
            aoe.transform.parent.gameObject.SetActive(false);
            GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(0, 1f), SetArrowActive, aoe.transform.parent.gameObject);
        }
    }

    public void SetArrowActive(GameObject arrow) {
        arrow.SetActive(true);
    }
}