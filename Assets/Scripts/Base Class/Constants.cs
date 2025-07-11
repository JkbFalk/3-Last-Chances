using System;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    public const float DEFAULT_BLACK_SCREEN_TRANSITION_DURATION = 2f;
    public const float DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS = 30;
    public const float DEFAULT_STACKING_EFFECT_DECAY_PER_SECOND = 0.05f;
    public const float INJURY_PERCENTAGE_FROM_RIPOSTE = 150;
    public const float STAGGER_PERCENTAGE_FROM_RIPOSTE = 500;
    public const float INJURY_PERCENTAGE_FROM_PROJECTILE_RIPOSTE = 200;
    public const float STAGGER_PERCENTAGE_FROM_PROJECTILE_RIPOSTE = 600;
    public const float INJURY_PERCENTAGE_FROM_COUNTER = 300;
    public const float STAGGER_PERCENTAGE_FROM_COUNTER = 1000;
    public const float DISTANCE_AWAY_FROM_COUNTERING_UNIT = 1.8f;
    public const float STANCE_SWITCH_ROTATE_TIME = 0.5f;
    public const float ENERGY_FROM_RIPOSTING = 10f;
    public const float ENERGY_FROM_COUNTERING = 20f;
    public const float ENERGY_FROM_BASIC_ATTACK = 4f;
    public const float ENERGY_PER_STAGGER_PERCENTAGE_LOST_FROM_BLOCKING = 1.2f;
    public const float ENERGY_PER_HEALTH_PERCENTAGE_LOST = 1f;
    public const float ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES = 1.0f;
    public const float ENERGY_FROM_DODGING = 8f;
    public const float ENERGY_FROM_INFLICTING_STAGGERED = 10f;
    public const float DEGREES_PER_RAD = 57.2958f;
    public const int ACTION_QUEUE_DURATION = 40;
    public const float DEFAULT_CROSSFADE_DURATION = 0.1f;
    public const float PLAYER_RUN_SPEED = 6f;
    public const float PLAYER_WALK_SPEED = 3f;
    public const float PLAYER_BLOCK_MOVE_SPEED = 2f;
    public const float DEFAULT_ENEMY_SPEED = 2f;
    public const float DEFAULT_ALLY_SPEED = 4f;
    public const float DEFAULT_FLINCHING_DURATION = 0.5f;
    public const float DEFAULT_PLAYER_STAGGERED_DURATION = 15f;
    public const float DEFAULT_SOFT_STAGGERED_DURATION = 3f;
    public const float DEFAULT_HARD_STAGGERED_DURATION = 12f;
    public const int DEFAULT_FIXED_FRAMES_UNTIL_EXITING_COMBAT = 500;
    public const float STANCE_SWITCH_COOLDOWN = 2f;
    public const float STANCE_SWITCH_COOLDOWN_OMNIMASTERY = 0.5f;
    public const float EXECUTE_DAMAGE_AMOUNT = 9999999;
    public const int MAX_DIALOGUE_SPEED = 50;
    public const int MELEE_HITSTOP_DURATION_IN_FIXED_FRAMES = 4;
    public const float SCOUT_ZOOM_SPEED = 0.15f;
    public const float SCOUT_RETURN_SPEED = 0.35f;
    public static readonly string[] VOWELS = {"A", "I", "E", "O", "U"};
    public static float NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_TIMER = 0.2f;
    public static float NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_SIZE = 2;
    public static float FOLLOW_UP_HEALTH_BAR_FREEZE_TIME = 1f;
    public static float FOLLOW_UP_HEALTH_BAR_DECREASE_SPEED = 0.02f;
    public static float MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS = 0.2f;
    public static float FORCE_REQUIRED_TO_PUSH_1M = 100;

    public const float PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_REGULAR_FLINCH = 40;
    public const float PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_BOSS_FLINCH = 30;
    public const float PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_PLAYER_FLINCH = 20;
    public const float ENERGY_REQUIRED_TO_USE_ULTIMATE = 0;

    public const float SECONDS_UNTIL_DIALOGUE_CHOICES_BECOME_CLICKABLE = 0.75f;
    public const float MAXIMUM_AMOUNT_OF_SAVE_FILES = 20;
    public const float FULLY_RESTED_INITIAL_ENERGY = 20;
    public const int FULLY_RESTED_INITIAL_AMMO = 6;
    public const int MAX_ULTIMATE_USES_POSSIBLE = 7;
    public const int PRONE_TO_KNOCKOUT_AMOUNT_ADDED_BY_GAUNTLET_BA = 40;

    public const float EXPECTED_POWER_AT_LEVEL_1 = 1.25f;
    public const float EXPECTED_POWER_AT_LEVEL_10 = 2.2f;
    public const float EXPECTED_POWER_AT_LEVEL_20 = 3.1f;
    public const float EXPECTED_POWER_AT_LEVEL_30 = 4.2f;
    public const float EXPECTED_POWER_AT_LEVEL_40 = 5.2f;
    public const float EXPECTED_POWER_AT_LEVEL_50 = 6f;
    public const float EXPECTED_POWER_AT_LEVEL_100 = 8f;

    public const float EXPERIENCE_MULTIPLIER_AT_LEVEL_10 = 2f;
    public const float EXPERIENCE_MULTIPLIER_AT_LEVEL_20 = 4f;
    public const float EXPERIENCE_MULTIPLIER_AT_LEVEL_30 = 6f;
    public const float EXPERIENCE_MULTIPLIER_AT_LEVEL_40 = 8f;
    public const float EXPERIENCE_MULTIPLIER_AT_LEVEL_50 = 10f;
    public const float EXPERIENCE_MULTIPLIER_AT_LEVEL_100 = 15f;

    public const float CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_10 = 1.15f;
    public const float CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_20 = 1.3f;
    public const float CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_30 = 1.45f;
    public const float CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_40 = 1.6f;
    public const float CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_50 = 1.75f;
    public const float CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_100 = 2f;

    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_TO_EXCELLENT = 1;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_TO_MASTERFUL = 3;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_TO_FLAWLESS = 6;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_TO_ULTIMATE = 10;

    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_2 = 2;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_3 = 4;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_4 = 6;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_5 = 8;
    public const int TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_6 = 10;

    public enum HitSoundTypeEnum {LargeBlunt, LongSharp,  SmallBlunt, SmallSharp, Magic, Fire, Ice, Arrow, Bullet, Neutral, Thunder, Shadow, Earth}
    
    public enum EnergyGainSource { BasicAttack, Dodge, Block, Riposte, HealthLost, InflictedStaggered, Counter }

    public enum Difficulty {Story, Regular, Challenge, Ultimate };

    public enum WeaponClass { TwinBlades, Gun, Bow, Polearm, Daggers, Greatsword, Hammer, Axe, Longblade, Cannon, Shield, None, Magic, Gauntlets, Fire, Ice };

    public enum DamageType { Heavy, Light, Ranged, Magic, CurrentWeapon, None };

    public enum ItemType { Heavy, Light, Ranged, Helmet, Outfit, Gloves, Boots, Tool, Quest, None }

    public enum StatSource { Base, PowerUp, Item, Buff }

    public enum AttackType { None, Strong, Fast };

    public enum ActionType { Idle, Moving, UsingAbility, InCutscene, UnderHardCrowdControl }

    public enum AIBehavior { ChasingCurrentTarget, Repositioning, Escaping, CirclingAround, Observing, UsingAbility, Waiting, None }

    public enum Faction { Ally, Enemy, Neutral, Monster, HostileToAll, DuelingEachOther };

    public enum GameplayMode { Regular, InMenu, InCutscene, Shopping, OnStartScreen, InfoPrompt, MissionSelect, Paused };

    public enum GameType { Challenge, Survival, Arena, Story, None };

    public static readonly Dictionary<Faction, Dictionary<Faction, int>> FactionAttitudes = new Dictionary<Faction, Dictionary<Faction, int>> {
        { Faction.Ally, new Dictionary<Faction, int>() {
                { Faction.Enemy, -1 },
                { Faction.Neutral, 0 },
                { Faction.Monster, -1 },
                { Faction.HostileToAll, -1 },
                { Faction.DuelingEachOther, 0 },
                { Faction.Ally, 1 }
            }
        },
        { Faction.Enemy, new Dictionary<Faction, int>() {
                { Faction.Ally, -1 },
                { Faction.Neutral, 0 },
                { Faction.Monster, -1 },
                { Faction.HostileToAll, -1 },
                { Faction.DuelingEachOther, 0 },
                { Faction.Enemy, 1 }
            }
        },
        { Faction.Neutral, new Dictionary<Faction, int>() {
                { Faction.Ally, 0 },
                { Faction.Enemy, 0 },
                { Faction.Monster, -1 },
                { Faction.HostileToAll, -1 },
                { Faction.DuelingEachOther, 0 },
                { Faction.Neutral, 1 }
            }
        },
        { Faction.Monster, new Dictionary<Faction, int>() {
                { Faction.Enemy, -1 },
                { Faction.Neutral, -1 },
                { Faction.Ally, -1 },
                { Faction.HostileToAll, -1 },
                { Faction.DuelingEachOther, -1 },
                { Faction.Monster, 1 }
            }
        },
        { Faction.HostileToAll, new Dictionary<Faction, int>() {
                { Faction.Enemy, -1 },
                { Faction.Neutral, -1 },
                { Faction.Ally, -1 },
                { Faction.Monster, -1 },
                { Faction.DuelingEachOther, -1 },
                { Faction.HostileToAll, -1 }
            }
        },
        { Faction.DuelingEachOther, new Dictionary<Faction, int>() {
                { Faction.Enemy, 0 },
                { Faction.Neutral, 0 },
                { Faction.Ally, 0 },
                { Faction.HostileToAll, -1 },
                { Faction.Monster, -1 },
                { Faction.DuelingEachOther, -1 }
            }
        },
    };

    public static List<string> RegularArenas_Small = new List<string>() { "Warehouse_Containers", "Warehouse_Storage", "Outside_GrassBackAlley", "Outside_RockBackAlley" };
    public static List<string> RegularArenas_Large = new List<string>() { "Warehouse_Library", "Warehouse_Barrels", "Warehouse_Corridor" };
    public static List<string> BossArenas = new List<string>() { "Warehouse_FightClub", "Warehouse_Arena", "Outside_GrassBackAlley", "Outside_RockBackAlley" };
    public static List<string> TimeAttackArenas = new List<string>() {"Warehouse_Corridor", "Warehouse_Library", "Warehouse_Barrels", "Warehouse_Arena" };
    public static List<string> DestructibleArenas = new List<string>() {"Warehouse_Pillars"};

    public static List<string> BossEnemies = new List<string> { "Unit_Ryker", "Unit_Clarise1", "Unit_Blaine",  "Unit_FlameShadow", "Unit_Colten", "Unit_WeaponPillager", "Unit_Maginhart", "Unit_Iris" };
    public static List<string> EliteEnemies = new List<string> {"Unit_ShieldGiant", "Unit_ExplosivesExpert", "Unit_SalutisAssassin",  "Unit_Berserker", "Unit_IgnisCaptain", "Unit_IgnisCannonier", "Unit_IgnisLancer", "Unit_IgnisSwordmaster", "Unit_IgnisAssassin", "Unit_AnimaBlademaster", "Unit_AnimaSpearmaster", "Unit_AnimaBowmaster"   };
    public static List<string> RegularEnemies = new List<string> { "Unit_Criminal_Spear", "Unit_Criminal_Shortbow", "Unit_Criminal_Hammer", "Unit_GraveRobber", "Unit_Criminal_Shield", "Unit_Criminal_Daggers", "Unit_Criminal_FreezeCaster", "Unit_Criminal_Grenadier", "Unit_FireElemental", "Unit_AnimatedArmor", "Unit_IgnisPyromancer", "Unit_IgnisKnight", "Unit_AnimatedGreataxe", "Unit_AnimatedBlades", "Unit_AnimatedBow", "Unit_Adventurer", "Unit_AnimaRookie", "Unit_AnimaGuardian" };
}