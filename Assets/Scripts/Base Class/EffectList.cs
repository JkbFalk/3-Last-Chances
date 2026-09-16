using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using UnityEditor;    
using UnityEngine;

public class EffectList
{
    public static List<Effect> GetEffect(string effect_name, float power_budget, string special_id = "") {
        if(string.IsNullOrWhiteSpace(effect_name)) {
            Debug.LogError("Requested effect with no name");
            return null;
        }
        else if(power_budget == 0) {
            Debug.LogError($"Requested effect ({effect_name}) with 0 power budget: {power_budget}");
            return null;
        }
        List<Effect> Effects = new List<Effect>();
        if (effect_name == "Health")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_HEALTH_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Health }
                }
            };
        }
        else if (effect_name == "StaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.StaggerBar, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.StaggerBar }
                }
            };
        }
        else if (effect_name == "Damage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger }
                }
            };
        }
        else if (effect_name == "Injury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury }
                }
            };
        }
        else if (effect_name == "Stagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger }
                }
            };
        }
        else if (effect_name == "AttackSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyAttackSpeed,Player.Instance.LightAttackSpeed,Player.Instance.RangedAttackSpeed,Player.Instance.MagicAttackSpeed }
                }
            };
        }
        else if (effect_name == "EnergyGain")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.EnergyGain, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.EnergyGain }
                }
            };
        }
        else if (effect_name == "Armor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Armor, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor }
                }
            };
        }
        else if (effect_name == "Tenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Tenacity, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Tenacity }
                }
            };
        }
        else if (effect_name == "CooldownReduction")
        {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.CooldownReduction, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.CooldownReduction }
                }
            };
        }
        else if (effect_name == "Control")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONTROL_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Control, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Control }
                }
            };
        }
        else if (effect_name == "MovementSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MovementSpeed, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MovementSpeed }
                }
            };
        }
        else if (effect_name == "HeavyDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury, Player.Instance.HeavyStagger }
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "LightDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightInjury, Player.Instance.LightStagger }
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "RangedDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedInjury, Player.Instance.RangedStagger }
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "MagicDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicInjury, Player.Instance.MagicStagger }
                },
                new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "HeavyInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury}
                }
            };
        }
        else if (effect_name == "LightInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightInjury}
                }
            };
        }
        else if (effect_name == "RangedInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedInjury}
                }
            };
        }
        else if (effect_name == "MagicInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicInjury}
                }
            };
        }
        else if (effect_name == "HeavyStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger}
                }
            };
        }
        else if (effect_name == "LightStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightStagger}
                }
            };
        }
        else if (effect_name == "RangedStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedStagger}
                }
            };
        }
        else if (effect_name == "MagicStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicStagger}
                }
            };
        }
        else if (effect_name == "HeavyAttackSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyAttackSpeed}
                }
            };
        }
        else if (effect_name == "LightAttackSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightAttackSpeed}
                }
            };
        }
        else if (effect_name == "RangedAttackSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedAttackSpeed}
                }
            };
        }
        else if (effect_name == "MagicAttackSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicAttackSpeed}
                }
            };
        }

















        // Anima
        else if (effect_name == "ArmorAgainstCounterable")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__COUNTERABLE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool> ((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.IsCounterable)
                }
            };
        }
        else if (effect_name == "BasicAttackDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool> ((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack))
                }
            };
        }
        else if (effect_name == "WeaponTechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE, PB.AFFECTS_ONLY__TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool> ((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.IsWeaponDamage)
                }
            };
        }
        else if (effect_name == "WeaponDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "WeaponInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury }
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "WeaponStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE, PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger }
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "SharpAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "SharpDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "SharpArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__SHARP });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Sharp), Player.Instance.Armor, calculatedPB, new(effect_name)) {
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor },
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "ArmorAfterRiposteOrCounter")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__5_SECONDS_AFTER_RIPOSTE_OR_10_SECONDS_AFTER_COUNTER });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Effect extraArmor = new Effect_ChangeStat(Player.Instance.Armor, new(ability)) {
                            FlatAmount = effect.FlatAmount,
                            ShowsInUI = true,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="ArmorAfterRiposteOrCounter" + special_id,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        };
                        Player.Instance.AddEffect(extraArmor, ability.Is(Ability.Property.Counter) ? 10 : 5);
                    })
                }
            };
        }
        else if (effect_name == "ArmorAgainstUnstoppable")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__UNSTOPPABLE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool> ((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Unstoppable))
                }
            };
        }
        else if (effect_name == "RiposteDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__RIPOSTES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool> ((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Riposte))
                }
            };
        }
        else if (effect_name == "SharpEmpowersBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / (PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB), PB.AFFECTS_ONLY__BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 100)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool> ((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
                        damage.DamageDealtPercentageModifier += sharp.DecayingAmount * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "ArmorDuringBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentAbilityBeingPerformed != null &&  Player.Instance.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "CounterDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__COUNTERS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Counter)
                    )
                }
            };
        }
        else if (effect_name == "StrongBasicAttackDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STRONG_BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "SharpEmpowersWeaponTechniques")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / (PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB), PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__WEAPON_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 100)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.IsWeaponDamage && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
                        damage.DamageDealtPercentageModifier += sharp.DecayingAmount * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "RestoreStaggerBarOnSuccesfulRiposteOrCounterWhileStaggered")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.PERCENTAGE_STAGGER_BAR_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__RIPOSTING, PB.REQUIRES__PLAYER_STAGGERED });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.PERCENTAGE_STAGGER_BAR_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__COUNTERING, PB.REQUIRES__PLAYER_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2},
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * (ability.Is(Ability.Property.Counter) ? effect.CustomParameters[1] : effect.CustomParameters[0]) / 100;
                    })
                }
            };
        }
        else if (effect_name == "HealFromBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.PercentageAmount + damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "HealFromRipostesAndCounters")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__RIPOSTES_AND_COUNTERS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.Property.Riposte) || damage.SourceOfDamage.Is(Ability.Property.Counter))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.PercentageAmount + damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }























        // Ignis
        else if (effect_name == "DamageToStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))
                    )
                }
            };
        }
        else if (effect_name == "BasicAttackStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "TechniqueStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique)
                    )
                }
            };
        }
        else if (effect_name == "BurnAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "BurnDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "BurnArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__BURN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 0.5f), Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * 2)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Burn burn = (Effect_Burn)damage.SourceOfDamage.User.GetEffect(typeof(Effect_Burn));
                        damage.ArmorModifier += burn.StackingEffectIntensityLevel == 1 ? effect.FlatAmount / 2 : burn.StackingEffectIntensityLevel == 2 ? effect.FlatAmount : burn.StackingEffectIntensityLevel == 3 ? effect.FlatAmount * 2 : 0;
                    })
                }
            };
        }
        else if (effect_name == "ArmorAgainstNonBosses")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__NON_BOSSES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && !damage.SourceOfDamage.User.IsBoss
                    )
                }
            };
        }
        else if (effect_name == "HeavyStrongBasicAttackStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STRONG_BASIC_ATTACKS, PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> {Player.Instance.HeavyStagger },
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack) && damage.DamageType == Constants.DamageType.Heavy
                    )
                }
            };
        }
        else if (effect_name == "ExtraMagicStaggerToEmptyStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMY_EMPTY_STAGGER_BAR, PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> {Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.StaggerBar.Current < 1 && damage.DamageType == Constants.DamageType.Magic
                    )
                }
            };
        }
        else if (effect_name == "BurnAmountToNonBurning")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMY_NO_STACKING_EFFECT_APPLIED });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionForEffectPowerChange = new Func<Unit, bool>(target =>
                        target.CheckIfUnderEffect(typeof(Effect_Burn)) == false
                    )
                }
            };
        }
        else if (effect_name == "ArmorAgainstBosses")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__BOSSES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor },
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.IsBoss
                    )
                }
            };
        }
        else if (effect_name == "BurnEmpowersStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_DAMAGING_EFFECT_INTO_EXTRA_DAMAGE_PER_PB, new List<float> { 1 / PB.BURN_PER_PB, PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Burn))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Burn burn = (Effect_Burn)damage.TargetOfDamage.GetEffect(typeof(Effect_Burn));
                        damage.StaggerDealtFlatModifier += burn.DecayingAmount * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "BurnExplosionDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BURN_EXPLOSION_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.Is(DamageInstance.DamageProperty.BurnExplosion)
                    )
                }
            };
        }
        else if (effect_name == "BurnArmorWithoutLimit")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { 1 / (PB.EXPECTED_AMOUNT_OF_DAMAGING_STACKING_EFFECT_ON_ENEMY * PB.BURN_PER_PB) });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Burn), Player.Instance.Armor, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<String>{Utils.GetFormattedFloat(calculatedPB * 100)},
                    IncreaseBasedOnEffectLevel = false
                },
            };
        }
        else if (effect_name == "HealFromStackingEffects")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_STACKING_EFFECT_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && (damage.Is(DamageInstance.DamageProperty.Burn) || damage.Is(DamageInstance.DamageProperty.BurnExplosion) || damage.Is(DamageInstance.DamageProperty.Freeze) || damage.Is(DamageInstance.DamageProperty.Bleed))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.PercentageAmount + damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "HealFromStaggerToNonStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__NON_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }
















        //Glacies
        else if (effect_name == "FreezeAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "FreezeDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "RangedDamageLowersTenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE, PB.AFFECTS_ONLY__WORKS_10_SECONDS_NON_STACKABLE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = -calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect decreasedTenacity = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(damage.SourceOfDamage)) {
                            PathToUIGraphic="UI/Control",
                            FlatAmount = effect.FlatAmount,
                            ShowsInUI = true,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="RangedDamageLowersTenacity" + special_id,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        };
                        damage.TargetOfDamage.AddEffect(decreasedTenacity, 10);
                    })
                }
            };
        }
        else if (effect_name == "MagicDamageLowersTenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__MAGIC_DAMAGE, PB.AFFECTS_ONLY__WORKS_10_SECONDS_NON_STACKABLE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = -calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Magic
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect decreasedTenacity = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(damage.SourceOfDamage)) {
                            PathToUIGraphic="UI/Control",
                            FlatAmount = effect.FlatAmount,
                            ShowsInUI = true,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="MagicDamageLowersTenacity" + special_id,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        };
                        damage.TargetOfDamage.AddEffect(decreasedTenacity, 10);
                    })
                }
            };
        }
        else if (effect_name == "StaggeringLowersTenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__STAGGERING_AN_ENEMY, PB.AFFECTS_ONLY__WORKS_30_SECONDS_NON_STACKABLE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = -calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.SourceOfEffect.User == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect decreasedTenacity = new Effect_ChangeStat(effect_started.TargetOfEffect.Tenacity, effect_started.SourceOfEffect) {
                            PathToUIGraphic="UI/Control",
                            FlatAmount = effect.FlatAmount,
                            ShowsInUI = true,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="StaggeringLowersTenacity" + special_id,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        };
                        effect_started.TargetOfEffect.AddEffect(decreasedTenacity, 30);
                    })
                }
            };
        }
        else if (effect_name == "MovementSpeedWhileInRangedStance")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ONE_STANCE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MovementSpeed},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect_ChangeStat msBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "MovementSpeedWhileInRangedStance" + special_id);
                        if(Player.Instance.CurrentStance.DamageType != Constants.DamageType.Ranged && msBuff != null) {
                            msBuff.EndThisEffect();
                        }
                        else if(Player.Instance.CurrentStance.DamageType == Constants.DamageType.Ranged && msBuff == null){
                            msBuff = new Effect_ChangeStat(Player.Instance.MovementSpeed, new("MovementSpeedWhileInRangedStance")) {
                                FlatAmount = effect.FlatAmount,
                                BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                                Id="MovementSpeedWhileInRangedStance" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                            };
                            Player.Instance.AddEffect(msBuff);
                        }
                    })
                }
            };
        }
        else if (effect_name == "StaggeringEnemyGivesAmmo")
        {
            float calculatedPB = CalculatePB(power_budget, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__STAGGERING_AN_ENEMY });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.TargetOfEffect.IsHostile
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectEnding, effect) =>  {
                        Player.Instance.Ammo += effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "ControlWhileNoAmmo")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONTROL_INCREASE_PER_PB, new List<float> { PB.REQUIRES__BELOW_1_AMMO });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Control },
                    ConditionCheckForAmmoAmountChanged = new Func<bool>(() => true),
                    ActionOnAmmoAmountChanged = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Effect_ChangeStat controlBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "ControlWhileNoAmmo" + special_id);
                        if(Player.Instance.Ammo >= 1 && controlBuff != null) {
                            controlBuff.EndThisEffect();
                        }
                        else if(Player.Instance.Ammo < 1 && controlBuff == null) {
                            controlBuff = new Effect_ChangeStat(Player.Instance.Control, new("ControlWhileNoAmmo")) {
                                FlatAmount = effect.FlatAmount,
                                Id="ControlWhileNoAmmo" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                            };
                            Player.Instance.AddEffect(controlBuff);
                        }
                    })
                }
            };
        }
        else if (effect_name == "ControlWhileLowHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONTROL_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_BELOW_25P_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Control },
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat controlBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "ControlWhileLowHealth" + special_id);
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.25f && controlBuff != null) {
                            controlBuff.EndThisEffect();
                        }
                        else if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.25f && controlBuff == null) {
                            controlBuff = new Effect_ChangeStat(Player.Instance.Control, new("ControlWhileLowHealth")) {
                                FlatAmount = effect.FlatAmount,
                                Id="ControlWhileLowHealth" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                            };
                            Player.Instance.AddEffect(controlBuff);
                        }
                    })
                }
            };
        }
        else if (effect_name == "DamageToCrowdControlled")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__CROWD_CONTROLLED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl
                    )
                }
            };
        }
        else if (effect_name == "FinalAmmoDealsMoreDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> {Player.Instance.RangedInjury,Player.Instance.RangedStagger,},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(DamageInstance.DamageProperty.FinalAmmo)
                    )
                }
            };
        }
        else if (effect_name == "DamageToFrozen")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FROZEN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Frozen))
                    )
                }
            };
        }
        else if (effect_name == "DealMoreStaggerBasedOnEnemyArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_1_ENEMY_ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.Armor.Current > 1
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtPercentageModifier = damage.TargetOfDamage.Armor.Current * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "HealFromApplyingCrowdControl")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_MAX_HEALTH_RESTORED_FOR_EACH_SECOND_OF_CC_APPLIED_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_HardCrowdControl)) && effect.SourceOfEffect.User == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum / 100 * effect.PercentageAmount * effect_started.RemainingDuration;
                    })
                }
            };
        }
        else if (effect_name == "HealFromDamageToStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.PercentageAmount + damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }

















        //Molis
        if (effect_name == "FlatHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_HEALTH_INCREASE_PER_PB, new List<float> {PB.SPECIAL__INCREASE_STAT_BASE_INSTEAD_OF_PERCENTAGE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Health }
                }
            };
        }
        if (effect_name == "FlatStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, new List<float> { PB.SPECIAL__INCREASE_STAT_BASE_INSTEAD_OF_PERCENTAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.StaggerBar, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.StaggerBar }
                }
            };
        }
        else if (effect_name == "BarrierArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__BARRIER });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Barrier), Player.Instance.Armor, calculatedPB, new(effect_name))
            };
        }
        else if (effect_name == "ConvertStaggerBarToHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, PB.MAXIMUM_HEALTH_INCREASE_PER_PB, PB.SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.StaggerBar, Player.Instance.Health, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100 * Player.Instance.StaggerBar.Maximum, 2), Utils.GetFormattedFloat(calculatedPB, 2)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Health }
                },
            };
        }
        else if (effect_name == "ConvertHealthToStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.MAXIMUM_HEALTH_INCREASE_PER_PB, PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, PB.SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Health, Player.Instance.StaggerBar, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100 * Player.Instance.Health.Maximum, 2), Utils.GetFormattedFloat(calculatedPB, 2)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.StaggerBar}
                }
            };
        }
        else if (effect_name == "ConvertArmorToTenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.ARMOR_PER_PB, PB.TENACITY_INCREASE_PER_PB, PB.SPECIAL__STAT_BONUS_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Armor, Player.Instance.Tenacity, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat((Player.Instance.Armor.Current - 1) * calculatedPB, 2), Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Tenacity}
                }
            };
        }
        else if (effect_name == "ConvertTenacityToArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.TENACITY_INCREASE_PER_PB, PB.ARMOR_PER_PB, PB.SPECIAL__STAT_BONUS_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Tenacity, Player.Instance.Armor, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat((Player.Instance.Tenacity.Current - 1) * calculatedPB, 2), Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor}
                }
            };
        }
        else if (effect_name == "BarrierAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Barrier), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "BarrierDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Barrier), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "MoreTenacityWhileBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> { PB.REQUIRES__BARRIER });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Tenacity},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Tenacity, new (effect_name)) {
                            FlatAmount = effect.FlatAmount,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="MoreTenacityWhileBarrier" + special_id,
                            IsRemovable = false,
                            ShowsInUI = true,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        });
                    }),
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect tenacityBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Id == "MoreTenacityWhileBarrier" + special_id));
                        if(tenacityBuff != null) {
                            tenacityBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "MoreArmorWhileCCed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_CROWD_CONTROLLED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl
                    )
                }
            };
        }
        else if (effect_name == "ConvertHealthToInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { PB.DAMAGE_INCREASE_PER_PB, 1 / PB.MAXIMUM_HEALTH_INCREASE_PER_PB, PB.AFFECTS_ONLY__INJURY, PB.SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Health, Player.Instance.HeavyInjury, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100 * Player.Instance.Health.Maximum, 2), Utils.GetFormattedFloat(calculatedPB, 2)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury, Player.Instance.LightInjury, Player.Instance.RangedInjury, Player.Instance.MagicInjury}
                },
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Health, Player.Instance.LightInjury, calculatedPB, new(effect_name) ) {},
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Health, Player.Instance.RangedInjury, calculatedPB, new(effect_name) ) {},
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Health, Player.Instance.MagicInjury, calculatedPB, new(effect_name) ) {}
            };
        }
        else if (effect_name == "EnergyGainFromHealthLost")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENERGY_GAIN_FROM_HEALTH_LOST });
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * 10 / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.HealthLost
                }
            };
        }
        else if (effect_name == "HealthRegenWhileBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_SECOND_PER_PB, new List<float> { PB.REQUIRES__BARRIER });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new (effect_name)) {
                            RegenerationFlatAmount = effect.FlatAmount,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="HealthRegenWhileBarrier" + special_id,
                            IsRemovable = false,
                            ShowsInUI = true,
                            PathToUIGraphic="UI/HealthRegeneration",
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        });
                    }),
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Id == "HealthRegenWhileBarrier" + special_id));
                        if(regenBuff != null) {
                            regenBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "ArmorWhileLowHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_BELOW_50P_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "ArmorWhileLowHealth" + special_id);
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.5f && drBuff != null) {
                            drBuff.EndThisEffect();
                        }
                        else if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.5f && drBuff == null) {
                            drBuff = new Effect_ChangeStat(Player.Instance.Armor, new("ArmorWhileLowHealth")) {
                                FlatAmount = effect.FlatAmount,
                                Id="ArmorWhileLowHealth" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                            };
                            Player.Instance.AddEffect(drBuff);
                        }
                    })
                }
            };
        }
        else if (effect_name == "ConvertStaggerBarToStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { PB.DAMAGE_INCREASE_PER_PB, 1 / PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, PB.AFFECTS_ONLY__STAGGER, PB.SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.StaggerBar, Player.Instance.HeavyStagger, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100 * Player.Instance.StaggerBar.Maximum, 2), Utils.GetFormattedFloat(calculatedPB, 2)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger, Player.Instance.LightStagger, Player.Instance.RangedStagger, Player.Instance.MagicStagger}
                },
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.StaggerBar, Player.Instance.LightStagger, calculatedPB, new(effect_name) ) {},
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.StaggerBar, Player.Instance.RangedStagger, calculatedPB, new(effect_name) ) {},
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.StaggerBar, Player.Instance.MagicStagger, calculatedPB, new(effect_name) ) {}
            };
        }
        else if (effect_name == "EnergyGainFromBlocking")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENERGY_GAIN_FROM_BLOCKING });
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_PER_STAGGER_PERCENTAGE_LOST_FROM_BLOCKING * 10 / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.Block
                }
            };
        }
        else if (effect_name == "StaggerBarRegenWhileBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_STAGGER_BAR_RESTORED_PER_SECOND_PER_PB, new List<float> { PB.REQUIRES__BARRIER });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = -calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.StaggerBar, new (effect_name)) {
                            RegenerationFlatAmount = effect.FlatAmount,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="StaggerBarRegenWhileBarrier" + special_id,
                            IsRemovable = false,
                            ShowsInUI = true,
                            PathToUIGraphic="UI/StaggerBarRegeneration",
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        });
                    }),
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Id == "StaggerBarRegenWhileBarrier" + special_id));
                        if(regenBuff != null) {
                            regenBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "ArmorWhileHighStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_ABOVE_50P_STAGGER_BAR });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "ArmorWhileHighStaggerBar" + special_id);
                        if(Player.Instance.StaggerBar.Current < Player.Instance.StaggerBar.Maximum * 0.5f && drBuff != null) {
                            drBuff.EndThisEffect();
                        }
                        else if(Player.Instance.StaggerBar.Current >= Player.Instance.StaggerBar.Maximum * 0.5f && drBuff == null) {
                            drBuff = new Effect_ChangeStat(Player.Instance.Armor, new("ArmorWhileHighStaggerBar")) {
                                FlatAmount = effect.FlatAmount,
                                Id="ArmorWhileHighStaggerBar" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                            };
                            Player.Instance.AddEffect(drBuff);
                        }
                    })
                }
            };
        }
        else if (effect_name == "HealMissingHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_HEALTH_RESTORED_PER_SECOND_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Player.Instance.Health.Current < Player.Instance.Health.Maximum
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Missing * effect.PercentageAmount / 100 / 10;
                    })
                }
            };
        }
        else if (effect_name == "HealFromStaggerTaken")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.SPECIAL__SCALES_WITH_ENEMY_DAMAGE_INSTEAD_OF_PLAYERS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.StaggerDealt > 0 && !Player.Instance.CheckIfUnderEffect(typeof(Effect_PlayerStaggered))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }


















        //Salutis
        else if (effect_name == "SalutisTechniqueCooldownReduction")
        {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FAMILY_TECHNIQUES, PB.AFFECTS_ONLY__COOLDOWN_REDUCTION_FOR_TECHNIQUES });
            return new List<Effect> {
                new Effect_Id("TechniqueCooldownReduction - Salutis", new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "TechniqueCooldownReduction")
        {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__COOLDOWN_REDUCTION_FOR_TECHNIQUES });
            return new List<Effect> {
                new Effect_Id("TechniqueCooldownReduction", new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "EffectCooldownReduction")
        {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__COOLDOWN_REDUCTION_FOR_EFFECTS });
            return new List<Effect> {
                new Effect_Id("EffectCooldownReduction", new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "InjuryToBleeding")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.REQUIRES__BLEED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Bleed))
                    )
                }
            };
        }
        else if (effect_name == "BleedAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Bleed), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "BleedDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Bleed), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "BackstabInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__BACKSTABS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab)
                    )
                }
            };
        }
        else if (effect_name == "TechniqueInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique)
                    )
                }
            };
        }
        else if (effect_name == "BackstabsReduceCooldowns")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BACKSTABS });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.Is(Ability.Property.Backstab)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.ReduceAllRemainingCooldowns(effect.PercentageAmount);
                    })
                }
            };
        }
        else if (effect_name == "BackstabsGiveEnergy")
        {
            float calculatedPB = CalculatePB(power_budget, PB.GAIN_FLAT_ENERGY_AFFECTED_BY_ENERGY_GAIN_PER_PB, new List<float> { PB.AFFECTS_ONLY__BACKSTABS });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.Is(Ability.Property.Backstab)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.Energy.GenerateEnergy(effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "BackstabsDealMoreDamageBasedOnBleed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_DAMAGING_EFFECT_INTO_EXTRA_DAMAGE_PER_PB, new List<float> { 1 / PB.BLEED_PER_PB, PB.AFFECTS_ONLY__BACKSTABS, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Bleed))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Bleed Bleed = (Effect_Bleed)damage.TargetOfDamage.GetEffect(typeof(Effect_Bleed));
                        damage.InjuryDealtFlatModifier += Bleed.DecayingAmount * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "BasicAttackInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "InjuryArmorPenetration")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_ARMOR_PENETRATION_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorPenetrationModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "BackstabsScaleWithOnslaughtSharpAndAnalysis")
        {

            float calculatedPB1 = CalculatePB(power_budget * 0.333f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ONSLAUGHT_PER_PB, PB.AFFECTS_ONLY__BACKSTABS, PB.SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS });
            float calculatedPB2 = CalculatePB(power_budget * 0.333f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB, PB.AFFECTS_ONLY__BACKSTABS, PB.SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS });
            float calculatedPB3 = CalculatePB(power_budget * 0.333f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ANALYSIS_PER_PB, PB.AFFECTS_ONLY__BACKSTABS, PB.SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2, calculatedPB3},
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Onslaught))) {
                            Effect_Onslaught onslaught = (Effect_Onslaught)damage.TargetOfDamage.GetEffect(typeof(Effect_Onslaught));
                            damage.DamageDealtPercentageModifier += onslaught.DecayingAmount * effect.CustomParameters[0] / 100;
                        }
                        if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))) {
                            Effect_Sharp sharp = (Effect_Sharp)damage.TargetOfDamage.GetEffect(typeof(Effect_Sharp));
                            damage.DamageDealtPercentageModifier += sharp.DecayingAmount * effect.CustomParameters[1] / 100;
                        }
                        if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis))) {
                            Effect_Analysis analysis = (Effect_Analysis)damage.TargetOfDamage.GetEffect(typeof(Effect_Analysis));
                            damage.DamageDealtPercentageModifier += analysis.DecayingAmount * effect.CustomParameters[2] / 100;
                        }
                    })
                }
            };
        }
        else if (effect_name == "BackstabsApplyBleed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_PERCENTAGE_OF_DAMAGE_DEALT_AS_STACKING_EFFECT_PER_PB, new List<float> { PB.BLEED_PER_PB, PB.AFFECTS_ONLY__BACKSTABS, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Bleed(damage.InjuryDealt * effect.PercentageAmount, new(damage.SourceOfDamage)));
                    })
                }
            };
        }
        else if (effect_name == "HealFromBackstabs")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__BACKSTABS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.DamagePercentageModifier + damage.StaggerDealt / 100 * effect.DamagePercentageModifier;
                    })
                }
            };
        }
        else if (effect_name == "HealFromTakedowns")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__TAKEDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitKnockedOut = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnUnitKnockedOut = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum * effect.PercentageAmount / 100;
                    }),
                    ConditionCheckForHealthBarBroken = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnHealthBarBroken = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum * effect.PercentageAmount / 100;
                    })
                }
            };
        }
























        //Tonitrui
        else if (effect_name == "SuperchargeAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Supercharge), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "SuperchargeDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Supercharge), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "ToolCooldownReduction")
        {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__COOLDOWN_REDUCTION_FOR_TOOLS });
            return new List<Effect> {
                new Effect_Id("ToolCooldownReduction", new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "SuperchargeMovementSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB, new List<float> { PB.REQUIRES__SUPERCHARGE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect is Effect_Supercharge && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.MovementSpeed, new (effect_name)) {
                            FlatAmount = effect.FlatAmount,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="SuperchargeMovementSpeed" + special_id,
                            IsRemovable = false,
                            ShowsInUI = true,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        });
                    }),
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect is Effect_Supercharge && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Id == "SuperchargeMovementSpeed" + special_id));
                        if(msBuff != null) {
                            msBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "SuperchargeArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__SUPERCHARGE });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Supercharge), Player.Instance.Armor, calculatedPB, new(effect_name))
            };
        }
        else if (effect_name == "SuperchargeInjuryDealt")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__SUPERCHARGE_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(DamageInstance.DamageProperty.Supercharge)
                    )
                }
            };
        }
        else if (effect_name == "SpendingSuperchargeGeneratesBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_ONE_STACKING_EFFECT_SPENT_INTO_ANOTHER_PER_PB, new List<float> { PB.BARRIER_PER_PB, 1 / PB.SUPERCHARGE_PER_PB });
            return new List<Effect> {
                new Effect_Id("SpendingSuperchargeGeneratesBarrier" + special_id, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "SuperchargeInjuryFromBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.AFFECTS_ONLY__SUPERCHARGE_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(DamageInstance.DamageProperty.Supercharge)
                    )
                }
            };
        }
        else if (effect_name == "ConvertMovementSpeedToArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.MOVEMENT_SPEED_INCREASE_PER_PB, PB.ARMOR_PER_PB, PB.SPECIAL__STAT_BONUS_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Tenacity, Player.Instance.Armor, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat((Player.Instance.MovementSpeed.Current - 1) * calculatedPB, 2), Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor}
                }
            };
        }
        else if (effect_name == "HealFromDistanceTravelled")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEALTH_RESTORED_PER_1_M_TRAVELLED_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Player.Instance.Health.Current < Player.Instance.Health.Maximum && Player.Instance.PlayerSavedPosition != Player.Instance.transform.position
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        if(Player.Instance.PlayerSavedPosition == null) {
                            Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                        }
                        Player.Instance.Health.Current += Vector2.Distance(Player.Instance.PlayerSavedPosition, Player.Instance.transform.position) * effect.PercentageAmount / 100 / 10;
                        Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                    })
                }
            };
        }
        else if (effect_name == "HealMoreFromHealthPotions")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EXTRA_MAXIMUM_HEALTH_RESTORED_BY_HEALTH_POTION_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability is Ability_Heal
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {RegenerationPercentageAmount = effect.PercentageAmount / 2 }, 2);
                    })
                }
            };
        }































        //Proprius
        else if (effect_name == "EnergyGainFromBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENERGY_GAIN_FROM_BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_BASIC_ATTACK / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.BasicAttack
                }
            };
        }
        else if (effect_name == "EnergyGainFromDodges")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENERGY_GAIN_FROM_DODGING });
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_DODGING / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.Dodge
                }
            };
        }
        else if (effect_name == "EnergyGainFromRipostesAndCounters")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__RIPOSTES_AND_COUNTERS });
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_RIPOSTING / 100), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_COUNTERING / 100, 1)},
                    IncreaseAmount = calculatedPB,
                    SpecifiedSource = Constants.EnergyGainSource.Riposte
                },
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB,
                    SpecifiedSource = Constants.EnergyGainSource.Counter
                }
            };
        }
        else if (effect_name == "EnergyGainFromStaggering")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENERGY_GAIN_FROM_STAGGERING });
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_INFLICTING_STAGGERED / 100)},
                    IncreaseAmount = calculatedPB,
                    SpecifiedSource = Constants.EnergyGainSource.InflictedStaggered
                }
            };
        }
        else if (effect_name == "TechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique)
                    )
                }
            };
        }
        else if (effect_name == "AnalysisAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Analysis), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "AnalysisDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Analysis), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "AnalysisBoostsAllDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ANALYSIS_PER_PB, PB.AFFECTS_ONLY__EVERYTHING_EXCEPT_TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis)) && damage.SourceOfDamage.IsNot(Ability.Property.Technique)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Analysis analysis = (Effect_Analysis)Player.Instance.GetEffect(typeof(Effect_Analysis));
                        damage.DamageDealtPercentageModifier += analysis.DecayingAmount * effect.DamagePercentageModifier / 100;
                    })
                }
            };
        }
        else if (effect_name == "UltimateTechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ULTIMATE_TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Ultimate)
                    )
                }
            };
        }
        else if (effect_name == "PropriusTechniqueCooldownReduction")
        {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FAMILY_TECHNIQUES, PB.AFFECTS_ONLY__COOLDOWN_REDUCTION_FOR_TECHNIQUES });
            return new List<Effect> {
                new Effect_Id("TechniqueCooldownReduction - Proprius", new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "AnalysisEnergyGain")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.REQUIRES__ANALYSIS });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect is Effect_Analysis && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.EnergyGain, new (effect_name)) {
                            FlatAmount = effect.FlatAmount,
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="AnalysisEnergyGain",
                            IsRemovable = false,
                            ShowsInUI = true,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount)
                        });
                    }),
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect is Effect_Analysis && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Id == "AnalysisEnergyGain" + special_id));
                        if(msBuff != null) {
                            msBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "AnalysisArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__ANALYSIS });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Analysis), Player.Instance.Armor, calculatedPB, new(effect_name))
            };
        }
        else if (effect_name == "PropriusUltimateTechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ULTIMATE_TECHNIQUES, PB.AFFECTS_ONLY__FAMILY_TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Ultimate) && Technique.GetFamily(damage.SourceOfDamage.GetType()) == Ability.AbilityFamily.Proprius
                    )
                }
            };
        }
        else if (effect_name == "TechniqueDamageGivesAnalysis")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ANALYSIS_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.AFFECTS_ONLY__TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Analysis(effect.FlatAmount, new(effect_name)));
                    })
                }
            };
        }
        else if (effect_name == "ConvertEnergyGainIntoHealthAndStaggerBar")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { PB.MAXIMUM_HEALTH_INCREASE_PER_PB, 1 / PB.ENERGY_GAIN_INCREASE_PER_PB, 1 / PB.SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, 1 / PB.ENERGY_GAIN_INCREASE_PER_PB, 1 / PB.SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION });
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.EnergyGain, Player.Instance.Health, calculatedPB1, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                },
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.EnergyGain, Player.Instance.StaggerBar, calculatedPB2, new(effect_name) ) {},
            };
        }
        else if (effect_name == "HealFromTechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.PercentageAmount + damage.StaggerDealt / 100 * effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "HealFromEnergyUsed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEALTH_RESTORED_PER_1_ENERGY_SPENT_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 10), "10"},
                    ConditionCheckForAbilityEnergyConsumed = new Func<Ability, float, bool, bool>((ability, amount, was_full_energy) =>
                        ability.User == Player.Instance && amount > 0
                    ),
                    ActionOnAbilityEnergyConsumed = new Action<Ability, float, bool, Effect_CustomizableEffectOnEvent> ((ability, amount, was_full_energy, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum * amount * calculatedPB / 100;
                    })
                }
            };
        }





































        //Duelist
        else if (effect_name == "GainSharpOnRiposteCounterOrDodge")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.HAPPENS_UPON__RIPOSTING });
            float calculatedPB2 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.HAPPENS_UPON__COUNTERING });
            float calculatedPB3 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.HAPPENS_UPON__DODGING });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3)},
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User is Player && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(ability.Is(Ability.Property.Riposte) ? effect.CustomParameters[0] : effect.CustomParameters[1], new(effect_name)));
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB3,
                    Id = "GainSharpOnRiposteCounterOrDodge" + special_id,
                    ConditionCheckForDamageWasDodged = new Func<DamageInstance, Ability, bool>((damage, dodge) =>
                        damage.TargetOfDamage == Player.Instance && dodge.TriggeredEffectWithId("GainSharpOnRiposteCounterOrDodge" + special_id) == false
                    ),
                    ActionOnDamageWasDodged = new Action<DamageInstance, Ability, Effect_CustomizableEffectOnEvent> ((damage, dodge, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(effect.FlatAmount, new(effect_name)));
                    })
                }
            };
        }
        else if (effect_name == "SharpArmorWithoutLimit")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { 1 / (PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB) });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Sharp), Player.Instance.Armor, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<String>{Utils.GetFormattedFloat(calculatedPB * 100)},
                    IncreaseBasedOnEffectLevel = false
                },
            };
        }
        else if (effect_name == "GainSharpOnBasicAttacksAndOnslaughtOnRipostesAndCounters")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ONSLAUGHT_PER_PB, PB.HAPPENS_UPON__RIPOSTING_OR_COUNTERING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    FlatAmount = calculatedPB1,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(effect.FlatAmount, effect.SourceOfEffect));
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount= calculatedPB2,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User is Player && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Onslaught(effect.FlatAmount, effect.SourceOfEffect));
                    })
                },
            };
        }
        else if (effect_name == "RestoreHealthOnRiposteOrCounter")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__RIPOSTING });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__COUNTERING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.Property.Riposte) || damage.SourceOfDamage.Is(Ability.Property.Counter))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.SourceOfDamage.Is(Ability.Property.Riposte) ? effect.CustomParameters[0] : effect.CustomParameters[1];
                    })
                }
            };
        }
        else if (effect_name == "GainInvincibleOnRiposteOrCounter")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.SECONDS_OF_INVINCIBILITY_PER_PB, new List<float> { PB.HAPPENS_UPON__RIPOSTING });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.SECONDS_OF_INVINCIBILITY_PER_PB, new List<float> { PB.HAPPENS_UPON__COUNTERING });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && (ability.Is(Ability.Property.Counter) || ability.Is(Ability.Property.Riposte))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Invincible(new(effect_name)), ability.Is(Ability.Property.Riposte) ? effect.CustomParameters[0] : effect.CustomParameters[1]);
                    })
                }
            };
        }
        else if (effect_name == "EmpowerNextAttackAfterRiposteOrCounter")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__RIPOSTING, PB.AFFECTS_ONLY__ONCE });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__COUNTERING, PB.AFFECTS_ONLY__ONCE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && (ability.Is(Ability.Property.Counter) || ability.Is(Ability.Property.Riposte))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(effect_name)) {
                            DamagePercentageModifier = ability.Is(Ability.Property.Riposte) ? effect.CustomParameters[0] : effect.CustomParameters[1],
                            ShowsInUI = true,
                            PathToUIGraphic = "Effect/Empowered",
                            UIText = ability.Is(Ability.Property.Riposte) ? Utils.GetFormattedFloat(effect.CustomParameters[1]) : Utils.GetFormattedFloat(effect.CustomParameters[2])
                        });
                    })
                }
            };
        }
        else if (effect_name == "ExtraEffectiveButConsumableSharp")
        {
            float calculatedPB1 = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB * -0.3f);
            float calculatedPB2 = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB * 1.3f, new List<float> { PB.REQUIRES__CONSUMING_ALL_SCALING_STACKING_EFFECT_WHEN_USED });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB1, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(Math.Abs(calculatedPB2)), Utils.GetFormattedFloat(calculatedPB1)}
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    ConditionCheckForAbilityEnded = new Func<Ability, bool>((ability) =>
                        ability.User is Player && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter)) && ability.User.CheckIfUnderEffect(typeof(Effect_Sharp))
                    ),
                    ActionOnAbilityEnded = new Action<Ability, Effect_CustomizableEffectOnEvent> ((Ability, effect) =>  {
                        Effect sharp = Player.Instance.GetEffect(typeof(Effect_Sharp));
                        sharp.EndThisEffect();
                    })
                },
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB2, new(effect_name))
            };
        }
        else if (effect_name == "ReceiveSharpWhileInProximity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.REQUIRES__ANY_ENEMY_IS_IN_5M_RANGE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {"5", Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB,
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Utils.GetAllUnits(true, true).FirstOrDefault(enemy => Vector2.Distance(enemy.transform.position, Player.Instance.transform.position) < 5) != null
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(effect.FlatAmount / 10, effect.SourceOfEffect));
                    })
                }
            };
        }


        //Weaponmaster
        else if (effect_name == "WeaponTechniquesEmpowerNextBasicAttackAndViceVersa")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.HAPPENS_UPON__USING_WEAPON_TECHNIQUE, PB.AFFECTS_ONLY__5_SECONDS });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__WEAPON_DAMAGE, PB.HAPPENS_UPON__BASIC_ATTACKING, PB.AFFECTS_ONLY__5_SECONDS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB1,
                    DescriptionParameters = new List<String> {"5", Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.IsWeaponDamage
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(
                        new Effect_CustomizableDamageChange(effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount,
                            ShowsInUI = true,
                            PathToUIGraphic="UI/AllWeapons",
                            Id="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa - EmpoweredBasicAttack" + special_id,
                            UIText = Utils.GetFormattedFloat(effect.PercentageAmount),
                            ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage2, effect2) =>
                                damage2.SourceOfDamage.User == Player.Instance && damage2.SourceOfDamage.Is(Ability.Property.BasicAttack)
                            ),
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage3, effect3) =>  {
                                damage3.DamageDealtPercentageModifier += effect3.PercentageAmount;
                                effect3.EndThisEffect();
                            })
                        }
                    , 5);
                })},
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount =  calculatedPB2,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(
                            new Effect_CustomizableDamageChange(effect.SourceOfEffect) {
                                PercentageAmount = effect.PercentageAmount,
                                ShowsInUI = true,
                                PathToUIGraphic="UI/Technique",
                                Id="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa - EmpoweredWeaponTechnique" + special_id,
                                UIText = Utils.GetFormattedFloat(effect.PercentageAmount),
                                ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage2, effect2) =>
                                    damage2.SourceOfDamage.User == Player.Instance && damage2.SourceOfDamage.Is(Ability.Property.Technique) && damage2.IsWeaponDamage
                                ),
                                BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                                Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage3, effect3) =>  {
                                    damage3.DamageDealtPercentageModifier += effect3.PercentageAmount;
                                    effect3.EndThisEffect();
                                })
                            },
                        5);
                    }
                )}
            };
        }
        else if (effect_name == "BlademastersGarb")
        {
            float calculatedPB1 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE, PB.AFFECTS_ONLY__BASIC_ATTACKS });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.ARMOR_PER_PB);
            float calculatedPB3 = CalculatePB(power_budget * 1.5f, PB.ARMOR_PER_PB);
            float calculatedPB4 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE, PB.AFFECTS_ONLY__BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_BlademastersGarb(new(effect_name)) {
                    DescriptionParameters = new List<String>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3), Utils.GetFormattedFloat(calculatedPB4)},
                    BladedBasicAttackBuff = calculatedPB1,
                    BladedArmorDebuff = calculatedPB2,
                    NonBladedArmorBuff = calculatedPB3,
                    NonBladedBasicAttackDebuff = calculatedPB4
                }
            };
        }
        else if (effect_name == "RestoreHealthOnWeaponDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_ANY_WEAPON_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.IsWeaponDamage
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FlatAmount;
                    })
                }
            };
        }
        else if (effect_name == "WeaponAttackSpeed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyAttackSpeed}
                },
                new Effect_ChangeStat(Player.Instance.LightAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightAttackSpeed}
                },
                new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedAttackSpeed}
                }
            };
        }
        else if (effect_name == "IncreasedWeaponDamageButDecreasedMagicDamage")
        {
            float calculatedPB1 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            return new List<Effect> {
                    new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                        PercentageAmount = calculatedPB1,
                        DescriptionParameters=new List<string>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}
                    },
                    new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                        PercentageAmount = calculatedPB1
                    },
                    new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                        PercentageAmount = calculatedPB1
                    },
                    new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                        PercentageAmount = calculatedPB1
                    },
                    new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                        PercentageAmount = calculatedPB1
                    },
                    new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                        PercentageAmount = calculatedPB1
                    },
                    new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                        PercentageAmount = -calculatedPB2
                    },
                    new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                        PercentageAmount = -calculatedPB2
                    }
            };
        }


        //Jailer
        else if (effect_name == "DealingOrTakingDamageAppliesChainedToYou")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.CHAINED_PER_PB, PB.HAPPENS_UPON__DEALING_OR_TAKING_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance || damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "ChainedAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "ChainedDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "GainDamageAndArmorForEachDebuff")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.666f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__PLAYER_DEBUFFED, 1 / PB.EXPECTED_AMOUNT_OF_DEBUFFS_ON_PLAYER });
            float calculatedPB2 = CalculatePB(power_budget * 0.334f, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__PLAYER_DEBUFFED, 1 / PB.EXPECTED_AMOUNT_OF_DEBUFFS_ON_PLAYER });
            return new List<Effect> {
                new Effect_GainDamageAndArmorForEachDebuff(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), "10"},
                    DamageGainedPerDebuff = calculatedPB1,
                    ArmorGainedPerDebuff = calculatedPB2,
                    MaxDebuffs = 10
                }
            };
        }
        else if (effect_name == "ChainedArmor")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.666f, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__CHAINED, PB.SPECIAL__BONUS_FOR_SCALING_WITH_BOTH_SELF_AND_ENEMY_DECAYING_EFFECT });
            float calculatedPB2 = CalculatePB(power_budget * 0.334f, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__CHAINED, PB.SPECIAL__BONUS_FOR_SCALING_WITH_BOTH_SELF_AND_ENEMY_DECAYING_EFFECT });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Chained), Player.Instance.Armor, calculatedPB1, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1 * 0.5f), Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB1 * 2), Utils.GetFormattedFloat(calculatedPB2 * 0.5f), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB2 * 2)},
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB2,
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Chained))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Chained burn = (Effect_Chained)damage.SourceOfDamage.User.GetEffect(typeof(Effect_Chained));
                        damage.ArmorModifier += burn.StackingEffectIntensityLevel == 1 ? effect.FlatAmount / 2 : burn.StackingEffectIntensityLevel == 2 ? effect.FlatAmount : burn.StackingEffectIntensityLevel == 3 ? effect.FlatAmount * 2 : 0;
                    })
                }
            };
        }
        else if (effect_name == "DodgingAndCounteringAppliesChained")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.CHAINED_PER_PB, PB.HAPPENS_UPON__DODGING });
            float calculatedPB2 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.CHAINED_PER_PB, PB.HAPPENS_UPON__RIPOSTING });
            float calculatedPB3 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.CHAINED_PER_PB, PB.HAPPENS_UPON__COUNTERING });
            float calculatedPB = calculatedPB1 + calculatedPB2 + calculatedPB3;
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User is Player && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, effect.SourceOfEffect));
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    ConditionCheckForDamageWasDodged = new Func<DamageInstance, Ability, bool>((damage, dodge) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    ActionOnDamageWasDodged = new Action<DamageInstance, Ability, Effect_CustomizableEffectOnEvent> ((damage, dodge, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "RestoreStaggerFromDebuffs")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_STAGGER_BAR_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__BEING_DEBUFFED, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "5"},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.TargetOfEffect == Player.Instance && effect.Type == Effect.EffectType.Debuff && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("RestoreStaggerFromDebuffs" + special_id)
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectStarted, effect) =>  {
                        Player.Instance.StaggerBar.Current -= effect.FlatAmount;
                        Player.Instance.AddCooldown(typeof(Effect), 5, "RestoreStaggerFromDebuffs" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "HealFromBeingDamagedByStackingEffects")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMY_STACKING_EFFECT_DAMAGE, PB.SPECIAL__SCALES_WITH_ENEMY_DAMAGE_INSTEAD_OF_PLAYERS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters=new List<string>{Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && (damage.Is(DamageInstance.DamageProperty.Burn) || damage.Is(DamageInstance.DamageProperty.Freeze) || damage.Is(DamageInstance.DamageProperty.Bleed))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt * effect.PercentageAmount / 100 + damage.StaggerDealt * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "ApplySelfChainedToEnemies")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)},
                    PercentageAmount = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && Player.Instance.CheckIfUnderEffect(typeof(Effect_Chained))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Chained playersChained = (Effect_Chained)Player.Instance.GetEffect(typeof(Effect_Chained));
                        damage.TargetOfDamage.AddEffect(new Effect_Chained(playersChained.DecayingAmount * effect.PercentageAmount / 100, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "GainChainedOnHeavyDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.CHAINED_PER_PB, PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "ConvertCurrentHealthIntoChained")
        {
            float currentHealthPercentageLostEachSecond = 2;
            float calculatedPB = CalculatePB(power_budget, PB.STACKING_EFFECT_GAINED_PER_10_HEALTH_LOST, new List<float> { PB.CHAINED_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {currentHealthPercentageLostEachSecond.ToString(), Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB,
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.1f && Player.Instance.InCombat
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.Health.Current -= Player.Instance.Health.Maximum * 0.02f / 10;
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount * (Player.Instance.Health.Maximum * 0.02f / 10 / 100), effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "BasicAttacksRestoreHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> { Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FlatAmount;
                    })
                }
            };
        }
        else if (effect_name == "ConvertChainedIntoHealth")
        {
            float percentageOfChainedConsumed = 15;
            float calculatedPB = CalculatePB(power_budget, PB.HEALTH_HEALED_PER_10_STACKING_EFFECT_LOST, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING, PB.REQUIRES__CHAINED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> { percentageOfChainedConsumed.ToString(), Utils.GetFormattedFloat(calculatedPB * 10)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                            damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Chained))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Chained playersChained = (Effect_Chained)Player.Instance.GetEffect(typeof(Effect_Chained));
                        Player.Instance.Health.Current += effect.PercentageAmount * playersChained.DecayingAmount * 0.15f;
                        Effect e2 = Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Id == "ConvertedChainedGeneratesBarrier"));
                        if(e2 != null) {
                            Player.Instance.AddEffect(new Effect_Barrier(e2.PercentageAmount * playersChained.DecayingAmount * 0.15f, effect.SourceOfEffect));
                        }
                        playersChained.ChangeDecayingAmount(-playersChained.DecayingAmount * 0.15f);
                    })
                }
            };
        }
        else if (effect_name == "ConvertedChainedGeneratesBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_ONE_STACKING_EFFECT_SPENT_INTO_ANOTHER_PER_PB, new List<float> { PB.BARRIER_PER_PB, 1 / PB.CHAINED_PER_PB });
            return new List<Effect> {
                new Effect_Id("ConvertedChainedGeneratesBarrier" + special_id, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 10)}
                }
            };
        }
        else if (effect_name == "GainBurnOnBasicAttackAndIncreaseDamageBasedOnBurn")
        {
            float calculatedPB1 = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB * 0.25f, new List<float> { PB.BURN_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            float calculatedPB2 = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB * 1.25f, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2 * 10)},
                    FlatAmount = calculatedPB1,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.Is(Ability.Property.BasicAttack)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Burn(effect.FlatAmount, effect.SourceOfEffect));
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB2,
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage?.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Burn)) && damage.DamageType == Constants.DamageType.Ranged
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Burn playersBurn = (Effect_Burn)Player.Instance.GetEffect(typeof(Effect_Burn));
                        damage.DamageDealtPercentageModifier += playersBurn.DecayingAmount * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "CleanseBurnOnWeaponSwitch")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REMOVE_DAMAGING_STACKING_EFFECT_FROM_PLAYER_PER_PB, new List<float> { PB.BURN_PER_PB, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    CustomParameters = new List<float>() {30 },
                    FlatAmount = calculatedPB,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch)) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Burn)) && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("CleanseBurnOnWeaponSwitch" + special_id)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddCooldown(new Cooldown(effect.GetType(), effect.CustomParameters[0], Player.Instance, "CleanseBurnOnWeaponSwitch" + special_id));
                        Effect_Burn playersBurn = (Effect_Burn)Player.Instance.GetEffect(typeof(Effect_Burn));
                        playersBurn.ChangeDecayingAmount(-effect.FlatAmount);
                    })
                }
            };
        }


        //Ancient
        else if (effect_name == "AncientHelmet")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> { PB.REQUIRES__SPECIFIC_ITEM_EQUIPPED });
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_AncientCrown(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FlatAmount = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.CooldownReduction, new(effect_name)) {
                    FlatAmount = calculatedPB2
                }
            };
        }
        else if (effect_name == "AncientGloves")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.REQUIRES__SPECIFIC_ITEM_EQUIPPED });
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.ENERGY_GAIN_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_AncientGauntlets(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FlatAmount = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.EnergyGain, new(effect_name)) {
                    FlatAmount = calculatedPB2
                }
            };
        }
        else if (effect_name == "AncientArmor")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__SPECIFIC_ITEM_EQUIPPED });
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.DAMAGE_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_AncientBreastplate(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    PercentageAmount = calculatedPB1
                },
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(effect_name)) {
                    PercentageAmount = calculatedPB2
                }
            };

        }
        else if (effect_name == "AncientBoots")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.REQUIRES__SPECIFIC_ITEM_EQUIPPED });
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.ATTACK_SPEED_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_AncientGreaves(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    PercentageAmount = calculatedPB1
                },
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(effect_name)) {
                    PercentageAmount = calculatedPB2
                }
            };
        }
        else if (effect_name == "AncientFirearmsDamage")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SPECIFIC_ITEM_EQUIPPED, PB.REQUIRES__SPECIFIC_ITEM_EQUIPPED });
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.DAMAGE_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_AncientFirearmsDamage(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    PercentageAmount = calculatedPB1
                },
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(effect_name)) {
                    PercentageAmount = calculatedPB2
                }
            };
        }
        else if (effect_name == "AncientFirearmsArmor")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.ARMOR_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SPECIFIC_ITEM_EQUIPPED, PB.REQUIRES__SPECIFIC_ITEM_EQUIPPED });
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.ARMOR_PER_PB);
            return new List<Effect> {
                new Effect_AncientFirearmsArmor(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FlatAmount = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.EnergyGain, new(effect_name)) {
                    FlatAmount = calculatedPB2
                }
            };
        }

        //BattleBorn
        else if (effect_name == "GainAttackSpeedAsHealthLowers")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeCompositeStat asBuff = (Effect_ChangeCompositeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "GainAttackSpeedAsHealthLowers" + special_id);
                        if(asBuff == null) {
                            asBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, effect.SourceOfEffect) {
                                PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum),
                                Id="GainAttackSpeedAsHealthLowers" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 0)
                            };
                            Player.Instance.AddEffect(asBuff);
                        }
                        else {
                            asBuff.PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                            asBuff.UIText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 0);
                        }
                    })
                }
            };
        }
        else if (effect_name == "TakingDamageGivesBarrierBasedOnMissingHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__GETTING_DAMAGED, PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "LoseHealthWhileAboveHalfAndRegenWhileBelow")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_ABOVE_50P_HEALTH, PB.SPECIAL__SCALES_WITH_CURRENT_HEALTH_INSTEAD_OF_MAXIMUM });
            float calculatedPB2 = CalculatePB(power_budget * 1.5f, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_BELOW_50P_HEALTH, PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2},
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect minusRegen = Player.Instance.GetEffectWithGivenId("LoseHealthWhileAboveHalfAndRegenWhileBelow - LoseHealth");
                        Effect plusRegen = Player.Instance.GetEffectWithGivenId("LoseHealthWhileAboveHalfAndRegenWhileBelow - RestoreHealth");
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.5f && plusRegen != null) {
                            plusRegen.EndThisEffect();
                        }
                        if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.5f && minusRegen != null) {
                            minusRegen.EndThisEffect();
                        }
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.5f && minusRegen == null) {
                            Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, effect.SourceOfEffect) {
                                RegenerationPercentageAmount = -effect.CustomParameters[0],
                                Id = "LoseHealthWhileAboveHalfAndRegenWhileBelow - LoseHealth"
                            });
                        }
                        if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.5f && plusRegen == null) {
                            Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, effect.SourceOfEffect) {
                                RegenerationPercentageAmount = effect.CustomParameters[1],
                                Id = "LoseHealthWhileAboveHalfAndRegenWhileBelow - RestoreHealth"
                            });
                        }
                    })
                }
            };
        }
        else if (effect_name == "GainArmorBasedOnMissingHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "GainArmorBasedOnMissingHealth" + special_id);
                        if(drBuff == null) {
                            drBuff = new Effect_ChangeStat(Player.Instance.Armor, effect.SourceOfEffect) {
                                FlatAmount = effect.FlatAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum),
                                Id="GainArmorBasedOnMissingHealth" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.FlatAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 0)
                            };
                            Player.Instance.AddEffect(drBuff);
                        }
                        else {
                            drBuff.PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                            drBuff.UIText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 0);
                        }
                    })
                }
            };
        }
        else if (effect_name == "ResistDeathAndGainDamageAgainstAttacker")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.RESIST_FATAL_BLOW_AND_RESTORE_HEALTH_PER_PB, new List<float> { PB.REQUIRES__300_SECOND_COOLDOWN });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__PLAYER_SUFFERING_A_FATAL_BLOW, PB.AFFECTS_ONLY__ENEMY_WHO_LANDED_FATAL_BLOW });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float>() {calculatedPB1, calculatedPB2, 300},
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), "300"},
                    ConditionCheckOnAboutToHandleFatalBlow = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("ResistDeathAndGainDamageAgainstAttacker" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), effect.CustomParameters[2], Player.Instance, "ResistDeathAndGainDamageAgainstAttacker" + special_id));
                        damage.WillBeFatalBlow = false;
                        Player.Instance.Health.Current = effect.CustomParameters[0];
                        Player.Instance.AddEffect(new Effect_Invincible(effect.SourceOfEffect) {ShowsInUI = false});
                        Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(effect_name)) {
                            DamagePercentageModifier = effect.CustomParameters[1],
                            ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage2, effect2) =>
                                damage2.SourceOfDamage.User == Player.Instance && damage2.TargetOfDamage == damage.SourceOfDamage.User
                            ),
                        });
                    })
                }
            };
        }
        else if (effect_name == "HeavyDamageAppliesBurn")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BURN_PER_PB, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    RemainsActiveInOtherStances = true,
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "5"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Heavy && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("HeavyDamageAppliesBurn" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Burn(effect.FlatAmount, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), 5, Player.Instance, "HeavyDamageAppliesBurn" + special_id));
                    })
                }
            };
        }
        else if (effect_name == "BlazingShadowWatchesYourBack")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.ARMOR_PER_PB, new List<float> { PB.HAPPENS_UPON__GETTING_DAMAGED_FROM_BEHIND });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BURN_PER_PB, PB.HAPPENS_UPON__GETTING_DAMAGED_FROM_BEHIND });
            return new List<Effect> {
                new Effect_BlazingShadowWatchesYourBack(new(effect_name)) {
                    ExtraArmorAgainstBackstabs = calculatedPB1,
                    BurnScalingInflictedToBackstabbers = calculatedPB2
                }
            };
        }
        else if (effect_name == "BurnAmountWhileBelowNHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_BELOW_50P_HEALTH });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, calculatedPB, new(effect_name)){
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "50"},
                    ConditionForEffectPowerChange = new Func<Unit, bool>(target =>
                        Player.Instance.Health.Current < Player.Instance.Health.Maximum * 0.5f
                    )
                }
            };
        }
        else if (effect_name == "BasicAttacksRestoreHealBasedOnMissingHealth")
        {
            float powerBudget = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = powerBudget,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(powerBudget)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FlatAmount * Player.Instance.Health.Missing / 100;
                    })
                }
            };
        }
        else if (effect_name == "BasicAttacksDealMoreDamageBasedOnMissingHealth")
        {
            float powerBudget = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM, PB.AFFECTS_ONLY__BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = powerBudget,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(powerBudget)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "GainDamageBasedOnMissingHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeCompositeStat damageBuff = (Effect_ChangeCompositeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "GainDamageBasedOnMissingHealth" + special_id);
                        if(damageBuff == null) {
                            damageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, effect.SourceOfEffect) {
                                PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum),
                                Id="GainDamageBasedOnMissingHealth" + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 0)
                            };
                            Player.Instance.AddEffect(damageBuff);
                        }
                        else {
                            damageBuff.PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                            damageBuff.UIText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 0);
                        }
                    })
                }
            };
        }


        //Knight
        else if (effect_name == "GainTenacityAfterBeingHit")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> { PB.REQUIRES__PLAYER_WAS_DAMAGED_IN_LAST_10_SECONDS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Tenacity, effect.SourceOfEffect) {
                            FlatAmount = effect.FlatAmount,
                            ShowsInUI = true,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount),
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="GainTenacityAfterBeingHit" + special_id
                        }, 10);
                    })
                }
            };
        }
        else if (effect_name == "GainArmorAfterBeingHit")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__PLAYER_WAS_DAMAGED_IN_LAST_10_SECONDS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Armor, effect.SourceOfEffect) {
                            FlatAmount = effect.FlatAmount,
                            ShowsInUI = true,
                            UIText = Utils.GetFormattedFloat(effect.FlatAmount),
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            Id="GainArmorAfterBeingHit" + special_id
                        }, 10);
                    })
                }
            };
        }
        else if (effect_name == "BlockXAmountOfDamageOnceEveryNSeconds")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.BLOCK_DAMAGE_PER_PB, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float>() { calculatedPB, 10},
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                    ShowsInUI = true,
                    HideInUIWhileCooldownWithIdExists = "BlockXAmountOfDamageOnceEveryNSeconds" + special_id,
                    PathToUIGraphic = "UI/Barrier",
                    UIText =  Utils.GetFormattedFloat(calculatedPB, 0),
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("BlockXAmountOfDamageOnceEveryNSeconds" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Injury -= effect.CustomParameters[0];
                        damage.Stagger -= effect.CustomParameters[0];
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), effect.CustomParameters[1], Player.Instance, "BlockXAmountOfDamageOnceEveryNSeconds" + special_id));
                    })
                }
            };
        }
        else if (effect_name == "BurnAmountOnPlayer")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DECREASE_STACKING_EFFECT_RECEIVED_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name))};
        }
        else if (effect_name == "FreezeAmountOnPlayer")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DECREASE_STACKING_EFFECT_RECEIVED_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name))};
        }
        else if (effect_name == "PlundererEmpower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FAMILY_TECHNIQUES, PB.AFFECTS_ONLY__10_SECONDS, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User.IsHostile && Vector2.Distance(Player.Instance.transform.position, ability.User.transform.position) < 10 && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("PlundererAbilityAmplify") && ability.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static) != null && !Player.Instance.CheckIfUnderEffect(typeof(Effect_PlundererAbilityAmplify))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_PlundererAbilityAmplify(effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount,
                            AmplifiedFamily = (Ability.AbilityFamily)ability.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static).GetValue(null),
                            Id = "PlundererAbilityAmplify",
                        }, 10);
                        Player.Instance.AddCooldown(typeof(Effect), 30, "PlundererAbilityAmplify");
                    })
                }
            };
        }
        else if (effect_name == "PlundererArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__ENEMY_USING_FAMILY_TECHNIQUE, PB.HAPPENS_UPON__USING_TECHNIQUE, PB.AFFECTS_ONLY__30_SECONDS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Player.Instance.CheckIfUnderEffectWithGivenId("PlundererAbilityAmplify")
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_ChangeStat(Player.Instance.Armor, effect.SourceOfEffect) {
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            FlatAmount = effect.FlatAmount
                        });
                    })
                }
            };
        }


        //Judge
        else if (effect_name == "WhenEnemyExitsCrowdControlApplyExtraFrozen")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STUN_PER_PB, new List<float> { PB.HAPPENS_UPON__ENEMY_EXITING_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.TargetOfEffect.IsHostile
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectEnding, effect) =>  {
                        effectEnding.TargetOfEffect.AddEffect(new Effect_Frozen(effect.SourceOfEffect), effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "GainEnergyWhenApplyingFrozen")
        {
            float calculatedPB = CalculatePB(power_budget, PB.GAIN_FLAT_ENERGY_AFFECTED_BY_ENERGY_GAIN_PER_PB, new List<float> { PB.HAPPENS_UPON__FREEZING_AN_ENEMY });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_Frozen)) && effect.TargetOfEffect.IsHostile
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectStarting, effect) =>  {
                        Player.Instance.Energy.GenerateEnergy(effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "PushAwayAndFreezeUponFallingBelowHalfHealth")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.333f, PB.KNOCK_BACK_METERS_PER_PB, new List<float> { PB.SPECIAL__AFFECTS_EVERY_ENEMY_WITHIN_RANGE });
            float calculatedPB2 = CalculatePB(power_budget * 0.666f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.FREEZE_PER_PB, PB.SPECIAL__AFFECTS_EVERY_ENEMY_WITHIN_RANGE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2 },
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health && stat.Current < stat.Maximum * 0.5f && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("PushAwayAndFreezeUponFallingBelowHalfHealth" + special_id)
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        List<Unit> enemies = Utils.GetSpecifiedUnits(new Func<Unit, bool>((unit) => unit.IsHostile && Vector2.Distance(Player.Instance.transform.position, unit.transform.position) < effect.CustomParameters[0]));
                        foreach(Unit enemy in enemies) {
                            float pushbackDistance = effect.CustomParameters[0] - Vector2.Distance(Player.Instance.transform.position, enemy.transform.position);
                            enemy.PushInTargetDirection((Player.Instance.transform.position - enemy.transform.position).normalized * pushbackDistance, null);
                            enemy.AddEffect(new Effect_Freeze(effect.CustomParameters[1], effect.SourceOfEffect));
                        }
                        Player.Instance.AddCooldown(typeof(Effect), 30, "PushAwayAndFreezeUponFallingBelowHalfHealth" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "ArmorWhileAbove50PHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_ABOVE_50P_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect armorBuff = Player.Instance.GetEffectWithGivenId("ArmorWhileAbove50PHealth - Outfit_Judge");
                        if(Player.Instance.Health.Current < Player.Instance.Health.Maximum * 0.5f && armorBuff != null) {
                            armorBuff.EndThisEffect();
                        }
                        if(Player.Instance.Health.Current >= Player.Instance.Health.Maximum * 0.5f && armorBuff == null) {
                            Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Armor, effect.SourceOfEffect) {
                                FlatAmount = effect.FlatAmount,
                                Id = "ArmorWhileAbove50PHealth - Outfit_Judge"
                            });
                        }
                    })
                }
            };
        }
        else if (effect_name == "DamagingAnEnemyFreezesThem")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.FREEZE_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Freeze(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "IgnorePortionOfEnemyTenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_TENACITY_REDUCTION_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_Id("TenacityPenetration" + special_id, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "BasicAttacksDecreaseEnemyTenacity")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_TENACITY_REDUCTION_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.AFFECTS_ONLY__15_SECONDS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "15"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect e = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, effect.SourceOfEffect) {
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                            PathToUIGraphic = "UI/Control",
                            ShowsInUI = true,
                            FlatAmount = -effect.FlatAmount,
                            Id = "BasicAttacksDecreaseEnemyTenacity"
                        };
                        damage.TargetOfDamage.AddEffect(e, 15);
                    })
                }
            };
        }
        else if (effect_name == "ApplySleepWithBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STUN_PER_PB, new List<float> { PB.SLEEP_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Sleep(effect.SourceOfEffect), effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "DamageToSleeping")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__SLEEPING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Sleep))
                    ),
                }
            };
        }
        else if (effect_name == "ApplyStunWithBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STUN_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Stun(effect.SourceOfEffect), effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "RangedTechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged && damage.SourceOfDamage.Is(Ability.Property.Technique)
                    ),
                }
            };
        }
        else if (effect_name == "DamageToStunned")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STUNNED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Stun))
                    ),
                }
            };
        }


        //Gunslinger
        else if (effect_name == "DealExtraDamageToEnemiesFarAway")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__GIVEN_ENEMY_IS_AT_LEAST_5M_AWAY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    DamagePercentageModifier = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Vector2.Distance(damage.TargetOfDamage.transform.position, Player.Instance.transform.position) >= 5
                    ),
                }
            };
        }
        else if (effect_name == "WhileInRangedStanceDecreaseArmorButIncreaseDamage")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__ONE_STANCE });
            float calculatedPB2 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ONE_STANCE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2 },
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect_ChangeStat armorDebuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "WhileInRangedStanceDecreaseArmorButIncreaseDamage - ArmorDebuff - " + special_id);
                        Effect_ChangeCompositeStat damageBuff = (Effect_ChangeCompositeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Id == "WhileInRangedStanceDecreaseArmorButIncreaseDamage - DamageBuff - " + special_id);
                        if(Player.Instance.CurrentStance.DamageType != Constants.DamageType.Ranged && armorDebuff != null && damageBuff != null) {
                            armorDebuff.EndThisEffect();
                            damageBuff.EndThisEffect();
                        }
                        else if(Player.Instance.CurrentStance.DamageType == Constants.DamageType.Ranged && armorDebuff == null && damageBuff == null){
                            armorDebuff = new Effect_ChangeStat(Player.Instance.Armor, new("WhileInRangedStanceDecreaseArmorButIncreaseDamage - ArmorDebuff - " + special_id)) {
                                FlatAmount = -effect.CustomParameters[0],
                                BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                                Id = "WhileInRangedStanceDecreaseArmorButIncreaseDamage - ArmorDebuff - " + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(-effect.CustomParameters[0])
                            };
                            Player.Instance.AddEffect(armorDebuff);
                            damageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new("WhileInRangedStanceDecreaseArmorButIncreaseDamage - DamageBuff - " + special_id)) {
                                FlatAmount = effect.CustomParameters[1],
                                BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameId,
                                Id = "WhileInRangedStanceDecreaseArmorButIncreaseDamage - DamageBuff - " + special_id,
                                IsRemovable = false,
                                ShowsInUI = true,
                                UIText = Utils.GetFormattedFloat(effect.CustomParameters[1])
                            };
                            Player.Instance.AddEffect(damageBuff);
                        }
                    })
                }
            };
        }
        else if (effect_name == "ArmorPenetration")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_ARMOR_PENETRATION_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorPenetrationModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    )
                }
            };
        }
        else if (effect_name == "RestoreXAmmoEachTimeYouDealDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "5"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("RestoreXAmmoEachTimeYouDealDamage" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Ammo += effect.PercentageAmount / 100;
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), 5, Player.Instance, "RestoreXAmmoEachTimeYouDealDamage" + special_id));
                    })
                }
            };
        }
        else if (effect_name == "GainMovementSpeedWhenThereIsAnEnemyNearYou")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB, new List<float> { PB.REQUIRES__ANY_ENEMY_IS_IN_5M_RANGE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "5"},
                    FlatAmount = calculatedPB,
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Utils.GetAllUnits(true, true).FirstOrDefault(enemy => Vector2.Distance(enemy.transform.position, Player.Instance.transform.position) < 5) != null
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.MovementSpeed, effect.SourceOfEffect) {FlatAmount = effect.FlatAmount}, 0.1f);
                    })
                }
            };
        }
        else if (effect_name == "DealIncreasedDamageBasedOnFlightTime")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.75f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__MAX_VALUE_WHEN_ENEMY_IS_15M_AWAY });
            float calculatedPB2 = CalculatePB(power_budget * 0.25f, PB.PROJECTILE_MAX_DISTANCE_INCREASE_PERCENTAGE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_GainProjectileSpeedAndDamageWithDistanceTravelled(calculatedPB1, calculatedPB2, new(effect_name)) {}
            };
        }
        else if (effect_name == "ArmorPenetrationBasedOnFlightTime")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_ARMOR_PENETRATION_PER_PB, new List<float> { PB.REQUIRES__MAX_VALUE_WHEN_ENEMY_IS_15M_AWAY });
            return new List<Effect> {
                new Effect_Id("ArmorPenetrationBasedOnFlightTime" + special_id, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "FinalAmmoDealsIncreasedDamageButHasCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(DamageInstance.DamageProperty.FinalAmmo) && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("FinalAmmoDealsIncreasedDamageButHasCooldown" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), 10, Player.Instance, "FinalAmmoDealsIncreasedDamageButHasCooldown" + special_id));
                    })
                }
            };
        }
        else if (effect_name == "RestoreAmmoWhileBelow1Ammo")
        {
            float calculatedPB = CalculatePB(power_budget, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> { PB.REQUIRES__BELOW_1_AMMO });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Player.Instance.Ammo < 1
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.Ammo += effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "DealMoreDamageAndKnockbackToCloserEnemies")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.75f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__GIVEN_ENEMY_IS_IN_5M_RANGE });
            float calculatedPB2 = CalculatePB(power_budget * 0.25f, PB.KNOCK_BACK_METERS_PER_PB, new List<float> { PB.REQUIRES__GIVEN_ENEMY_IS_IN_5M_RANGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB1,
                    FlatAmount = calculatedPB2,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), "5"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Vector2.Distance(damage.TargetOfDamage.transform.position, Player.Instance.transform.position) < 5
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.DamageDealtPercentageModifier += effect.PercentageAmount;
                        damage.TargetOfDamage.ApplyKnockback(effect.FlatAmount, Player.Instance.transform.position, damage.SourceOfDamage);
                    })
                }
            };
        }
        else if (effect_name == "RefundAmmoIfEnemyHitByBasicAttackWasClose")
        {
            float calculatedPB = CalculatePB(power_budget, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> { PB.REQUIRES__GIVEN_ENEMY_IS_IN_5M_RANGE, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Vector2.Distance(damage.TargetOfDamage.transform.position, Player.Instance.transform.position) < 5
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Ammo += effect.PercentageAmount / 100;
                    })
                }
            };
        }

        //Arbiter
        else if (effect_name == "ConvertInjuryToStaggerAgainstNonStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__NON_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.IsStaggered
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Stagger += damage.Injury * effect.PercentageAmount / 100;
                        damage.Injury -= damage.Injury * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "InjuryToNonStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__NON_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    InjuryPercentageModifier = calculatedPB,
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.IsStaggered
                    ),
                }
            };
        }
        else if (effect_name == "DealingDamageProlongsStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STUN_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.AFFECTS_ONLY__STAGGERED, PB.AFFECTS_ONLY__STUN_CANNOT_EXCEED_DOUBLE_ITS_ORIGINAL_LENGTH });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(Constants.DEFAULT_HARD_STAGGERED_DURATION * 2)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.IsStaggered && damage.TargetOfDamage.GetEffect(typeof(Effect_HardStaggered)).ElapsedDuration < Constants.DEFAULT_HARD_STAGGERED_DURATION * 2
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect staggered = damage.TargetOfDamage.GetEffect(typeof(Effect_HardStaggered));
                        if (staggered != null)
                        {
                            staggered.ProlongDuration(effect.FlatAmount);
                        }
                    })
                }
            };
        }
        else if (effect_name == "StaggeringAnEnemyHealsOnceEveryNSeconds")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__STAGGERING_AN_ENEMY });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.TargetOfEffect.IsHostile
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.Health.Current += effect.FlatAmount;
                    })
                }
            };
        }
        else if (effect_name == "EnemiesRegainStaggerBarXPercentSlower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.GLOBAL_STAGGER_BAR_REGENERATION_REDUCTION_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_Id("EnemiesRegainStaggerBarXPercentSlower" + special_id, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ActionOnStart = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.GlobalEnemyStaggerBarRegenerationModifier -= effect.PercentageAmount;
                    }),
                    ActionOnEnd = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.GlobalEnemyStaggerBarRegenerationModifier += effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "DamageToAboveHalfStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMY_ABOVE_50P_STAGGER_BAR });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    DamagePercentageModifier = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.StaggerBar.Current >= damage.TargetOfDamage.StaggerBar.Maximum * 0.5f
                    ),
                }
            };
        }
        else if (effect_name == "DealExtraHeavyStaggerWithCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.REQUIRES__15_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "15"},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("DealExtraHeavyStaggerWithCooldown" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtFlatModifier += effect.FlatAmount;
                        Player.Instance.AddCooldown(typeof(Effect), 15, "DealExtraHeavyStaggerWithCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "RefundCooldownOnHittingStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGERED, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.REQUIRES__15_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "15"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("RefundCooldownOnHittingStaggered" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.ReduceAllRemainingCooldowns(effect.PercentageAmount);
                        Player.Instance.AddCooldown(typeof(Effect), 15, "DealExtraHeavyStaggerWithCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "ApplyLethargySlowAndProneOnExitingStaggered")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.2f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SLOW_PER_PB, PB.HAPPENS_UPON__ENEMY_EXITING_STAGGERED });
            float calculatedPB2 = CalculatePB(power_budget * 0.4f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.LETHARGY_PER_PB, PB.HAPPENS_UPON__ENEMY_EXITING_STAGGERED });
            float calculatedPB3 = CalculatePB(power_budget * 0.4f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.PRONE_PER_PB, PB.HAPPENS_UPON__ENEMY_EXITING_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2, calculatedPB3 },
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB1),Utils.GetFormattedFloat(calculatedPB3)},
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.TargetOfEffect.IsHostile
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectEnding, effect) =>  {
                        effectEnding.TargetOfEffect.AddEffect(new Effect_Frozen(effect.SourceOfEffect), effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "StaggerToNonStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__NON_STAGGERED });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.IsStaggered
                    ),
                }
            };
        }
        else if (effect_name == "DealExtraRangedStaggerWithCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("DealExtraRangedStaggerWithCooldown" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtFlatModifier += effect.FlatAmount;
                        Player.Instance.AddCooldown(typeof(Effect), 30, "DealExtraRangedStaggerWithCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "RangedDamageStaggersEnemiesWithCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.PRONE_PER_PB, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("RangedDamageStaggersEnemiesWithCooldown" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(damage.TargetOfDamage.IsStaggered == false) {
                            damage.StaggerDealtFlatModifier += 1000000;
                        }
                        damage.TargetOfDamage.AddEffect(new Effect_Prone(effect.FlatAmount, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(typeof(Effect), 30, "RangedDamageStaggersEnemiesWithCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "OnSwitchingToWeaponSpawnAMarkerThatDealsMassiveStaggerAndStunOnHit")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.666f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STAGGER, PB.REQUIRES__30_SECOND_COOLDOWN, PB.REQUIRES__HITTING_SPAWNED_MARK });
            float calculatedPB2 = CalculatePB(power_budget * 0.333f, PB.SECONDS_OF_STUN_PER_PB, new List<float> { PB.REQUIRES__30_SECOND_COOLDOWN, PB.REQUIRES__HITTING_SPAWNED_MARK });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), "30"},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Unit u = Player.Instance.CurrentTarget != null ? Player.Instance.CurrentTarget : Player.Instance.GetClosestValidTarget();
                        GameObject mark = Utils.CreateVisualEffect(effect.SourceOfEffect, "SpecialMark", u.transform.position.x, u.transform.position.y);
                        mark.name = "SpecialMark" + special_id;
                        mark.transform.SetParent(u.transform);
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2 },
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfCollision.gameObject.name == "SpecialMark" + special_id
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        GameObject.Destroy(damage.SourceOfCollision.gameObject);
                        damage.StaggerDealtFlatModifier += effect.CustomParameters[0];
                        damage.TargetOfDamage.AddEffect(new Effect_Stun(effect.SourceOfEffect), effect.CustomParameters[1]);
                    })
                }
            };
        }

        //IronBlooded
        else if (effect_name == "RegeneratePortionOfInjuryTakenAsHealthOver30Seconds")
        {
            float calculatedPB = CalculatePB(power_budget, PB.HEALTH_RESTORED_PER_INJURY_TAKEN_OVER_30_SECONDS_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.InjuryDealt > 0 && damage.IsNot(DamageInstance.DamageProperty.DamageOverTime)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, effect.SourceOfEffect) {
                            RegenerationFlatAmount = damage.InjuryDealt * effect.PercentageAmount / 100
                        }, 30);
                    })
                }
            };
        }
        else if (effect_name == "ReflectInjuryTakenBackAtAttacker")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_DAMAGE_TAKEN_REFLECTED_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.InjuryDealt > 0 && damage.IsNot(DamageInstance.DamageProperty.DamageOverTime)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        new DamageInstance(damage.TargetOfDamage, damage.SourceOfDamage, damage.DamagingObject) {
                            InjuryDealtFlatModifier = damage.InjuryDealt * effect.PercentageAmount / 100
                        }.CalculateAndApplyDamage();
                    })
                }
            };
        }
        else if (effect_name == "ArmorIsPartiallyEffectiveWhileStaggered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_ARMOR_RETAINED_WHILE_STAGGERED_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && Player.Instance.IsStaggered
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.RetainedArmorPercentageWhileStaggered = effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "ConvertXPercentOfStaggerDealtToYouIntoInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_HALF_OF_DAMAGE_TYPE_DEALT_TO_PLAYER_INTO_THE_OTHER_AND_REDUCE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.Stagger > 0
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Injury += damage.Stagger * 0.5f * (1 - effect.PercentageAmount / 100);
                        damage.Stagger *= 0.5f;
                    })
                }
            };
        }
        else if (effect_name == "AfterGettingHitIncreaseDamageOfNextAttack")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && (damage.InjuryDealt > 0 || damage.StaggerDealt > 0)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_CustomizableDamageChange(effect.SourceOfEffect) {
                            DamagePercentageModifier = effect.PercentageAmount,
                            ShowsInUI = true,
                            PathToUIGraphic = "UI/Damage",
                            ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                                damage.SourceOfDamage.User == Player.Instance
                            ),
                        });
                    })
                }
            };
        }




        //Unbreakable
        else if (effect_name == "ArmorWhileBlocking")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__BLOCKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Block))
                    )
                }
            };
        }
        else if (effect_name == "ProtectFromFlinchingOnce")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_Id("ProtectFromFlinchingOnce" + special_id, new(effect_name)) {
                    FlatAmount = 30 * (1 - calculatedPB / 100),
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "ReflectPortionOfBlockedDamageBackAtAttacker")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_DAMAGE_TAKEN_REFLECTED_PER_PB, new List<float> { PB.REQUIRES__BLOCKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Block))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        new DamageInstance(damage.TargetOfDamage, damage.SourceOfDamage, damage.DamagingObject) {
                            InjuryDealtFlatModifier = damage.InjuryDealt * effect.PercentageAmount / 100,
                            StaggerDealtFlatModifier = damage.InjuryDealt * effect.PercentageAmount / 100
                        }.CalculateAndApplyDamage();
                    })
                }
            };
        }
        else if (effect_name == "CancelPlayerStaggeredPerCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_STAGGER_BAR_RESTORED_PER_PB, new List<float> { PB.REQUIRES__PLAYER_STAGGERED, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect.GetType().IsSubclassOf(typeof(Effect_HardStaggered)) && effect.TargetOfEffect == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("CancelPlayerStaggeredPerCooldown" + special_id)
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.GetEffect(typeof(Effect_HardStaggered)).EndThisEffect();
                        Player.Instance.StaggerBar.Current = Player.Instance.StaggerBar.Maximum - effect.FlatAmount;
                        Player.Instance.AddCooldown(typeof(Effect), 30, "CancelPlayerStaggeredPerCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "ConvertXPercentOfInjuryDealtToYouIntoStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_HALF_OF_DAMAGE_TYPE_DEALT_TO_PLAYER_INTO_THE_OTHER_AND_REDUCE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.Injury > 0
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Injury *= 0.5f;
                        damage.Stagger += damage.Injury * 0.5f * (1 - effect.PercentageAmount / 100);
                    })
                }
            };
        }
        else if (effect_name == "HeavyDamageStealsStaggerBarPerCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.STEAL_1P_OF_ENEMY_STAGGER_BAR_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("HeavyDamageStealsStaggerBarPerCooldown" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtFlatModifier += damage.TargetOfDamage.StaggerBar.Maximum * effect.PercentageAmount / 100;
                        Player.Instance.StaggerBar.Current -= damage.TargetOfDamage.StaggerBar.Maximum * effect.PercentageAmount / 100;
                        Player.Instance.AddCooldown(typeof(Effect), 5, "HeavyDamageStealsStaggerBarPerCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "BasicAttacksRestorePercentageOfStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.STEAL_1P_OF_ENEMY_STAGGER_BAR_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("HeavyDamageStealsStaggerBarPerCooldown" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtFlatModifier += damage.TargetOfDamage.StaggerBar.Maximum * effect.PercentageAmount / 100;
                        Player.Instance.StaggerBar.Current -= damage.TargetOfDamage.StaggerBar.Maximum * effect.PercentageAmount / 100;
                        Player.Instance.AddCooldown(typeof(Effect), 5, "HeavyDamageStealsStaggerBarPerCooldown" + special_id);
                    })
                }
            };
        }


        //Mercenary
        else if (effect_name == "StrongBasicAttacksGiveBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__STRONG_BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "DealMoreDamageBasedOnBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_DAMAGING_STACKING_EFFECT_ON_PLAYER * PB.BARRIER_PER_PB, PB.REQUIRES__BARRIER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Barrier))
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.DamageDealtPercentageModifier = Player.Instance.GetEffect(typeof(Effect_Barrier)).DecayingAmount * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "GainBarrierUponFallingBelow25PHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__PLAYER_FALLING_BELOW_25P_HEALTH, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health && stat.Current < stat.Maximum * 0.25f && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("GainBarrierUponFallingBelow25PHealth" + special_id)
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(typeof(Effect), 30, "GainBarrierUponFallingBelow25PHealth" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "GainPortionOfDamageTakenAsBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_1P_OF_DAMAGE_TAKEN_INTO_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && (damage.InjuryDealt > 0 || damage.StaggerDealt > 0)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier((damage.InjuryDealt + damage.StaggerDealt) * effect.PercentageAmount / 100, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "BlockingGivesBarrierPerCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__BLOCKING, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> { Utils.GetFormattedFloat(calculatedPB), "10" },
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.GetType().IsSubclassOf(typeof(Ability_Block)) && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("BlockingGivesBarrierPerCooldown" + special_id)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent>((ability, effect) =>
                    {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(typeof(Effect), 10, "BlockingGivesBarrierPerCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "BlockingGivesInvinciblePerCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_INVINCIBILITY_PER_PB, new List<float> { PB.HAPPENS_UPON__BLOCKING, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> { Utils.GetFormattedFloat(calculatedPB), "10" },
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.GetType().IsSubclassOf(typeof(Ability_Block)) && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("BlockingGivesInvinciblePerCooldown" + special_id)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent>((ability, effect) =>
                    {
                        Player.Instance.AddEffect(new Effect_Invincible(effect.SourceOfEffect), effect.FlatAmount);
                        Player.Instance.AddCooldown(typeof(Effect), 10, "BlockingGivesInvinciblePerCooldown" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "BasicAttacksGrantBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }


        //Survivor
        else if (effect_name == "HealthRestorationPower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.INCREASED_HEALTH_RESTORATION_POWER_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_Id("HealthRestorationPower" + special_id, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ActionOnStart = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.HealthRestorationPower.AddFlatModifier(effect, effect.PercentageAmount);
                    }),
                    ActionOnEnd = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.HealthRestorationPower.RemoveFlatModifier(effect, effect.PercentageAmount);
                    })
                }
            };
        }
        else if (effect_name == "GainHealthRegenerationThatIsTripledWhenBelowHalfHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_SECOND_PER_PB, new List<float> { PB.AFFECTS_ONLY__EFFECT_TRIPLED_WHILE_BELOW_50P_HEALTH });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    Id = "GainHealthRegenerationThatIsTripledWhenBelowHalfHealth" + special_id,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    RegenerationFlatAmount = calculatedPB
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Armor},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat regen = (Effect_ChangeStat)Player.Instance.GetEffectWithGivenId("GainHealthRegenerationThatIsTripledWhenBelowHalfHealth" + special_id);
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum / 2 && regen.RegenerationFlatAmount != effect.FlatAmount) {
                            regen.RegenerationFlatAmount = effect.FlatAmount;
                        }
                        if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum / 2 && regen.RegenerationFlatAmount != effect.FlatAmount * 3) {
                            regen.RegenerationFlatAmount = effect.FlatAmount * 3;
                        }
                    })
                }
            };
        }
        else if (effect_name == "GainInvincibleUponFallingBelow25PHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__PLAYER_FALLING_BELOW_25P_HEALTH, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health && stat.Current < stat.Maximum * 0.25f && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("GainBarrierUponFallingBelow25PHealth" + special_id)
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(typeof(Effect), 30, "GainBarrierUponFallingBelow25PHealth" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "HealFromInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.InjuryDealt > 0
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "DealMoreDamageWhileAtFullHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__PLAYER_FULL_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Player.Instance.Health.Current >= Player.Instance.Health.Maximum
                    )
                }
            };
        }
        else if (effect_name == "GainArmorWhileAtFullHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__PLAYER_FULL_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Player.Instance.Health.Current >= Player.Instance.Health.Maximum
                    )
                }
            };
        }
        else if (effect_name == "RestoreHealthWhenBasicAttacking")
        {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FlatAmount;
                    })
                }
            };
        }
        else if (effect_name == "BleedRestoresHealthInsteadOfDealingInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage.IsHostile && damage.Is(DamageInstance.DamageProperty.Bleed)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Injury = 0;
                        Player.Instance.Health.Current += damage.Injury * effect.PercentageAmount / 100;
                    })
                }
            };
        }

        //Assassin
        else if (effect_name == "ReducedBackstabCooldown")
        {
            float calculatedPB = CalculatePB(power_budget, PB.BACKSTAB_REDUCED_COOLDOWN_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_Id("ReducedBackstabCooldown" + special_id, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ActionOnStart = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.BackstabCooldown -= effect.FlatAmount;
                    }),
                    ActionOnEnd = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.BackstabCooldown += effect.FlatAmount;
                    })
                }
            };
        }
        else if (effect_name == "BackstabDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BACKSTABS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab)
                    ),
                }
            };
        }
        else if (effect_name == "IncreasedBackstabDamageWhileStealthed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BACKSTABS, PB.REQUIRES__STEALTH });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Stealth))
                    ),
                }
            };
        }
        else if (effect_name == "GainStealthUponFallingBelow50PHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STEALTH_PER_PB, new List<float> { PB.HAPPENS_UPON__PLAYER_FALLING_BELOW_50P_HEALTH, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Health && stat.Current < stat.Maximum * 0.5f && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("GainStealthUponFallingBelow50PHealth" + special_id)
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Stealth(effect.SourceOfEffect), effect.FlatAmount);
                        Player.Instance.AddCooldown(typeof(Effect), 10, "GainStealthUponFallingBelow50PHealth" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "DamageWhileStealthed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.REQUIRES__STEALTH });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Stealth))
                    ),
                }
            };
        }
        else if (effect_name == "StealthDuration")
        {
            float calculatedPB = CalculatePB(power_budget, PB.STEALTH_DURATION_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_Id("ReducedBackstabCooldown" + special_id, new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ActionOnStart = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.StealthDuration += effect.PercentageAmount / 100;
                    }),
                    ActionOnEnd = new Action<Effect_Id> ((effect) =>  {
                        Player.Instance.BackstabCooldown -= effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "GainStealthOnceEveryNSeconds")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STEALTH_PER_PB, new List<float> { PB.HAPPENS_UPON__PLAYER_FALLING_BELOW_50P_HEALTH, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_GainStealthOnceEveryNSeconds(10, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                }
            };
        }
        else if (effect_name == "MovementSpeedDuringStealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB, new List<float> { PB.REQUIRES__STEALTH });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) =>
                        effect is Effect_Stealth && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.MovementSpeed, effect.SourceOfEffect) {
                            FlatAmount = effect.FlatAmount,
                            Id = "MovementSpeedDuringStealth" + special_id
                        });
                    }),
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) =>
                        effect is Effect_Stealth && effect.TargetOfEffect == Player.Instance
                    ),
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect e = Player.Instance.GetEffectWithGivenId("MovementSpeedDuringStealth" + special_id);
                        if(e != null && e.EffectEnded == false) {
                            e.EndThisEffect();
                        }
                    }),
                }
            };
        }
        else if (effect_name == "HeavyDamageAppliesMassiveInjuryAndEvenMoreIfBackstab")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.7f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.REQUIRES__20_SECOND_COOLDOWN });
            float calculatedPB2 = CalculatePB(power_budget * 0.3f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.REQUIRES__20_SECOND_COOLDOWN, PB.HAPPENS_UPON__BACKSTABING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2 },
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), "20"},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("HeavyDamageAppliesMassiveInjuryAndEvenMoreIfBackstab" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.InjuryDealtFlatModifier += effect.CustomParameters[0] + (damage.SourceOfDamage.Is(Ability.Property.Backstab) ? effect.CustomParameters[1] : 0);
                        Player.Instance.AddCooldown(typeof(Effect), 20, "HeavyDamageAppliesMassiveInjuryAndEvenMoreIfBackstab" + special_id);
                    })
                }
            };
        }
        else if (effect_name == "TakedownsGiveStealthAndEmpowerNextAttack")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.STEALTH_DURATION_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__TAKEDOWN });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ONCE, PB.HAPPENS_UPON__TAKEDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB1,
                    PercentageAmount = calculatedPB2,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckForTakedown = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnTakedown = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Stealth(effect.SourceOfEffect), effect.FlatAmount);
                        Player.Instance.AddEffect(new Effect_EmpowerNextAttack(effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount
                        });
                    })
                }
            };
        }
        else if (effect_name == "BasicAttacksAndTechniquesPerformedWhileInStealthCountAsBackstab")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BACKSTABS, PB.SPECIAL__MAKES_BASIC_ATTACKS_AND_TECHNIQUES_PERFORMED_IN_STEALTH_COUNT_AS_BACKSTABS });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Stealth)) && (ability.Is(Ability.Property.BasicAttack) || ability.Is(Ability.Property.Technique))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        ability.Properties.Add(Ability.Property.Backstab);
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Backstab)
                    ),
                }
            };
        }
        else if (effect_name == "BasicAttacksAndTechniquesDontEndStealthImmediately")
        {
            float calculatedPB = CalculatePB(power_budget, PB.SECONDS_OF_STEALTH_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__STEALTH, PB.REQUIRES__60_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_Id("BasicAttacksAndTechniquesDontEndStealthImmediately" + special_id, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }

        //Executioner
        else if (effect_name == "ConvertStaggerDealtIntoInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { 1 / PB.AFFECTS_ONLY__STAGGER, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.Stagger > 0
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Stagger += damage.Injury * effect.PercentageAmount / 100;
                        damage.Injury -= damage.Injury * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "SalutisTechniqueDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FAMILY_TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && Technique.GetFamily(damage.SourceOfDamage.GetType()) == Ability.AbilityFamily.Salutis
                    )
                }
            };
        }
        else if (effect_name == "Every5thHitDealsMassivelyIncreasedDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__5TH_ATTACK });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = 0,
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(effect.FlatAmount == 4) {
                            effect.FlatAmount = 0;
                            damage.DamageDealtPercentageModifier += effect.PercentageAmount;
                        }
                        else {
                            effect.FlatAmount++;
                        }
                    })
                }
            };
        }
        else if (effect_name == "Every5thHitLifesteals")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__5TH_ATTACK });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = 0,
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(effect.FlatAmount == 4) {
                            effect.FlatAmount = 0;
                            Player.Instance.Health.Current += damage.InjuryDealt * effect.PercentageAmount / 100 + damage.StaggerDealt * effect.PercentageAmount / 100;
                        }
                        else {
                            effect.FlatAmount++;
                        }
                    })
                }
            };
        }
        else if (effect_name == "DealIncreasedDamageToEnemiesBelow25PHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMY_BELOW_25P_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    DamagePercentageModifier = calculatedPB,
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.Health.Current < damage.TargetOfDamage.Health.Maximum * 0.25f
                    ),
                }
            };
        }
        else if (effect_name == "ArmorAgainstBosses")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__BOSSES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.IsBoss
                    ),
                }
            };
        }
        else if (effect_name == "TakedownsGrantEmpowered")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.HAPPENS_UPON__TAKEDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForTakedown = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnTakedown = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Empowered(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "ArmorAgainstNonBosses")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.AFFECTS_ONLY__NON_BOSSES });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.IsBoss
                    ),
                }
            };
        }
        else if (effect_name == "ReduceCooldownsOnTakedowns")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__TAKEDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForTakedown = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnTakedown = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.ReduceAllRemainingCooldowns(effect.PercentageAmount);
                    })
                }
            };
        }
        else if (effect_name == "FinishOffLowHealthEnemies")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DEAL_FLAT_DAMAGE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMY_BELOW_25P_HEALTH });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForTakedown = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnTakedown = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        new DamageInstance(damage.TargetOfDamage, damage.SourceOfDamage, damage.DamagingObject) {
                            InjuryDealtFlatModifier = 1000000,
                        }.CalculateAndApplyDamage();
                    })
                }
            };
        }
        else if (effect_name == "TakedownsRestoreAmmo")
        {
            float calculatedPB = CalculatePB(power_budget, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__TAKEDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForTakedown = new Func<DamageInstance, bool>((damage) =>
                        damage.TargetOfDamage.IsHostile
                    ),
                    ActionOnTakedown = new Action<DamageInstance, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.Ammo += effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "OnSwitchingToThisWeaponSpawnMarkerThatDealsInjuryOnHit")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__INJURY, PB.AFFECTS_ONLY__WEAPON_TYPE, PB.REQUIRES__45_SECOND_COOLDOWN, PB.REQUIRES__HITTING_SPAWNED_MARK });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "45"},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Unit u = Player.Instance.CurrentTarget != null ? Player.Instance.CurrentTarget : Player.Instance.GetClosestValidTarget();
                        GameObject mark = Utils.CreateVisualEffect(effect.SourceOfEffect, "SpecialMark", u.transform.position.x, u.transform.position.y);
                        mark.name = "SpecialMark" + special_id;
                        mark.transform.SetParent(u.transform);
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged && damage.SourceOfCollision.gameObject.name == "SpecialMark" + special_id
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        GameObject.Destroy(damage.SourceOfCollision.gameObject);
                        damage.InjuryDealtFlatModifier += effect.FlatAmount;
                    })
                }
            };
        }


        //Artisan
        else if (effect_name == "UsingToolsIncreasesToolPowerUntilEndOfCombat")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TOOL_POWER_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__USING_TOOL });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "45"},
                    ConditionCheckForToolUsed = new Func<Item, bool>((tool) =>
                        true
                    ),
                    ActionOnToolUsed = new Action<Item, Effect_CustomizableEffectOnEvent> ((tool, effect) =>  {
                        Player.Instance.ItemPower.AddFlatModifier(effect, effect.FlatAmount);
                    }),
                },
            };
        }
        else if (effect_name == "StancePower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.STANCE_POWER_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.StancePower, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "PassivePowerUpPower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PASSIVE_POWER_UP_POWER_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.PassivePowerUpPower, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "ItemPower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ITEM_POWER_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.ItemPower, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB
                }
            };
        }

        else if (effect_name == "ToolPower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TOOL_POWER_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.ToolPower, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "Gain2ExtraChargesOfHealthPotionAndIncreaseEffectivness")
        {
            float calculatedPB = CalculatePB(power_budget, PB.HEALTH_POTION_POWER_INCREASE_PER_PB, new List<float> { PB.SPECIAL__GIVE_2_EXTRA_HEALTH_POTION_CHARGES });
            return new List<Effect> {
                new Effect_Id("Gain2ExtraChargesOfHealthPotionAndIncreaseEffectivness" + special_id, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ActionOnStart = new Action<Effect_Id> ((effect) =>  {
                        SaveFile.Instance.MaxHealCharges += 2;
                        Player.Instance.HealthPotionPower.AddFlatModifier(effect, effect.FlatAmount);
                    }),
                    ActionOnEnd = new Action<Effect_Id> ((effect) =>  {
                        SaveFile.Instance.MaxHealCharges -= 2;
                        Player.Instance.HealthPotionPower.RemoveFlatModifier(effect, effect.FlatAmount);
                    })
                }
            };
        }
        else if (effect_name == "Gain1ExtraUseOfToolsAndIncreaseToolPower")
        {
            float calculatedPB = CalculatePB(power_budget, PB.TOOL_POWER_INCREASE_PER_PB, new List<float> { PB.SPECIAL__GIVE_1_EXTRA_TOOL_CHARGES });
            return new List<Effect> {
                new Effect_Id("Gain1ExtraUseOfToolsAndIncreaseToolPower" + special_id, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ActionOnStart = new Action<Effect_Id> ((effect) =>  {
                        foreach(Type tool in SaveFile.Instance.ToolMaxAmounts.Keys) {
                            SaveFile.Instance.ToolMaxAmounts[tool]++;
                        }
                        Player.Instance.ToolPower.AddFlatModifier(effect, effect.FlatAmount);
                    }),
                    ActionOnEnd = new Action<Effect_Id> ((effect) =>  {
                        foreach(Type tool in SaveFile.Instance.ToolMaxAmounts.Keys) {
                            SaveFile.Instance.ToolMaxAmounts[tool]--;
                        }
                        Player.Instance.ToolPower.RemoveFlatModifier(effect, effect.FlatAmount);
                    })
                }
            };
        }


        //Alacrity
        else if (effect_name == "DodgingDamageGivesSupercharge")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SUPERCHARGE_PER_PB, PB.HAPPENS_UPON__DODGING });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForDamageWasDodged = new Func<DamageInstance, Ability, bool>((damage, ability) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    ActionOnDamageWasDodged = new Action<DamageInstance, Ability, Effect_CustomizableEffectOnEvent> ((damage, ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Supercharge(effect.FlatAmount, effect.SourceOfEffect));
                    }),
                },
            };
        }
        else if (effect_name == "DodgingDamageEmpowersNextAttack")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ONCE, PB.HAPPENS_UPON__DODGING });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForDamageWasDodged = new Func<DamageInstance, Ability, bool>((damage, ability) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    ActionOnDamageWasDodged = new Action<DamageInstance, Ability, Effect_CustomizableEffectOnEvent> ((damage, ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_EmpowerNextAttack(effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount
                        });
                    }),
                },
            };
        }
        else if (effect_name == "ConvertAttackSpeedToDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { PB.DAMAGE_INCREASE_PER_PB, 1 / PB.ATTACK_SPEED_INCREASE_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(Player.Instance.CurrentWeaponAttackSpeed.Current * calculatedPB / 100), Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.DamageDealtPercentageModifier += Player.Instance.CurrentWeaponAttackSpeed.Current * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "ConvertMovementSpeedToDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> { PB.DAMAGE_INCREASE_PER_PB, 1 / PB.MOVEMENT_SPEED_INCREASE_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(Player.Instance.MovementSpeed.Current * calculatedPB / 100), Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.DamageDealtPercentageModifier += Player.Instance.MovementSpeed.Current * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "GainSuperchargeBasedOnDistanceTravelled")
        {
            float calculatedPB = CalculatePB(power_budget, PB.STACKING_EFFECT_GAINED_PER_1M_TRAVELLED_PER_PB, new List<float> { PB.SUPERCHARGE_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Player.Instance.PlayerSavedPosition != Player.Instance.transform.position
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        float metersTravelled = Vector2.Distance(Player.Instance.PlayerSavedPosition, Player.Instance.transform.position);
                        Player.Instance.AddEffect(new Effect_Supercharge(metersTravelled * effect.PercentageAmount / 100, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "IncreaseInvincibilityTimeOfDodge")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DODGE_INVINCIBILITY_DURATION_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_Id("IncreaseInvincibilityTimeOfDodge", new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "DodgingDamageAppliesProne")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.PRONE_PER_PB, PB.HAPPENS_UPON__DODGING });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForDamageWasDodged = new Func<DamageInstance, Ability, bool>((damage, ability) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    ActionOnDamageWasDodged = new Action<DamageInstance, Ability, Effect_CustomizableEffectOnEvent> ((damage, ability, effect) =>  {
                        ability.User.AddEffect(new Effect_Prone(effect.FlatAmount, effect.SourceOfEffect));
                    }),
                },
            };
        }
        else if (effect_name == "GainAccelerationOnLightDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ACCELERATION_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Light
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Acceleration(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "ApplyLethargyOnLightDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.LETHARGY_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Light
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Lethargy(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "GainAccelerationBasedOnDistanceTravelled")
        {
            float calculatedPB = CalculatePB(power_budget, PB.STACKING_EFFECT_GAINED_PER_1M_TRAVELLED_PER_PB, new List<float> { PB.ACCELERATION_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneTenthSecondElapsedInGame = new Func<bool>(() =>
                        Player.Instance.PlayerSavedPosition != Player.Instance.transform.position
                    ),
                    ActionOnOneTenthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        if(Player.Instance.PlayerSavedPosition == null) {
                            Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                        }
                        float metersTravelled = Vector2.Distance(Player.Instance.PlayerSavedPosition, Player.Instance.transform.position);
                        Player.Instance.AddEffect(new Effect_Acceleration(metersTravelled * effect.PercentageAmount / 100, effect.SourceOfEffect));
                        Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                    })
                }
            };
        }


        //Enforcer
        else if (effect_name == "BasicAttacksReduceCooldowns")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.ReduceAllRemainingCooldowns(effect.PercentageAmount / 100);
                    })
                }
            };
        }
        else if (effect_name == "WhileAtFullEnergyBasicAttacksDealMoreDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.REQUIRES__BEING_AT_FULL_ENERGY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && Player.Instance.Energy.Current >= 100
                    ),
                }
            };
        }
        else if (effect_name == "BasicAttacksGiveOnslaught")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ONSLAUGHT_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Onslaught(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "SharpAndAnalysisAlsoIncreaseBasicAttackDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * ((PB.SHARP_PER_PB + PB.ANALYSIS_PER_PB) / 2), PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.SPECIAL__BONUS_FOR_AFFECTING_2_DIFFERENT_STACKING_EFFECTS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect sharp = Player.Instance.GetEffect(typeof(Effect_Sharp));
                        if(sharp != null) {
                            damage.DamageDealtPercentageModifier += sharp.DecayingAmount * effect.PercentageAmount;
                        }
                        Effect analysis = Player.Instance.GetEffect(typeof(Effect_Analysis));
                        if(analysis != null) {
                            damage.DamageDealtPercentageModifier += analysis.DecayingAmount * effect.PercentageAmount;
                        }
                    })
                }
            };
        }
        else if (effect_name == "NonStopBasicAttackingIncreasesDamageDealtAndSpeed")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.75f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.SPECIAL__STACKS_WHEN_NON_STOP_BASIC_ATTACKING });
            float calculatedPB2 = CalculatePB(power_budget * 0.25f, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.SPECIAL__STACKS_WHEN_NON_STOP_BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_NonStopBasicAttackingIncreasesDamageDealtAndSpeed(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    DamageBonusPerHit = calculatedPB1,
                    AttackSpeedBonusPerHit = calculatedPB2
                }
            };
        }
        else if (effect_name == "NonStrongBasicAttacksEmpowerYourNextStrongBasicAttack")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ONCE, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && damage.SourceOfDamage.IsNot(Ability.Property.StrongBasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect buff = Player.Instance.GetEffectWithGivenId("NonStrongBasicAttacksEmpowerYourNextStrongBasicAttack" + special_id);
                        if(buff != null) {
                            buff.PercentageAmount += effect.PercentageAmount;
                        }
                        else {
                            Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(effect_name)) {
                                Id = "NonStrongBasicAttacksEmpowerYourNextStrongBasicAttack" + special_id,
                                ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                                ),
                                Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                                    damage.DamageDealtPercentageModifier += effect.PercentageAmount;
                                    effect.EndThisEffect();
                                })
                            });
                        }
                    })
                }
            };
        }
        else if (effect_name == "StrongBasicAttackInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STRONG_BASIC_ATTACKS, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                }
            };
        }
        else if (effect_name == "NonStrongBasicAttacksEmpowerYourStrongBasicAttacks")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && damage.SourceOfDamage.IsNot(Ability.Property.StrongBasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect buff = Player.Instance.GetEffectWithGivenId("NonStrongBasicAttacksEmpowerYourStrongBasicAttacks" + special_id);
                        if(buff != null) {
                            buff.PercentageAmount += effect.PercentageAmount;
                        }
                        else {
                            Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(effect_name)) {
                                DamagePercentageModifier = effect.PercentageAmount,
                                Id = "NonStrongBasicAttacksEmpowerYourStrongBasicAttacks" + special_id,
                                ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                                ),
                            });
                        }
                    })
                }
            };
        }
        else if (effect_name == "CanBasicAttackNonStopAndDealMoreDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.SPECIAL__STACKS_WHEN_NON_STOP_BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage is BA_Bow_F
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        int barrageCount = ((BA_Bow_F)damage.SourceOfDamage).BarrageComboNumber;
                        damage.DamageDealtPercentageModifier += barrageCount * effect.PercentageAmount;
                    })
                }
            };
        }


        //Sage
        else if (effect_name == "UsingTechniquesDecreasesRemainingCooldownOfAllOtherTechniques")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__USING_TECHNIQUE, PB.AFFECTS_ONLY__COOLDOWN_REDUCTION_FOR_TECHNIQUES });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.Is(Ability.Property.Technique)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        foreach(Cooldown cd in Player.Instance.TechniqueCooldowns) {
                            if(cd.Type != ability.GetType()) {
                                cd.RemainingDuration = cd.RemainingDuration * effect.PercentageAmount / 100;
                            }
                        }
                    })
                }
            };
        }
        else if (effect_name == "IncreaseMagicDamageButDecreaseWeaponDamage")
        {
            float calculatedPB1 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__MAGIC_DAMAGE });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB1,
                    DescriptionParameters=new List<string>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}
                },
                new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = -calculatedPB2,
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = -calculatedPB2
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageAmount = -calculatedPB2
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageAmount = -calculatedPB2
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageAmount = -calculatedPB2
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageAmount = -calculatedPB2
                },
            };
        }
        else if (effect_name == "IncreaseEnergyGainWhileEnergyIsBelowHalf")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.REQUIRES__BEING_BELOW_HALF_ENERGY });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) =>
                        stat.Owner == Player.Instance && stat is Energy
                    ),
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat energyGainBuff = (Effect_ChangeStat)Player.Instance.GetEffectWithGivenId("IncreaseEnergyGainWhileEnergyIsBelowHalf" + special_id);
                        if(energyGainBuff == null && Player.Instance.Energy.Current <= Player.Instance.Energy.Maximum * 0.5f) {
                            Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Energy, effect.SourceOfEffect) {
                                PercentageAmount = effect.PercentageAmount,
                                Id = "IncreaseEnergyGainWhileEnergyIsBelowHalf" + special_id
                            });
                        }
                        else if(energyGainBuff != null && Player.Instance.Energy.Current > Player.Instance.Energy.Maximum * 0.5f) {
                            energyGainBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "MaxEnergy")
        {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_ENERGY_INCREASE_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Energy, new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "DealingMagicDamageGivesBarrier")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__DEALING_MAGIC_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "5"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Magic && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("DealingMagicDamageGivesBarrier" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FlatAmount, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(typeof(Effect), 5, "DealingMagicDamageGivesBarrier" + special_id);
                    })
                }
            };
        }


        //RoyalGuard
        else if (effect_name == "BasicAttacksGiveAnalysis")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ANALYSIS_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Analysis(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "MagicTechniquesEmpowerWeaponTechniquesAndViceVersa")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__MAGIC_DAMAGE, PB.HAPPENS_UPON__USING_WEAPON_TECHNIQUE, PB.AFFECTS_ONLY__ONCE });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__WEAPON_DAMAGE, PB.HAPPENS_UPON__USING_MAGIC_TECHNIQUE, PB.AFFECTS_ONLY__ONCE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB1,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.IsWeaponDamage
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_EmpowerNextAttack(effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount,
                            ConditionCheck = new Func<DamageInstance, Effect_EmpowerNextAttack, bool>((damage, effect) =>
                                damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.DamageType == Constants.DamageType.Magic
                            ),
                        });
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB1,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.DamageType == Constants.DamageType.Magic
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_EmpowerNextAttack(effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount,
                            ConditionCheck = new Func<DamageInstance, Effect_EmpowerNextAttack, bool>((damage, effect) =>
                                damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && damage.IsWeaponDamage
                            ),
                        });
                    })
                }
            };
        }
        else if (effect_name == "AnalysisAlsoIncreasesWeaponDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ANALYSIS_PER_PB, PB.AFFECTS_ONLY__WEAPON_DAMAGE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.IsWeaponDamage
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect analysis = Player.Instance.GetEffect(typeof(Effect_Analysis));
                        if(analysis != null) {
                            damage.DamageDealtPercentageModifier += analysis.DecayingAmount * effect.PercentageAmount;
                        }
                    })
                }
            };
        }
        else if (effect_name == "IncreaseWeaponDamageByPortionOfMagicDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.IsWeaponDamage
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.InjuryDealtPercentageModifier += Player.Instance.MagicInjury.Current * effect.PercentageAmount / 100;
                        damage.StaggerDealtPercentageModifier += Player.Instance.MagicStagger.Current * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "BasicAttacksDealIncreasedDamageBasedOnMagicDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, PB.AFFECTS_ONLY__BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.InjuryDealtPercentageModifier += Player.Instance.MagicInjury.Current * effect.PercentageAmount / 100;
                        damage.StaggerDealtPercentageModifier += Player.Instance.MagicStagger.Current * effect.PercentageAmount / 100;
                    })
                }
            };
        }


        //ShadowGifted
        else if (effect_name == "DealMoreDamageBasedOnPositiveStackingEffectsOnYouAndNegativeOnEnemy")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_UNIQUE_STACKING_EFFECTS_ON_PLAYER_AND_ENEMY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        int count = Player.Instance.CurrentEffects.Where(e => e.Type == Effect.EffectType.Buff && e.BehaviourWhenDuplicateEffect == Effect.BehaviourWhenDuplicateEffectEnum.StackDecayingAmount).Count() + damage.TargetOfDamage.CurrentEffects.Where(e => e.Type == Effect.EffectType.Debuff && e.BehaviourWhenDuplicateEffect == Effect.BehaviourWhenDuplicateEffectEnum.StackDecayingAmount).Count();
                        damage.DamageDealtPercentageModifier += count * effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "WhenAttackingAnEnemyWithAllSignatureEffectsAppliedDealMassiveDamageBasedOnAmountAndRemoveAllOfThem")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DEAL_FLAT_DAMAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.SPECIAL__REMOVES_BURN_FREEZE_AND_BLEED_FROM_ENEMY, PB.REQUIRES__BURN, PB.REQUIRES__FREEZE, PB.REQUIRES__BLEED, PB.REQUIRES__30_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "30"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                         damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Burn)) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Freeze)) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Bleed)) && !damage.TargetOfDamage.CheckIfEffectWithGivenIdIsOnCooldown("WhenAttackingAnEnemyWithAllSignatureEffectsAppliedDealMassiveDamageBasedOnAmountAndRemoveAllOfThem" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.DamageDealtFlatModifier += effect.FlatAmount;
                        damage.TargetOfDamage.GetEffect(typeof(Effect_Burn)).EndThisEffect();
                        damage.TargetOfDamage.GetEffect(typeof(Effect_Freeze)).EndThisEffect();
                        damage.TargetOfDamage.GetEffect(typeof(Effect_Bleed)).EndThisEffect();
                        damage.TargetOfDamage.AddCooldown(typeof(Effect), 30, "WhenAttackingAnEnemyWithAllSignatureEffectsAppliedDealMassiveDamageBasedOnAmountAndRemoveAllOfThem" + special_id);
                    })
                }
            };
        }

        else if (effect_name == "GainAnalysisOnUsingTechniques")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ANALYSIS_PER_PB, PB.HAPPENS_UPON__USING_TECHNIQUE });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.User == Player.Instance && ability.Is(Ability.Property.Technique)
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Analysis(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "AllStackingEffectsAmount")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> { PB.SPECIAL__AFFECTS_ALL_STACKING_EFFECTS });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Both, calculatedPB, new(effect_name)) {
                    Id = "AllStackingEffectsAmount",
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "AllStackingEffectsDecay")
        {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB, new List<float> { PB.SPECIAL__AFFECTS_ALL_STACKING_EFFECTS });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Both, calculatedPB, new(effect_name)) {
                    Id = "AllStackingEffectsDecay",
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if (effect_name == "GainArmorBasedOnNegativeStackingEffectsOnEnemy")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_UNIQUE_STACKING_EFFECTS_ON_ENEMY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        int count = damage.TargetOfDamage.CurrentEffects.Where(e => e.Type == Effect.EffectType.Debuff && e.BehaviourWhenDuplicateEffect == Effect.BehaviourWhenDuplicateEffectEnum.StackDecayingAmount).Count();
                        damage.ArmorModifier += count * effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "GainArmorBasedOnPositiveStackingEffectsOnYou")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { 1 / PB.EXPECTED_AMOUNT_OF_UNIQUE_STACKING_EFFECTS_ON_PLAYER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        int count = Player.Instance.CurrentEffects.Where(e => e.Type == Effect.EffectType.Buff && e.BehaviourWhenDuplicateEffect == Effect.BehaviourWhenDuplicateEffectEnum.StackDecayingAmount).Count();
                        damage.ArmorModifier += count * effect.PercentageAmount;
                    })
                }
            };
        }
        else if (effect_name == "OnHitApplyRandomStackingEffectYouDoNotCurrentlyPossess")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB2 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BURN_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB3 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.FREEZE_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB4 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB5 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BLEED_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB6 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SUPERCHARGE_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB7 = CalculatePB(power_budget * 0.143f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ANALYSIS_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2, calculatedPB3, calculatedPB4, calculatedPB5, calculatedPB6, calculatedPB7},
                    DescriptionParameters = new List<String> { Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3), Utils.GetFormattedFloat(calculatedPB4), Utils.GetFormattedFloat(calculatedPB5), Utils.GetFormattedFloat(calculatedPB6), Utils.GetFormattedFloat(calculatedPB7), "5"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("OnHitApplyRandomStackingEffectYouDoNotCurrentlyPossess" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Dictionary<Type, float> validEffects = new Dictionary<Type, float> {
                            {typeof(Effect_Sharp), effect.CustomParameters[0]},
                            {typeof(Effect_Burn), effect.CustomParameters[1]},
                            {typeof(Effect_Freeze), effect.CustomParameters[2]},
                            {typeof(Effect_Barrier), effect.CustomParameters[3]},
                            {typeof(Effect_Bleed), effect.CustomParameters[4]},
                            {typeof(Effect_Supercharge), effect.CustomParameters[5]},
                            {typeof(Effect_Analysis), effect.CustomParameters[6]},
                        };
                        foreach(Type e in new List<Type> {typeof(Effect_Burn), typeof(Effect_Freeze), typeof(Effect_Bleed)}) {
                            if(damage.TargetOfDamage.CheckIfUnderEffect(e)) {
                                validEffects.Remove(e);
                            }
                        }
                        foreach(Type e in new List<Type> {typeof(Effect_Sharp), typeof(Effect_Barrier), typeof(Effect_Supercharge), typeof(Effect_Analysis)}) {
                            if(Player.Instance.CheckIfUnderEffect(e)) {
                                validEffects.Remove(e);
                            }
                        }
                        if(validEffects.Count > 0) {
                            int index = UnityEngine.Random.Range(0, validEffects.Count);
                            Type randomizedEffect = validEffects.Keys.ToList()[index];
                            if(randomizedEffect == typeof(Effect_Burn) || randomizedEffect == typeof(Effect_Freeze) || randomizedEffect == typeof(Effect_Bleed)) {
                                Effect e = (Effect)Activator.CreateInstance(randomizedEffect, new object[] { validEffects[randomizedEffect], effect.SourceOfEffect });
                                damage.TargetOfDamage.AddEffect(e);
                            }
                            else {
                                Effect e = (Effect)Activator.CreateInstance(randomizedEffect, new object[] { validEffects[randomizedEffect], effect.SourceOfEffect });
                                Player.Instance.AddEffect(e);
                            }
                            Player.Instance.AddCooldown(typeof(Effect), 5, "OnHitApplyRandomStackingEffectYouDoNotCurrentlyPossess" + special_id);
                        }
                    })
                }
            };
        }
        else if (effect_name == "LightDamageConvertsAllFreezeIntoBurnOrBurnIntoFreeze")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE, (PB.BURN_PER_PB + PB.FREEZE_PER_PB) / 2, PB.REQUIRES__10_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Light && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("LightDamageConvertsAllFreezeIntoBurnOrBurnIntoFreeze" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect burn = damage.TargetOfDamage.GetEffect(typeof(Effect_Burn));
                        Effect freeze = damage.TargetOfDamage.GetEffect(typeof(Effect_Freeze));
                        float burnAmount = burn == null ? 0 : burn.DecayingAmount;
                        float freezeAmount = freeze == null ? 0 : freeze.DecayingAmount;
                        if(burnAmount > 0 && burnAmount > freezeAmount) {
                            damage.TargetOfDamage.AddEffect(new Effect_Freeze(burnAmount, effect.SourceOfEffect));
                            burn.EndThisEffect();
                            Player.Instance.AddCooldown(typeof(Effect), 10, "LightDamageConvertsAllFreezeIntoBurnOrBurnIntoFreeze" + special_id);
                        }
                        else if(freezeAmount > 0 && freezeAmount > burnAmount){
                            damage.TargetOfDamage.AddEffect(new Effect_Burn(freezeAmount, effect.SourceOfEffect));
                            freeze.EndThisEffect();
                            Player.Instance.AddCooldown(typeof(Effect), 10, "LightDamageConvertsAllFreezeIntoBurnOrBurnIntoFreeze" + special_id);
                        }
                    })
                }
            };
        }
        else if (effect_name == "RangedDamageAppliesBurnOrFreezeToEqualize")
        {
            float calculatedPB1 = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE, PB.BURN_PER_PB, PB.REQUIRES__5_SECOND_COOLDOWN });
            float calculatedPB2 = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE, PB.FREEZE_PER_PB, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    CustomParameters = new List<float> {calculatedPB1, calculatedPB2 },
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("RangedDamageAppliesBurnOrFreezeToEqualize" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect burn = damage.TargetOfDamage.GetEffect(typeof(Effect_Burn));
                        Effect freeze = damage.TargetOfDamage.GetEffect(typeof(Effect_Freeze));
                        float burnAmount = burn == null ? 0 : burn.DecayingAmount;
                        float freezeAmount = freeze == null ? 0 : freeze.DecayingAmount;
                        if(burnAmount > freezeAmount) {
                            damage.TargetOfDamage.AddEffect(new Effect_Freeze(effect.CustomParameters[1], effect.SourceOfEffect));
                        }
                        else {
                            damage.TargetOfDamage.AddEffect(new Effect_Burn(effect.CustomParameters[0], effect.SourceOfEffect));
                        }
                        Player.Instance.AddCooldown(typeof(Effect), 5, "RangedDamageAppliesBurnOrFreezeToEqualize" + special_id);
                    })
                }
            };
        }


        //Unique
        else if (effect_name == "StrongBasicAttacksReducePercentageOfEnemyStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_1P_OF_ENEMY_STAGGER_BAR_PER_PB, new List<float> { PB.HAPPENS_UPON__STRONG_BASIC_ATTACKING, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtFlatModifier += damage.TargetOfDamage.StaggerBar.Maximum * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "ReplaceAllBasicAttacksWithChargeAndImproveDamage")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STRONG_BASIC_ATTACKS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                }
            };
        }
        else if (effect_name == "ReplaceAllBasicAttacksWithThrowAddPullAndIncreaseStagger")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STRONG_BASIC_ATTACKS, PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                }
            };
        }
        else if (effect_name == "ReplaceAllBasicAttacksWithThrowAndIncreaseInjury")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STRONG_BASIC_ATTACKS, PB.AFFECTS_ONLY__INJURY });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    InjuryPercentageModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                }
            };
        }
        else if (effect_name == "StrongBasicAttacksApplyBleedBasedOnEnemyHealth")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.HAPPENS_UPON__STRONG_BASIC_ATTACKING, PB.BLEED_PER_PB, PB.SPECIAL__SCALES_WITH_CURRENT_HEALTH_INSTEAD_OF_MAXIMUM });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.StrongBasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Bleed(effect.FlatAmount * (damage.TargetOfDamage.Health.Current / damage.TargetOfDamage.Health.Maximum), effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "DealMoreLightInjuryOrStaggerAndCanSwitchUsingBlock")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { (PB.AFFECTS_ONLY__INJURY + PB.AFFECTS_ONLY__STAGGER) / 2 });
            return new List<Effect> {
                new Effect_DealMoreLightInjuryOrStaggerAndCanSwitchUsingBlock(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "BasicAttacksApplyBleed")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BLEED_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Bleed(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "BleedArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__BLEED });
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Bleed), Player.Instance.Armor, calculatedPB, new(effect_name))
            };
        }
        else if (effect_name == "BasicAttacksApplyPoison")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.POISON_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Poison(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if (effect_name == "LightDamageTemporarilyLowersEnemyArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.ARMOR_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE, PB.AFFECTS_ONLY__20_SECONDS });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = -calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "20"},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.DamageType == Constants.DamageType.Light
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_ChangeStat(damage.TargetOfDamage.Armor, effect.SourceOfEffect) {
                            FlatAmount = effect.FlatAmount
                        }, 20);
                    })
                }
            };
        }
        else if (effect_name == "LightDamageIgnoresPercentageOfEnemyArmor")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_ARMOR_PENETRATION_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ArmorPenetrationModifier = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.DamageType == Constants.DamageType.Light
                    ),
                }
            };
        }
        else if (effect_name == "BasicAttacksReducePercentageOfEnemyStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_1P_OF_ENEMY_STAGGER_BAR_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING, PB.REQUIRES__5_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.StaggerDealtFlatModifier += damage.TargetOfDamage.StaggerBar.Maximum * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "ConvertAllInjuryIntoStaggerOrViceVersaDependingOnWhichIsLowerForEnemy")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.SPECIAL__CONVERT_ALL_DAMAGE_DEPENDING_ON_WHICH_IS_LOWER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(damage.TargetOfDamage.Health.Current / damage.TargetOfDamage.Health.Maximum < 1 - (damage.TargetOfDamage.StaggerBar.Current / damage.TargetOfDamage.StaggerBar.Maximum)) {
                            damage.StaggerDealtPercentageModifier += effect.PercentageAmount;
                        }
                        else {
                            damage.InjuryDealtPercentageModifier += effect.PercentageAmount;
                        }
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(damage.TargetOfDamage.Health.Current / damage.TargetOfDamage.Health.Maximum < 1 - (damage.TargetOfDamage.StaggerBar.Current / damage.TargetOfDamage.StaggerBar.Maximum)) {
                            damage.Injury += damage.Stagger;
                            damage.Stagger = 0;
                        }
                        else {
                            damage.Stagger += damage.Injury;
                            damage.Injury = 0;
                        }
                    })
                }
            };
        }
        else if (effect_name == "RangedInjuryAndStaggerScaleWithEachOther")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_TYPE });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.InjuryDealtPercentageModifier = Player.Instance.RangedStagger.Current * effect.PercentageAmount / 100;
                        damage.StaggerDealtPercentageModifier = Player.Instance.RangedInjury.Current * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if (effect_name == "EbonySpecialScaling")
        {
            return new List<Effect> {
                new Effect_Id("EbonySpecialScaling", new(effect_name))
            };
        }
        else if (effect_name == "OnSwitchingToThisWeaponGainRangedAttackSpeedAndNextBasicAttackStunsEnemy")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.25f, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__5_SECONDS, PB.REQUIRES__20_SECOND_COOLDOWN });
            float calculatedPB2 = CalculatePB(power_budget * 0.75f, PB.SECONDS_OF_STUN_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE, PB.AFFECTS_ONLY__5_SECONDS, PB.REQUIRES__20_SECOND_COOLDOWN });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB1,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), "5", Utils.GetFormattedFloat(calculatedPB2), "20"},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, effect.SourceOfEffect) {
                            PercentageAmount = effect.PercentageAmount,
                            Id = "OnSwitchingToThisWeaponGainRangedAttackSpeedAndNextBasicAttackStunsEnemy" + special_id
                        }, 5);
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB2,
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && damage.DamageType == Constants.DamageType.Ranged && Player.Instance.CheckIfUnderEffectWithGivenId("OnSwitchingToThisWeaponGainRangedAttackSpeedAndNextBasicAttackStunsEnemy" + special_id)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Stun(effect.SourceOfEffect), effect.FlatAmount);
                        Player.Instance.GetEffectWithGivenId("OnSwitchingToThisWeaponGainRangedAttackSpeedAndNextBasicAttackStunsEnemy" + special_id).EndThisEffect();
                    })
                }
            };
        }
        else if (effect_name == "TripleArrowsShotWithoutSpendingExtraAmmo")
        {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_ORIGINAL_DAMAGE_DEALT_BY_TRIPLE_ARROWS_PER_PB, new List<float> { });
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForProjectileCreated = new Func<Projectile, bool>((projectile) =>
                        projectile.gameObject.name.Contains("BowBasicAttack") && Player.Instance.Actions.CurrentAbilityBeingPerformed is BA_Bow_F
                    ),
                    ActionOnProjectileCreated = new Action<Projectile, Effect_CustomizableEffectOnEvent> ((projectile, effect) =>  {
                        for(int i = 0; i < 2; i++) {
                            Projectile proj = Utils.CreateProjectile(new(projectile.SourceAbility), "BowBasicAttack");
                            proj.FlightSpeed = 10;
                            proj.transform.eulerAngles = new Vector3(projectile.transform.eulerAngles.x, projectile.transform.eulerAngles.y, projectile.transform.eulerAngles.z + (i == 0 ? -10 : 10));
                        }
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    PercentageAmount = calculatedPB,
                    ConditionCheckAfterHitDamageCalculation = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage is BA_Bow_F
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Injury = effect.PercentageAmount / 100;
                        damage.Stagger = effect.PercentageAmount / 100;
                    })
                },
            };
        }
        else if (effect_name == "BasicAttacksDealIncreasedDamageButAlsoInflictSelfStagger")
        {
            float calculatedPB1 = CalculatePB(power_budget * 1.5f, PB.DEAL_FLAT_DAMAGE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS });
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DEAL_FLAT_DAMAGE_PER_PB, new List<float> { PB.AFFECTS_ONLY__BASIC_ATTACKS, PB.AFFECTS_ONLY__STAGGER });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB1,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack)
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.DamageDealtFlatModifier += effect.FlatAmount;
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount = calculatedPB2,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) =>
                        ability.GetType().IsSubclassOf(typeof(BasicAttack))
                    ),
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        new DamageInstance(Player.Instance, ability, null) {
                            StaggerDealtFlatModifier = effect.FlatAmount
                        }.CalculateAndApplyDamage();
                    })
                },
            };
        }









        else if (effect_name == "IgnisManor_WeaponTraining")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "IgnisManor_BackstabPowerUp")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FlatAmount = calculatedPB,
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.Is(Ability.Property.Backstab) && damage.SourceOfDamage?.User == Player.Instance
                    ),
                    Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Burn(Player.Instance.HeavyStagger.Current * 0.05f, new(damage.SourceOfDamage)));
                    })
                }
            };
        }
        else if (effect_name == "IgnisManor_BurningPowerUp")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 10, new(effect_name)),
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))
                    )
                }
            };
        }
        else if (effect_name == "IgnisVolcano_BuriedEnergy")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 10, new(effect_name)),
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -10, new(effect_name))
            };
        }
        else if (effect_name == "IgnisVolcano_FireRiver")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.Properties.Contains(DamageInstance.DamageProperty.Burn)
                    )
                }
            };
        }
        else if (effect_name == "IgnisManorOnFire_BurningSword")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if (effect_name == "IgnisManorOnFire_MuseumSword")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 15, new(effect_name))
            };
        }
        else if (effect_name == "IgnisManorOnFire_MuseumArmour")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    FlatAmount = -calculatedPB
                }
            };
        }
        else if (effect_name == "AnimaIsland_3MajorDuels")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageAmount = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageAmount = calculatedPB
                }
            };
        }
        else
        {
            Debug.LogError("Power Up behavior not defined for: " + effect_name);
            return null;
        }
    }

    public static float CalculatePB(float base_pb, float scales_with, List<float> multipliers = null) {
        float amount = scales_with * base_pb;
        if(multipliers != null && multipliers.Count > 0) {
            foreach(float multi in multipliers) {
                amount *= multi;
            }
        }
        return amount;
    }

    public static float CalculatePB(float base_pb, string scales_with, List<string> multipliers) {
        FieldInfo scaling = typeof(Constants).GetField(scales_with, BindingFlags.Public | BindingFlags.Static);
        if(String.IsNullOrWhiteSpace(scales_with) || scales_with == null) {
            Debug.LogError($"Incorrect scaling ({scales_with}) used");
            return 0;
        }
        float amount = (float)scaling.GetValue(null) * base_pb;
        if(multipliers != null && multipliers.Count > 0) {
            foreach(string multiName in multipliers) {
                FieldInfo multi = typeof(Constants).GetField(multiName, BindingFlags.Public | BindingFlags.Static);
                if(multi == null) {
                    Debug.LogError($"Incorrect multiplier name {multiName} used for effect scaling {scales_with}");
                    return 0;
                }
                amount *= (float)multi.GetValue(null);
            }
        }
        return amount;
    }
}
