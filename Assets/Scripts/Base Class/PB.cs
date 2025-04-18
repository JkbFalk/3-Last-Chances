using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB
{
    // Total PB Values
    public const float EXPECTED_TOTAL_PB_FROM_ITEMS_AT_LEVEL_50 = 21 * MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER5_PB; //525
    public const float EXPECTED_TOTAL_PB_FROM_SKILL_TREE_AT_LEVEL_50 = 22 * ENERGY_LEVEL_3_MULTIPLIER * SPECIALITY_SKILL_TREE_PB + 22 * ENERGY_LEVEL_2_MULTIPLIER * SPECIALITY_SKILL_TREE_PB + 5 * ENERGY_LEVEL_1_MULTIPLIER * SPECIALITY_SKILL_TREE_PB + ENERGY_LEVEL_3_MULTIPLIER * ROW12_HEAL_POWER_UP_PB; // 574 + 40 = 614
    public const float EXPECTED_TOTAL_PB_FROM_STANCE_AT_LEVEL_50 = 200; // 70 base + 40 upg1 + 40 upg2 + 70 upg3
    public const float EXPECTED_TOTAL_PB_FROM_PERMANENT_POWER_UPS_AT_LEVEL_50 = 3 * 5 * MISSION_POWER_UPS_PB_BUDGET; //225
    //1564 Total at lvl 50


    // Skill Tree Node PB Values
    public const float NON_SPECIALITY_SKILL_TREE_PB = 8; // Final Intended is 5
    public const float SPECIALITY_SKILL_TREE_PB = 10; // Final Intended is 7
    public const float ROW12_HEAL_POWER_UP_PB = 20;
    public const float ENERGY_LEVEL_1_MULTIPLIER = 1.2f;
    public const float ENERGY_LEVEL_2_MULTIPLIER = 1.5f;
    public const float ENERGY_LEVEL_3_MULTIPLIER = 2.0f;
    public const float MISSION_POWER_UPS_PB_BUDGET = 15;


    // Item PB Values
    public const float MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER1_PB = 20;
    public const float MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER1_PB = 0;
    public const float MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER1_PB = 10;
    public const float MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER1_PB = 0;
    public const float MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER2_PB = 40;
    public const float MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER2_PB = 0;
    public const float MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER2_PB = 20;
    public const float MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER2_PB = 0;
    public const float MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER3_PB = 50;
    public const float MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER3_PB = 20;
    public const float MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER3_PB = 25;
    public const float MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER3_PB = 10;
    public const float MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER4_PB = 70;
    public const float MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER4_PB = 35;
    public const float MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER4_PB = 35;
    public const float MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER4_PB = 17.5f;
    public const float MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER5_PB = 100;
    public const float MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER5_PB = 50;
    public const float MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER5_PB = 50;
    public const float MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER5_PB = 25;


    //Multipliers for only affecting certain subset
    public const float SPECIALIZATION__DODGES_RIPOSTE_COUNTERS = 2.2f;
    public const float SPECIALIZATION__RIPOSTES_COUNTERS = 3f;
    public const float SPECIALIZATION__RIPOSTES = 5f;
    public const float SPECIALIZATION__COUNTERS = 5f;
    public const float SPECIALIZATION__WEAPONS = 1.2f;
    public const float SPECIALIZATION__MAGIC = 1.2f;
    public const float SPECIALIZATION__EVERYTHING_EXCEPT_TECHNIQUES = 1.2f;
    public const float SPECIALIZATION__TECHNIQUES = 1.55f;
    public const float SPECIALIZATION__ULTIMATE_TECHNIQUES = 3.25f;
    public const float SPECIALIZATION__FAMILY_TECHNIQUES = 1.85f;
    public const float SPECIALIZATION__COOLDOWN_REDUCTION_FOR_TECHNIQUES = 1.5f;
    public const float SPECIALIZATION__COOLDOWN_REDUCTION_FOR_TOOLS = 2.1f;
    public const float SPECIALIZATION__COOLDOWN_REDUCTION_FOR_EFFECTS = 2.25f;
    public const float SPECIALIZATION__STRONG_BASIC_ATTACK = 3.5f;
    public const float SPECIALIZATION__BASIC_ATTACK = 1.75f;
    public const float SPECIALIZATION__BACKSTAB = 3f;
    public const float SPECIALIZATION__INJURY = 1.4f;
    public const float SPECIALIZATION__STAGGER = 1.4f;
    public const float SPECIALIZATION__WEAPON_TYPE = 1.4f;
    public const float SPECIALIZATION__AGAINST_COUNTERABLE = 3f;
    public const float SPECIALIZATION__AGAINST_UNSTOPPABLE = 3f;
    public const float SPECIALIZATION__AGAINST_STAGGERED = 2.5f;
    public const float SPECIALIZATION__AGAINST_FROZEN = 3.25f;
    public const float SPECIALIZATION__AGAINST_CROWD_CONTROLLED = 2.5f;
    public const float SPECIALIZATION__AGAINST_NON_STAGGERED = 1.75f;
    public const float SPECIALIZATION__AGAINST_BOSSES = 2f;
    public const float SPECIALIZATION__AGAINST_NON_BOSSES = 2f;
    public const float SPECIALIZATION__PLAYER_STACKING_EFFECT_DAMAGE = 5f;
    public const float SPECIALIZATION__ONE_STANCE = 1.5f;
    public const float SPECIALIZATION__FINAL_AMMO = 5.5f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_HEALTH_LOST = 3.5f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_BLOCKING = 4f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_BASIC_ATTACKS = 2.5f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_DODGING = 6f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_RIPOSTING = 10f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_COUNTERING = 10f;
    public const float SPECIALIZATION__ENERGY_GAIN_FROM_STAGGERING = 8f;
    public const float SPECIALIZATION__SUPERCHARGE_DAMAGE = 5.0f;
    public const float SPECIALIZATION__BURN_EXPLOSION_DAMAGE = 8f;
    public const float SPECIALIZATION__ENEMY_STACKING_EFFECT_DAMAGE = 20f;


    //Multipliers for requiring certain event to happen to activate
    public const float REQUIREMENT__DODGE = 1.5f;
    public const float REQUIREMENT__RIPOSTE = 2f;
    public const float REQUIREMENT__COUNTER = 4f;
    public const float REQUIREMENT__WEAPON_DAMAGE = 1.5f;
    public const float REQUIREMENT__WEAPON_TECHNIQUE = 2.2f;
    public const float REQUIREMENT__BASIC_ATTACK = 1.5f;
    public const float REQUIREMENT__STAGGERING_AN_ENEMY = 5f;
    public const float REQUIREMENT__TAKEDOWN = 5f;
    public const float REQUIREMENT__DEALING_DAMAGE = 2.5f;
    public const float REQUIREMENT__DEALING_OR_TAKING_DAMAGE = 1.5f;
    public const float REQUIREMENT__BEING_DEBUFFED = 3.5f;
    public const float REQUIREMENT__GETTING_DAMAGED = 2.0f;
    public const float REQUIREMENT__GETTING_DAMAGED_FROM_BEHIND = 3.5f;
    private const float REQUIREMENT__SPECIFIC_EFFECT = 1.5f;
    public const float REQUIREMENT__SHARP = REQUIREMENT__SPECIFIC_EFFECT * 1.0f;
    public const float REQUIREMENT__BURN = REQUIREMENT__SPECIFIC_EFFECT * 1.15f;
    public const float REQUIREMENT__FREEZE = REQUIREMENT__SPECIFIC_EFFECT * 1.15f;
    public const float REQUIREMENT__BARRIER = REQUIREMENT__SPECIFIC_EFFECT * 1.25f;
    public const float REQUIREMENT__INCISION = REQUIREMENT__SPECIFIC_EFFECT * 1.1f;
    public const float REQUIREMENT__SUPERCHARGE = REQUIREMENT__SPECIFIC_EFFECT * 1.25f;
    public const float REQUIREMENT__ANALYSIS = REQUIREMENT__SPECIFIC_EFFECT * 1.0f;
    public const float REQUIREMENT__CHAINED = REQUIREMENT__SPECIFIC_EFFECT * 1.25f;
    public const float REQUIREMENT__AN_ENEMY_IS_IN_5M_RANGE = 2f;
    public const float REQUIREMENT__PLAYER_WAS_DAMAGED_IN_LAST_10_SECONDS = 2.5f;
    public const float REQUIREMENT__SPECIFIC_ITEM_EQUIPPED = 3.0f;
    public const float REQUIREMENT__PLAYER_SUFFERING_A_FATAL_BLOW = 2.25f;


    //Multipliers for special rules limiting effectivness
    public const float RESTRICTION__ONE_TIME_ONLY = 1.5f;
    public const float RESTRICTION__5_SECONDS_AFTER_RIPOSTE_OR_10_SECONDS_AFTER_COUNTER = 2.5f;
    public const float RESTRICTION__PLAYER_BASIC_ATTACKING = 2.1f;
    public const float RESTRICTION__PLAYER_CROWD_CONTROLLED = 4f;
    public const float RESTRICTION__WORKS_10_SECONDS_NON_STACKABLE = 3f;
    public const float RESTRICTION__WORKS_30_SECONDS_NON_STACKABLE = 1.7f;
    public const float RESTRICTION__ENEMY_EMPTY_STAGGER_BAR = 6.5f;
    public const float RESTRICTION__ENEMY_NO_STACKING_EFFECT_APPLIED = 5f;
    public const float RESTRICTION__0_AMMO = 2.0f;
    public const float RESTRICTION__ENEMY_BELOW_25P_HEALTH = 6f;
    public const float RESTRICTION__PLAYER_BELOW_25P_HEALTH = 3.5f;
    public const float RESTRICTION__PLAYER_ABOVE_50P_HEALTH = 1.3f;
    public const float RESTRICTION__PLAYER_BELOW_50P_HEALTH = 1.8f;
    public const float RESTRICTION__PLAYER_ABOVE_50P_STAGGER_BAR = 3.5f;
    public const float RESTRICTION__PLAYER_DEBUFFED = 2f;
    public const float RESTRICTION__PLAYER_STAGGERED = 3f;
    public const float RESTRICTION__5_SECOND_COOLDOWN = 1.5f;
    public const float RESTRICTION__10_SECOND_COOLDOWN = 1.75f;
    public const float RESTRICTION__15_SECOND_COOLDOWN = 2f;
    public const float RESTRICTION__30_SECOND_COOLDOWN = 2.5f;
    public const float RESTRICTION__60_SECOND_COOLDOWN = 3f;
    public const float RESTRICTION__120_SECOND_COOLDOWN = 3.5f;
    public const float RESTRICTION__300_SECOND_COOLDOWN = 4f;
    public const float RESTRICTION__LASTS_5_SECONDS = 2.0f;
    public const float RESTRICTION__LASTS_15_SECONDS = 1.5f;
    public const float RESTRICTION__LASTS_30_SECONDS = 1.25f;
    public const float RESTRICTION__LOSE_1P_CURRENT_HEALTH_EACH_SECOND = 5f;
    public const float RESTRICTION__CONSUME_1P_OF_STACKING_EFFECT = 1.5f;
    public const float RESTRICTION__CONSUME_ALL_SCALING_STACKING_EFFECT_WHEN_USED = 3f;
    public const float RESTRICTION__ONLY_AGAINST_ENEMY_WHO_LANDED_FATAL_BLOW = 1.75f;


    //Multipliers for requiring certain condition
    public const float SPECIAL__SCALES_WITH_ENEMY_DAMAGE_INSTEAD_OF_PLAYERS = 0.5f;
    public const float SPECIAL__SCALES_WITH_CURRENT_HEALTH_INSTEAD_OF_MAXIMUM = 1.6f;
    public const float SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM = 1.8f;
    public const float SPECIAL__INCREASE_STAT_BASE_INSTEAD_OF_PERCENTAGE = 5f;
    public const float SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION = 0.05714F;
    public const float SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION = 0.05714F;
    public const float SPECIAL__STAT_BONUS_CONVERSION = 1.0f;
    public const float SPECIAL__BONUS_FOR_AFFECTING_2_DIFFERENT_STACKING_EFFECTS = 1.25f;
    public const float SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS = 1.5f; //It is much harder to extract full value from all 3 stacking effects at the same time compared to just 1


    //Multipliers for certain unique properties
    public const float MULTIPLIER__BURN_PROPERTIES = 0.6f; //Burn Explosion
    public const float MULTIPLIER__FREEZE_PROPERTIES = 0.75f; //Freeze in place
    public const float MULTIPLIER__INCISION_PROPERTIES = 1.2f; //Cannot kill
    public const float MULTIPLIER__BARRIER_PROPERTIES = 1.5f; //Both Injury and Stagger can deplete
    public const float MULTIPLIER__SUPERCHARGE_PROPERTIES = 5f; //Consumed on usage, requires to be applied through Damage instead of damaging automatically
    public const float MULTIPLIER__CHAINED_PROPERTIES = 1.2f; //Has negative effects attached to it


    //Expected amount of certain stats or effects on average enemy
    public const float EXPECTED_AMOUNT_OF_REGULAR_ENEMY_HEALTH = 1000; // 1250 with DR
    public const float EXPECTED_AMOUNT_OF_REGULAR_ENEMY_STAGGER_BAR = 500; // 625 with DR
    public const float EXPECTED_AMOUNT_OF_BOSS_ENEMY_HEALTH = 10000; // 12500 with DR
    public const float EXPECTED_AMOUNT_OF_BOSS_ENEMY_STAGGER_BAR = 1000; // 1250 with DR
    public const float EXPECTED_AMOUNT_OF_ENEMY_DAMAGE_REDUCTION = 25;
    public const float EXPECTED_AMOUNT_OF_DEBUFFS_ON_PLAYER = 3;
    public const float EXPECTED_AMOUNT_OF_DEBUFFS_ON_ENEMY = 3;
    public const float EXPECTED_AMOUNT_OF_SPECIFIC_ITEM_EQUIPPED = 3.0f;
    public const float EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER = 30;
    public const float EXPECTED_AMOUNT_OF_DAMAGING_STACKING_EFFECT_ON_PLAYER = 50;


    //Stat Percentage Increase per 1 PB
    public const float DAMAGE_INCREASE_PER_PB = 1f;
    public const float DAMAGE_INCREASE_PER_1_ENEMY_DAMAGE_REDUCTION_PER_PB = 1 / PB.EXPECTED_AMOUNT_OF_ENEMY_DAMAGE_REDUCTION;
    public const float DAMAGE_REDUCTION_PENETRATION_PER_PB = 1 / PB.EXPECTED_AMOUNT_OF_ENEMY_DAMAGE_REDUCTION;
    public const float MAXIMUM_STAGGER_BAR_INCREASE_PER_PB = 2f;
    public const float MAXIMUM_HEALTH_INCREASE_PER_PB = 2f;
    public const float ATTACK_SPEED_INCREASE_PER_PB = 1.6f;
    public const float MOVEMENT_SPEED_INCREASE_PER_PB = 1.2f;
    public const float DAMAGE_REDUCTION_INCREASE_PER_PB = 1f;
    public const float TENACITY_INCREASE_PER_PB = 1.5f;
    public const float CONTROL_INCREASE_PER_PB = 1.5f;
    public const float ENERGY_GAIN_INCREASE_PER_PB = 1.25f;
    public const float FLAT_HEALTH_RESTORED_PER_PB = 2f;
    public const float FLAT_HEALTH_RESTORED_PER_SECOND_PER_PB = 0.4f;
    public const float FLAT_STAGGER_BAR_RESTORED_PER_PB = 4f;
    public const float FLAT_STAGGER_BAR_RESTORED_PER_SECOND_PER_PB = 0.8f;
    public const float PERCENTAGE_HEALTH_RESTORED_PER_PB = 0.1f;
    public const float PERCENTAGE_HEALTH_RESTORED_PER_SECOND_PER_PB = 0.02f;
    public const float PERCENTAGE_STAGGER_BAR_RESTORED_PER_PB = 0.2f;
    public const float PERCENTAGE_STAGGER_BAR_RESTORED_PER_SECOND_PER_PB = 0.04f;
    public const float COOLDOWN_REDUCTION_INCREASE_PER_PB = 1.25f;
    public const float EFFECT_AMOUNT_INCREASE_PER_PB = 2f;
    public const float BLOCK_DAMAGE_PER_PB = 10f;
    public const float EFFECT_DECAY_INCREASE_PER_PB = 3f;
    public const float PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB = 0.1f;
    public const float PERCENTAGE_OF_HEALTH_RESTORED_PER_1_ENERGY_SPENT_PER_PB = 0.01f;
    public const float PERCENTAGE_OF_HEALTH_RESTORED_PER_1_M_TRAVELLED_PER_PB = 0.05f;
    public const float PERCENTAGE_OF_MAX_HEALTH_RESTORED_FOR_EACH_SECOND_OF_CC_APPLIED_PER_PB = 0.1f;
    public const float CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB = 2;
    public const float PERCENTAGE_CONVERSION_OF_ONE_STACKING_EFFECT_SPENT_INTO_ANOTHER_PER_PB = 0.5f;
    public const float PERCENTAGE_CONVERSION_OF_DAMAGING_EFFECT_INTO_EXTRA_DAMAGE_PER_PB = 20f;
    public const float REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB = 1f;
    public const float GAIN_FLAT_ENERGY_AFFECTED_BY_ENERGY_GAIN_PER_PB = 0.5f;
    public const float AMMO_PERCENTAGE_RESTORED_PER_PB = 1f;
    public const float APPLY_STACKING_EFFECT_PER_PB = 1f;
    public const float DECREASE_STACKING_EFFECT_RECEIVED_PER_PB = 1.5f;
    public const float REMOVE_DAMAGING_STACKING_EFFECT_FROM_PLAYER_PER_PB = 0.5f;
    public const float EXTRA_MAXIMUM_HEALTH_RESTORED_BY_HEALTH_POTION_PER_PB = PERCENTAGE_HEALTH_RESTORED_PER_SECOND_PER_PB * 10;


    //Flat Stats per 1 PB
    public const float PLAYER_HEALTH_PER_PB = 40f;
    public const float ENEMY_HEALTH_PER_PB = 80f;
    public const float PLAYER_STAGGER_BAR_PER_PB = 80f;
    public const float ENEMY_STAGGER_BAR_PER_PB = 80f;
    public const float INJURY_SCALING_PER_PB = 100f;
    public const float STAGGER_SCALING_PER_PB = 100f;
    private const float SCALING_STACKING_EFFECT_PER_PB = 1f;
    private const float DAMAGING_STACKING_EFFECT_PER_PB = 1f;
    public const float SECONDS_OF_STUN_PER_PB = 0.1f;
    public const float SECONDS_OF_INVINCIBILITY_PER_PB = 0.02f;


    //Effect amount per 1 PB
    public const float ONSLAUGHT_PER_PB = SCALING_STACKING_EFFECT_PER_PB * SPECIALIZATION__BASIC_ATTACK;
    public const float SHARP_PER_PB = SCALING_STACKING_EFFECT_PER_PB * SPECIALIZATION__RIPOSTES_COUNTERS;
    public const float BURN_PER_PB = DAMAGING_STACKING_EFFECT_PER_PB * SPECIALIZATION__STAGGER * MULTIPLIER__BURN_PROPERTIES;
    public const float FREEZE_PER_PB = DAMAGING_STACKING_EFFECT_PER_PB * SPECIALIZATION__STAGGER * MULTIPLIER__FREEZE_PROPERTIES;
    public const float BARRIER_PER_PB = (PLAYER_HEALTH_PER_PB + ENEMY_HEALTH_PER_PB) / 2 * MULTIPLIER__BARRIER_PROPERTIES;
    public const float INCISION_PER_PB = DAMAGING_STACKING_EFFECT_PER_PB * SPECIALIZATION__INJURY * MULTIPLIER__INCISION_PROPERTIES;
    public const float SUPERCHARGE_PER_PB = DAMAGING_STACKING_EFFECT_PER_PB * MULTIPLIER__SUPERCHARGE_PROPERTIES;
    public const float ANALYSIS_PER_PB = SCALING_STACKING_EFFECT_PER_PB * SPECIALIZATION__TECHNIQUES;
    public const float CHAINED_PER_PB = SCALING_STACKING_EFFECT_PER_PB * MULTIPLIER__CHAINED_PROPERTIES;


    //Special effects per 1 PB
    public const float RESIST_FATAL_BLOW_AND_HEAL_TO_PERCENTAGE_HEALTH_PER_PB = 0.05f;
}
