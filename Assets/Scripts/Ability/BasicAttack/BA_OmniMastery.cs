using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BA_OmniMastery : BasicAttack {

    public static bool CanBeUsedDuringOtherAbilities = true;

    public BA_OmniMastery(Unit ability_user) : base(ability_user) {
        NameOfAnimationToAutoPlay = "OmniMastery_" + ability_user.CurrentWeaponClass;
        AddCustomSound("SwingGreatsword", "Greatsword/Greatsword_Swing20", 0.8f);
        AddCustomSound("SwingLongblade", "Greatsword/Greatsword_Swing20", 0.8f);
        AddCustomSound("SwingPolearm", "Polearm/Polearm_Swing2", 0.8f);
        AddCustomSound("SwingTwinBlades", "TwinBlades/TwinBlades_Swing5", 0.8f);
        AddCustomSound("SwingDaggers", "Daggers/Daggers_Swing1", 0.8f);
        AddCustomSound("SwingGun", "Generic/Generic_Swoosh4", 0.8f);
        AddCustomSound("SwingBow", "Generic/Generic_Swoosh4", 0.8f);
        AddCustomSound("SwingMagic", "Magic/Magic_Blast2", 0.8f);
        if(ability_user.CurrentWeaponDamageCategory == Constants.DamageType.Ranged) {
            DamageSources.Add(new DamageSource(200, 400, Constants.DamageType.Ranged, "AoE"));
        } else {
            DamageSources.Add(new DamageSource(200, 400, ability_user.CurrentWeaponDamageCategory));
        }
    }
}
