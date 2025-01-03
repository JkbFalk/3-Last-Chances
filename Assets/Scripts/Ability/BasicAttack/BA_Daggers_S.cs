using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BA_Daggers_S : BasicAttack {

    public static Unit ValidTarget;
    public BA_Daggers_S(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(250 / 2, 350 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        AddCustomSound("Teleport1", "Ability/Light Sword Swing 10", 0.7f);
        AddCustomSound("Teleport2", "Footsteps/Footsteps_Gravel10", 0.8f);
    }

    public override void CallAbilityEvent1()
    {
        Player.Instance.transform.position = ValidTarget.Actions.IsFlipped ? (ValidTarget.transform.position + new Vector3(2, 0, 0)) : (ValidTarget.transform.position + new Vector3(-2, 0, 0));
        Player.Instance.Actions.IsFlipped = ValidTarget.Actions.IsFlipped;
    }

    public static bool CheckIfAnyValidTargetInRange() {
        Vector2 check = Player.Instance.Actions.IsFlipped ? (Player.Instance.transform.position + new Vector3(-3, 0)) : (Player.Instance.transform.position + new Vector3(3, 0));
        if(Player.Instance.CurrentTarget != null) {
            ValidTarget = Player.Instance.CurrentTarget;
            return Vector2.Distance(check, Player.Instance.CurrentTarget.transform.position) < 5;
        }
        List<Unit> enemies = Utils.GetAllUnits(true, true).OrderBy(u => Vector2.Distance(u.transform.position, check)).ToList();
        ValidTarget = enemies.Count > 0 ? enemies[0] : null;
        return enemies != null && enemies.Count > 0 && Vector2.Distance(enemies[0].transform.position, check) < 5;
    }
}