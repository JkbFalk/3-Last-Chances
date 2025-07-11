using UnityEngine;

public class NPCAbility_FeintCombo : Ability {
    public static float Cooldown = 6;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public NPCAbility_FeintCombo(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.3f;
        AddCustomSound("Swing1", "Fire/Fire7", 0.5f);
        AddCustomSound("Swing2", "Fire/Fire9", 0.5f);
        AddCustomSound("Swing3", "Fire/Fire12", 0.5f);
        AddCustomSound("Ignite", "Fire/Fire3", 0.7f);
        AddCustomSound("Explosion1", "Fire/FireExplosion1", 0.8f);
        AddCustomSound("Explosion2", "Fire/FireExplosion2", 0.8f);
        Properties.Add(Property.ImmuneToFlinch);
        float random = UnityEngine.Random.Range(1, 100);
        if (random < 25)
        {
            DamageSources.Add(new DamageSource(125, 0, Constants.DamageType.Heavy));
            DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Heavy, "2"));
            DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Heavy, "3"));
            DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Magic, "AoE"));
        }
        else if(random < 50)
        {
            DamageSources.Add(new DamageSource(150, 350, Constants.DamageType.Heavy));
            NameOfAnimationToAutoPlay = "FeintCombo_Thrust";
            Properties.Add(Property.CounteredByRoll);
        }
        else if(random < 75)
        {
            DamageSources.Add(new DamageSource(125, 0, Constants.DamageType.Heavy));
            DamageSources.Add(new DamageSource(100, 450, Constants.DamageType.Heavy, "2"));
            NameOfAnimationToAutoPlay = "FeintCombo_Sweep";
        }
        else
        {
            DamageSources.Add(new DamageSource(125, 0, Constants.DamageType.Heavy));
            DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Heavy, "2"));
            DamageSources.Add(new DamageSource(50, 700, Constants.DamageType.Heavy, "3"));
            DamageSources.Add(new DamageSource(250, 0, Constants.DamageType.Magic, "AoE"));
            NameOfAnimationToAutoPlay = "FeintCombo_Slam";
        }
    }
}