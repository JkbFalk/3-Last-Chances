using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EffectList
{
    public static List<Effect> GetEffect(string effect_name, float power_budget, string special_identifier = "") {
        if(string.IsNullOrWhiteSpace(effect_name)) {
            Debug.LogError("Requested effect with no name");
            return null;
        }
        else if(power_budget == 0) {
            Debug.LogError($"Requested effect ({effect_name}) with 0 power budget: {power_budget}");
            return null;
        }
        List<Effect> Effects = new List<Effect>();
        if (effect_name == "Health") {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_HEALTH_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Health }
                }
            };
        }
        else if (effect_name == "StaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.StaggerBar, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.StaggerBar }
                }
            };
        }
        else if (effect_name == "Damage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger }
                }
            };
        }
        else if (effect_name == "Injury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY});
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury }
                }
            };
        }
        else if (effect_name == "Stagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER});
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger }
                }
            };
        }
        else if (effect_name == "AttackSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyAttackSpeed,Player.Instance.LightAttackSpeed,Player.Instance.RangedAttackSpeed,Player.Instance.MagicAttackSpeed }
                }
            };
        }
        else if (effect_name == "EnergyGain") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.EnergyGain, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.EnergyGain }
                }
            };
        }
        else if (effect_name == "DamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.DamageReduction, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction }
                }
            };
        }
        else if (effect_name == "Tenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Tenacity, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Tenacity }
                }
            };
        }
        else if (effect_name == "CooldownReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.CooldownReduction, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.CooldownReduction }
                }
            };
        }
        else if (effect_name == "Control") {
            float calculatedPB = CalculatePB(power_budget, PB.CONTROL_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Control, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Control }
                }
            };
        }
        else if (effect_name == "MovementSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MovementSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MovementSpeed }
                }
            };
        }
        else if (effect_name == "HeavyDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury, Player.Instance.HeavyStagger }
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB
                }
            };
        }
        else if (effect_name == "LightDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightInjury, Player.Instance.LightStagger }
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB
                }
            };
        }
        else if (effect_name == "RangedDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedInjury, Player.Instance.RangedStagger }
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB
                }
            };
        }
        else if (effect_name == "MagicDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__MAGIC});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicInjury, Player.Instance.MagicStagger }
                },
                new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB
                }
            };
        }
        else if (effect_name == "HeavyInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury}
                }
            };
        }
        else if (effect_name == "LightInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightInjury}
                }
            };
        }
        else if (effect_name == "RangedInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedInjury}
                }
            };
        }
        else if (effect_name == "MagicInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.SPECIALIZATION__MAGIC});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicInjury}
                }
            };
        }
        else if (effect_name == "HeavyStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger}
                }
            };
        }
        else if (effect_name == "LightStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightStagger}
                }
            };
        }
        else if (effect_name == "RangedStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedStagger}
                }
            };
        }
        else if (effect_name == "MagicStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__MAGIC});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicStagger}
                }
            };
        }
        else if (effect_name == "HeavyAttackSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyAttackSpeed}
                }
            };
        }
        else if (effect_name == "LightAttackSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.LightAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightAttackSpeed}
                }
            };
        }
        else if (effect_name == "RangedAttackSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedAttackSpeed}
                }
            };
        }
        else if (effect_name == "MagicAttackSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__MAGIC});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.MagicAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MagicAttackSpeed}
                }
            };
        }

















        // Anima
        else if(effect_name == "DamageReductionAgainstCounterable") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_COUNTERABLE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamageReductionChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool> ((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.IsCounterable)
                }
            };
        }
        else if (effect_name == "BasicAttackDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__BASIC_ATTACK});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                     ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury,Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool> ((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack))
                }
            };
        }
        else if (effect_name == "WeaponTechniqueDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPONS, PB.SPECIALIZATION__TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool> ((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.IsWeaponDamage)
                }
            };
        }
        else if (effect_name == "WeaponDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPONS});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger }
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB
                }
            };
        }
        else if (effect_name == "WeaponInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPONS, PB.SPECIALIZATION__INJURY});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury }
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                }
            };
        }   
        else if (effect_name == "WeaponStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPONS, PB.SPECIALIZATION__STAGGER});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger }
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                }
            };
        }          
        else if(effect_name == "SharpAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "SharpDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "SharpDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SHARP});
            return new List<Effect> { 
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Sharp), Player.Instance.DamageReduction, calculatedPB, new(effect_name)) {
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction },
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "DamageReductionAfterRiposteOrCounter") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__5_SECONDS_AFTER_RIPOSTE_OR_10_SECONDS_AFTER_COUNTER});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Effect extraDR = new Effect_ChangeStat(Player.Instance.DamageReduction, new(ability)) {
                            PercentageModifier = effect.PercentageAmount, 
                            ShowsInUI = true, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                            Identifier="DamageReductionAfterRiposteOrCounter" + special_identifier, 
                            EffectIndicatorText = effect.PercentageAmount + "%"
                        };
                        Player.Instance.AddEffect(extraDR, ability.Is(Ability.AbilityProperty.Counter) ? 10 : 5);
                    })
                }
            };
        }
        else if(effect_name == "DamageReductionAgainstUnstoppable") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_UNSTOPPABLE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamageReductionChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool> ((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Unstoppable))
                }
            };
        }
        else if (effect_name == "RiposteDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__RIPOSTES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool> ((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte))
                }
            };
        }
        else if (effect_name == "SharpEmpowersBasicAttacks") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / (PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB), PB.SPECIALIZATION__BASIC_ATTACK});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 100)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool> ((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
                        damage.ExtraDamageDealtPercentage += sharp.DecayingAmount * effect.DamagePercentageChange;
                    })
                }
            };
        }
        else if(effect_name == "DamageReductionDuringBasicAttacks") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_BASIC_ATTACKING});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamageReductionChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentAbilityBeingPerformed != null &&  Player.Instance.Actions.CurrentAbilityBeingPerformed.Is(Ability.AbilityProperty.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "CounterDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__COUNTERS});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter)
                    )
                }
            };
        }
        else if (effect_name == "StrongBasicAttackDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STRONG_BASIC_ATTACK});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury,Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.StrongBasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "SharpEmpowersWeaponTechniques") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / (PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB), PB.SPECIALIZATION__TECHNIQUES, PB.SPECIALIZATION__WEAPONS});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 100)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.IsWeaponDamage && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
                        damage.ExtraDamageDealtPercentage += sharp.DecayingAmount * effect.DamagePercentageChange;
                    })
                }
            };
        }
        else if (effect_name == "RestoreStaggerBarOnSuccesfulRiposteOrCounterWhileStaggered") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.PERCENTAGE_STAGGER_BAR_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__RIPOSTE, PB.RESTRICTION__PLAYER_STAGGERED});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.PERCENTAGE_STAGGER_BAR_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__COUNTER, PB.RESTRICTION__PLAYER_STAGGERED});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    FirstParameter = calculatedPB1,
                    PercentageAmount = calculatedPB2,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * (ability.Is(Ability.AbilityProperty.Counter) ? effect.PercentageAmount : effect.FirstParameter) / 100;
                    })
                }
            };
        }
        else if(effect_name == "HealFromBasicAttacks") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__BASIC_ATTACK});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.FirstParameter + damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                }
            };
        }
        else if(effect_name == "HealFromRipostesAndCounters") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__RIPOSTES_COUNTERS});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte) || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.FirstParameter + damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                }
            };
        }























        // Ignis
        else if(effect_name == "DamageToStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_STAGGERED});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))
                    )
                }
            };
        }
        else if (effect_name == "BasicAttackStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__BASIC_ATTACK, PB.SPECIALIZATION__STAGGER});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    )
                }
            };
        }
        else if (effect_name == "TechniqueStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__TECHNIQUES, PB.SPECIALIZATION__STAGGER});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)
                    )
                }
            };
        }
        else if(effect_name == "BurnAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if(effect_name == "BurnDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if(effect_name == "BurnDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__BURN});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 2)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Burn burn = (Effect_Burn)damage.SourceOfDamage.User.GetEffect(typeof(Effect_Burn));
                        damage.ExtraDamageReduction += burn.StackingEffectIntensityLevel == 1 ? effect.FirstParameter / 2 : burn.StackingEffectIntensityLevel == 2 ? effect.FirstParameter : burn.StackingEffectIntensityLevel == 3 ? effect.FirstParameter * 2 : 0;
                    })
                }
            };
        }
        else if (effect_name == "HeavyStrongBasicAttackStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__STRONG_BASIC_ATTACK, PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> {Player.Instance.HeavyStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.StrongBasicAttack) && damage.DamageType == Constants.DamageType.Heavy
                    )
                }
            };
        }
        else if (effect_name == "ExtraMagicStaggerToEmptyStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__ENEMY_EMPTY_STAGGER_BAR, PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__MAGIC});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    StaggerPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> {Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.StaggerBar.Current < 1 && damage.DamageType == Constants.DamageType.Magic
                    )
                }
            };
        }
        else if(effect_name == "BurnAmountToNonBurning") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__ENEMY_NO_STACKING_EFFECT_APPLIED});
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionForEffectPowerChange = new Func<Unit, bool>(target => 
                        target.CheckIfUnderEffect(typeof(Effect_Burn)) == false
                    )
                }
            };
        }
        else if(effect_name == "DamageReductionAgainstBosses") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_BOSSES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamageReductionChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.IsBoss
                    )
                }
            };
        }
        else if (effect_name == "BurnEmpowersStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_DAMAGING_EFFECT_INTO_EXTRA_DAMAGE_PER_PB, new List<float> {1 / PB.BURN_PER_PB, PB.SPECIALIZATION__STAGGER});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Burn))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Burn burn = (Effect_Burn)damage.TargetOfDamage.GetEffect(typeof(Effect_Burn));
                        damage.ExtraStaggerDealtFlat += burn.DecayingAmount * effect.FirstParameter / 100;
                    })
                }
            };
        }
        else if (effect_name == "BurnExplosionDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__BURN_EXPLOSION_DAMAGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.Is(Damage.DamageProperty.BurnExplosion)
                    )
                }
            };
        }
        else if(effect_name == "DamageReductionAgainstNonBosses") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_NON_BOSSES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamageReductionChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && !damage.SourceOfDamage.User.IsBoss
                    )
                }
            };
        }
        else if(effect_name == "HealFromStackingEffects") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__PLAYER_STACKING_EFFECT_DAMAGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && (damage.Is(Damage.DamageProperty.Burn) || damage.Is(Damage.DamageProperty.BurnExplosion) || damage.Is(Damage.DamageProperty.Freeze) || damage.Is(Damage.DamageProperty.Incision))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.FirstParameter + damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                }
            };
        }
        else if(effect_name == "HealFromStaggerToNonStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER, PB.SPECIALIZATION__AGAINST_NON_STAGGERED});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                }
            };
        }
















        //Glacies
        else if(effect_name == "FreezeAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if(effect_name == "FreezeDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "RangedDamageLowersTenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE, PB.RESTRICTION__WORKS_10_SECONDS_NON_STACKABLE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = -calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect decreasedTenacity = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(damage.SourceOfDamage)) {
                            PathToEffectGraphic="UI/Control", 
                            PercentageModifier = effect.FirstParameter, 
                            ShowsInUI = true, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                            Identifier="RangedDamageLowersTenacity" + special_identifier, 
                            EffectIndicatorText=effect.FirstParameter + "%"
                        };
                        damage.TargetOfDamage.AddEffect(decreasedTenacity, 10);
                    })
                }
            };
        }
        else if (effect_name == "MagicDamageLowersTenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__MAGIC, PB.RESTRICTION__WORKS_10_SECONDS_NON_STACKABLE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = -calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Magic
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect decreasedTenacity = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(damage.SourceOfDamage)) {
                            PathToEffectGraphic="UI/Control", 
                            PercentageModifier = effect.FirstParameter, 
                            ShowsInUI = true, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                            Identifier="MagicDamageLowersTenacity" + special_identifier, 
                            EffectIndicatorText=effect.FirstParameter + "%"
                        };
                        damage.TargetOfDamage.AddEffect(decreasedTenacity, 10);
                    })
                }
            };
        }
        else if (effect_name == "StaggeringLowersTenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__STAGGERING_AN_ENEMY, PB.RESTRICTION__WORKS_30_SECONDS_NON_STACKABLE});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = -calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.SourceOfEffect.User == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect decreasedTenacity = new Effect_ChangeStat(effect_started.TargetOfEffect.Tenacity, effect_started.SourceOfEffect) {
                            PathToEffectGraphic="UI/Control", 
                            PercentageModifier = effect.PercentageAmount, 
                            ShowsInUI = true, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                            Identifier="StaggeringLowersTenacity" + special_identifier, 
                            EffectIndicatorText=effect.PercentageAmount + "%"
                        };
                        effect_started.TargetOfEffect.AddEffect(decreasedTenacity, 30);
                    })
                }
            };
        }
        else if (effect_name == "MovementSpeedWhileInRangedStance") {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ONE_STANCE});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.MovementSpeed},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect_ChangeStat msBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "MovementSpeedWhileInRangedStance" + special_identifier);
                        if(Player.Instance.CurrentStance.DamageType != Constants.DamageType.Ranged && msBuff != null) {
                            msBuff.EndThisEffect();
                        }
                        else if(Player.Instance.CurrentStance.DamageType == Constants.DamageType.Ranged && msBuff == null){
                            msBuff = new Effect_ChangeStat(Player.Instance.MovementSpeed, new("MovementSpeedWhileInRangedStance")) {
                                PercentageModifier = effect.PercentageAmount, 
                                BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                                Identifier="MovementSpeedWhileInRangedStance" + special_identifier, 
                                IsRemovable=false, ShowsInUI=true, 
                                EffectIndicatorText=effect.PercentageAmount + "%"
                            };
                            Player.Instance.AddEffect(msBuff); 
                        }
                    })
                }
            };
        }
        else if (effect_name == "StaggeringEnemyGivesAmmo") {
            float calculatedPB = CalculatePB(power_budget, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__STAGGERING_AN_ENEMY});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.SourceOfEffect.User == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.Ammo += effect.PercentageAmount; 
                    })
                }
            };
        }
        else if (effect_name == "ControlWhileNoAmmo") {
            float calculatedPB = CalculatePB(power_budget, PB.CONTROL_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__0_AMMO});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Control },
                    ConditionCheckForAmmoAmountChanged = new Func<bool>(() => true), 
                    ActionOnAmmoAmountChanged = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Effect_ChangeStat controlBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "ControlWhileNoAmmo" + special_identifier);
                        if(Player.Instance.Ammo > 0 && controlBuff != null) {
                            controlBuff.EndThisEffect();
                        }
                        else if(Player.Instance.Ammo == 0 && controlBuff == null) {
                            controlBuff = new Effect_ChangeStat(Player.Instance.Control, new("ControlWhileNoAmmo")) {
                                PercentageModifier = effect.PercentageAmount, 
                                Identifier="ControlWhileNoAmmo" + special_identifier, 
                                IsRemovable=false, 
                                ShowsInUI=true, 
                                EffectIndicatorText=effect.PercentageAmount + "%"
                            };
                            Player.Instance.AddEffect(controlBuff);  
                        } 
                    })
                }
            };
        }
        else if (effect_name == "ControlWhileLowHealth")  {
            float calculatedPB = CalculatePB(power_budget, PB.CONTROL_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_BELOW_25P_HEALTH});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Control },
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                        stat.Owner == Player.Instance && stat is Health
                    ), 
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat controlBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "ControlWhileLowHealth" + special_identifier);
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.25f && controlBuff != null) {
                            controlBuff.EndThisEffect();
                        }
                        else if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.25f && controlBuff == null) {
                            controlBuff = new Effect_ChangeStat(Player.Instance.Control, new("ControlWhileLowHealth")) {
                                PercentageModifier = effect.PercentageAmount, 
                                Identifier="ControlWhileLowHealth" + special_identifier, 
                                IsRemovable=false, ShowsInUI=true, 
                                EffectIndicatorText=effect.PercentageAmount + "%"
                            };
                            Player.Instance.AddEffect(controlBuff); 
                        } 
                    })
                }
            };
        }
        else if(effect_name == "DamageToCrowdControlled") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_CROWD_CONTROLLED});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl
                    )
                }
            };
        }
        else if(effect_name == "FinalAmmoDealsMoreDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> {Player.Instance.RangedInjury,Player.Instance.RangedStagger,},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.IsFinalAmmo)
                    )
                }
            };
        }
        else if(effect_name == "DamageToFrozen") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_FROZEN});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Frozen))
                    )
                }
            };
        }
        else if(effect_name == "DealMoreStaggerBasedOnEnemyDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_1_ENEMY_DAMAGE_REDUCTION_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.DamageReduction.Current > 1
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.ExtraStaggerDealtPercentage = (damage.TargetOfDamage.DamageReduction.Current - 1) * effect.FirstParameter * 100;
                    })
                }
            };
        }
        else if(effect_name == "HealFromApplyingCrowdControl") {
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
        else if(effect_name == "HealFromDamageToStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__AGAINST_STAGGERED});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.FirstParameter + damage.StaggerDealt / 100 * effect.FirstParameter; 
                    })
                }
            };
        }

















        //Molis
        if (effect_name == "FlatHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_HEALTH_INCREASE_PER_PB, new List<float> {PB.SPECIAL__INCREASE_STAT_BASE_INSTEAD_OF_PERCENTAGE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    BaseModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Health },
                    IsFlatIncreaseStatIncrease = true
                }
            };
        }
        if (effect_name == "FlatStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, new List<float> {PB.SPECIAL__INCREASE_STAT_BASE_INSTEAD_OF_PERCENTAGE});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.StaggerBar, new(effect_name)) {
                    BaseModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.StaggerBar },
                    IsFlatIncreaseStatIncrease = true
                }
            };
        }
        else if(effect_name == "BarrierDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__BARRIER});
            return new List<Effect> { 
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Barrier), Player.Instance.DamageReduction, calculatedPB, new(effect_name))
            };
        }
        else if(effect_name == "ConvertStaggerBarToHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {1 / PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, PB.MAXIMUM_HEALTH_INCREASE_PER_PB, PB.SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION});
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.StaggerBar, Player.Instance.Health, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100 * Player.Instance.StaggerBar.Maximum, 2), Utils.GetFormattedFloat(calculatedPB, 2)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Health }
                },
            };
        }
        else if(effect_name == "ConvertHealthToStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {1 / PB.MAXIMUM_HEALTH_INCREASE_PER_PB, PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, PB.SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION});
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Health, Player.Instance.StaggerBar, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB / 100 * Player.Instance.Health.Maximum, 2), Utils.GetFormattedFloat(calculatedPB, 2)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.StaggerBar}
                }
            };
        }
        else if(effect_name == "ConvertDamageReductionToTenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {1 / PB.DAMAGE_REDUCTION_INCREASE_PER_PB, PB.TENACITY_INCREASE_PER_PB, PB.SPECIAL__STAT_BONUS_CONVERSION});
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.DamageReduction, Player.Instance.Tenacity, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat((Player.Instance.DamageReduction.Maximum - 1) * calculatedPB, 2), Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Tenacity}
                }
            };
        }
        else if(effect_name == "ConvertTenacityToDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {1 / PB.TENACITY_INCREASE_PER_PB, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, PB.SPECIAL__STAT_BONUS_CONVERSION});
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Tenacity, Player.Instance.DamageReduction, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat((Player.Instance.Tenacity.Maximum - 1) * calculatedPB, 2), Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction}
                }
            };
        }
        else if(effect_name == "BarrierAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Barrier), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if(effect_name == "BarrierDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Barrier), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}
                }
            };
        }
        else if (effect_name == "MoreTenacityWhileBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__BARRIER});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.Tenacity},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Tenacity, new (effect_name)) {
                            PercentageModifier = effect.PercentageAmount, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="MoreTenacityWhileBarrier" + special_identifier, 
                            IsRemovable=false, ShowsInUI=true, 
                            EffectIndicatorText=effect.PercentageAmount + "%"
                        });
                    }), 
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => 
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect tenacityBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "MoreTenacityWhileBarrier" + special_identifier));
                        if(tenacityBuff != null) {
                            tenacityBuff.EndThisEffect();
                        } 
                    })
                }
            };
        }
        else if (effect_name == "MoreDamageReductionWhileCCed") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_CROWD_CONTROLLED});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamageReductionChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl
                    )
                }
            };
        }
        else if(effect_name == "ConvertHealthToInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {PB.DAMAGE_INCREASE_PER_PB, 1 / PB.MAXIMUM_HEALTH_INCREASE_PER_PB, PB.SPECIALIZATION__INJURY, PB.SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION});
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
        else if(effect_name == "EnergyGainFromHealthLost") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ENERGY_GAIN_FROM_HEALTH_LOST});
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * 10 / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.HealthLost
                }
            };
        }
        else if (effect_name == "HealthRegenWhileBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_SECOND_PER_PB, new List<float> {PB.REQUIREMENT__BARRIER});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    FlatAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new (effect_name)) {
                            RegenerationFlatModifier = effect.FlatAmount, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="HealthRegenWhileBarrier" + special_identifier, 
                            IsRemovable=false, ShowsInUI=true, 
                            PathToEffectGraphic="UI/HealthRegeneration", 
                            EffectIndicatorText=effect.FlatAmount.ToString()
                        });
                    }), 
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => 
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "HealthRegenWhileBarrier" + special_identifier));
                        if(regenBuff != null) {
                            regenBuff.EndThisEffect();
                        } 
                    })
                }
            };
        }
        else if (effect_name == "DamageReductionWhileLowHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_BELOW_50P_HEALTH});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                        stat.Owner == Player.Instance && stat is Health
                    ), 
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "DamageReductionWhileLowHealth" + special_identifier);
                        if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.5f && drBuff != null) {
                            drBuff.EndThisEffect();
                        }
                        else if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.5f && drBuff == null) {
                            drBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, new("DamageReductionWhileLowHealth")) {
                                PercentageModifier = effect.PercentageAmount, 
                                Identifier="DamageReductionWhileLowHealth" + special_identifier, 
                                IsRemovable=false, 
                                ShowsInUI=true, 
                                EffectIndicatorText=effect.PercentageAmount + "%"
                            };
                            Player.Instance.AddEffect(drBuff);
                        }  
                    })
                }
            };
        }
        else if(effect_name == "ConvertStaggerBarToStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {PB.DAMAGE_INCREASE_PER_PB, 1 / PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, PB.SPECIALIZATION__STAGGER, PB.SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION});
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
        else if(effect_name == "EnergyGainFromBlocking") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ENERGY_GAIN_FROM_BLOCKING});
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_PER_STAGGER_PERCENTAGE_LOST_FROM_BLOCKING * 10 / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.Block
                }
            };
        }
        else if (effect_name == "StaggerBarRegenWhileBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_STAGGER_BAR_RESTORED_PER_SECOND_PER_PB, new List<float> {PB.REQUIREMENT__BARRIER});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    FlatAmount = -calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.StaggerBar, new (effect_name)) {
                            RegenerationFlatModifier = effect.FlatAmount, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="StaggerBarRegenWhileBarrier" + special_identifier, 
                            IsRemovable=false, 
                            ShowsInUI=true, 
                            PathToEffectGraphic="UI/StaggerBarRegeneration", 
                            EffectIndicatorText=effect.FlatAmount.ToString()
                        });
                    }), 
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => 
                        effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "StaggerBarRegenWhileBarrier" + special_identifier));
                        if(regenBuff != null) {
                            regenBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if (effect_name == "DamageReductionWhileHighStaggerBar")
        {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_ABOVE_50P_STAGGER_BAR});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                        stat.Owner == Player.Instance && stat is Health
                    ), 
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "DamageReductionWhileHighStaggerBar" + special_identifier);
                        if(Player.Instance.StaggerBar.Current < Player.Instance.StaggerBar.Maximum * 0.5f && drBuff != null) {
                            drBuff.EndThisEffect();
                        }
                        else if(Player.Instance.StaggerBar.Current >= Player.Instance.StaggerBar.Maximum * 0.5f && drBuff == null) {
                            drBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, new("DamageReductionWhileHighStaggerBar")) {
                                PercentageModifier = effect.PercentageAmount, 
                                Identifier="DamageReductionWhileHighStaggerBar" + special_identifier, 
                                IsRemovable=false, 
                                ShowsInUI=true, 
                                EffectIndicatorText=effect.PercentageAmount + "%"
                            };
                            Player.Instance.AddEffect(drBuff);
                        }
                    })
                }
            };
        }
        else if(effect_name == "HealMissingHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_HEALTH_RESTORED_PER_SECOND_PER_PB, new List<float> {PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneFifthSecondElapsedInGame = new Func<bool>(() => 
                        Player.Instance.Health.Current < Player.Instance.Health.Maximum
                    ), 
                    ActionOnOneFifthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.Health.Current += (Player.Instance.Health.Maximum - Player.Instance.Health.Current) * effect.PercentageAmount / 100 / 5;
                    })
                }
            };
        }
        else if(effect_name == "HealFromStaggerTaken") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__STAGGER, PB.SPECIAL__SCALES_WITH_ENEMY_DAMAGE_INSTEAD_OF_PLAYERS});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && damage.StaggerDealt > 0 && !Player.Instance.CheckIfUnderEffect(typeof(Effect_PlayerStaggered))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                }
            };
        }


















        //Salutis
        else if(effect_name == "SalutisTechniqueCooldownReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FAMILY_TECHNIQUES, PB.SPECIALIZATION__COOLDOWN_REDUCTION_FOR_TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAboutToAddCooldown = new Func<Cooldown, bool>((cooldown) => 
                        cooldown.GetType().IsSubclassOf(typeof(Technique)) && cooldown.GetCooldownTechniqueFamily() == "Salutis" && cooldown.CooldownTarget == Player.Instance
                    ), 
                    ActionOnAboutToAddCooldown = new Action<Cooldown, Effect_CustomizableEffectOnEvent> ((cooldown, effect) =>  {
                        cooldown.ExtraCooldownReduction += effect.PercentageAmount;
                    })
                }
            };
        }
        else if(effect_name == "TechniqueCooldownReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__COOLDOWN_REDUCTION_FOR_TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAboutToAddCooldown = new Func<Cooldown, bool>((cooldown) => 
                        cooldown.GetType().IsSubclassOf(typeof(Technique)) && cooldown.CooldownTarget == Player.Instance
                    ), 
                    ActionOnAboutToAddCooldown = new Action<Cooldown, Effect_CustomizableEffectOnEvent> ((cooldown, effect) =>  {
                        cooldown.ExtraCooldownReduction += effect.PercentageAmount;
                    })
                }
            };
        }
        else if(effect_name == "EffectCooldownReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__COOLDOWN_REDUCTION_FOR_EFFECTS});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAboutToAddCooldown = new Func<Cooldown, bool>((cooldown) => 
                        cooldown.GetType().IsSubclassOf(typeof(Effect)) && cooldown.CooldownTarget == Player.Instance
                    ), 
                    ActionOnAboutToAddCooldown = new Action<Cooldown, Effect_CustomizableEffectOnEvent> ((cooldown, effect) =>  {
                        cooldown.ExtraCooldownReduction += effect.PercentageAmount;
                    })
                }
            };
        }
        else if(effect_name == "InjuryToIncised") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.REQUIREMENT__INCISION});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    InjuryPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Incision))
                    )
                }
            };
        }
        else if(effect_name == "IncisionAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Incision), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "IncisionDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Incision), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "BackstabInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.SPECIALIZATION__BACKSTAB});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    InjuryPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)
                    )
                }
            };
        }
        else if(effect_name == "TechniqueInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY, PB.SPECIALIZATION__TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    InjuryPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)
                    )
                }
            };
        }
        else if(effect_name == "BackstabsReduceCooldowns") {
            float calculatedPB = CalculatePB(power_budget, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> {PB.SPECIALIZATION__BACKSTAB});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && ability.Is(Ability.AbilityProperty.Backstab)
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        foreach(Cooldown cd in Player.Instance.TechniqueCooldowns) {
                            cd.RemainingDuration = cd.RemainingDuration - cd.RemainingDuration * effect.PercentageAmount / 100;
                        }
                        foreach(Cooldown cd in Player.Instance.EffectCooldowns) {
                            cd.RemainingDuration = cd.RemainingDuration - cd.RemainingDuration * effect.PercentageAmount / 100;
                        }
                        Player.Instance.ToolCooldown.RemainingDuration = Player.Instance.ToolCooldown.RemainingDuration - Player.Instance.ToolCooldown.RemainingDuration * effect.PercentageAmount / 100;
                    })
                }
            };
        }
        else if(effect_name == "BackstabsGiveEnergy") {
            float calculatedPB = CalculatePB(power_budget, PB.GAIN_FLAT_ENERGY_AFFECTED_BY_ENERGY_GAIN_PER_PB, new List<float> {PB.SPECIALIZATION__BACKSTAB});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    FlatAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && ability.Is(Ability.AbilityProperty.Backstab)
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.Energy.GenerateEnergy(effect.FlatAmount);
                    })
                }
            };
        }
        else if(effect_name == "BackstabsDealMoreDamageBasedOnIncision") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_DAMAGING_EFFECT_INTO_EXTRA_DAMAGE_PER_PB, new List<float> {1 / PB.INCISION_PER_PB, PB.SPECIALIZATION__BACKSTAB, PB.SPECIALIZATION__INJURY});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Incision))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Incision incision = (Effect_Incision)damage.TargetOfDamage.GetEffect(typeof(Effect_Incision));
                        damage.ExtraInjuryDealtFlat += incision.DecayingAmount * effect.FirstParameter / 100;
                    })
                }
            };
        }
        else if(effect_name == "BasicAttackInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__BASIC_ATTACK, PB.SPECIALIZATION__INJURY});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    InjuryPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    )
                }
            };
        }
        else if(effect_name == "InjuryDamageReductionPenetration") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_PENETRATION_PER_PB, new List<float> {PB.SPECIALIZATION__INJURY});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    IgnorePercentageOfDamageReduction = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    )
                }
            };
        }
        else if(effect_name == "BackstabsScaleWithOnslaughtSharpAndAnalysis") {
            
            float calculatedPB1 = CalculatePB(power_budget * 0.333f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ONSLAUGHT_PER_PB, PB.SPECIALIZATION__BACKSTAB, PB.SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS});
            float calculatedPB2 = CalculatePB(power_budget * 0.333f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB, PB.SPECIALIZATION__BACKSTAB, PB.SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS});
            float calculatedPB3 = CalculatePB(power_budget * 0.333f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ANALYSIS_PER_PB, PB.SPECIALIZATION__BACKSTAB, PB.SPECIAL__BONUS_FOR_AFFECTING_3_DIFFERENT_STACKING_EFFECTS});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB1,
                    SecondParameter = calculatedPB2,
                    DamagePercentageChange = calculatedPB3,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Onslaught))) {
                            Effect_Onslaught onslaught = (Effect_Onslaught)damage.TargetOfDamage.GetEffect(typeof(Effect_Onslaught));
                            damage.ExtraDamageDealtPercentage += onslaught.DecayingAmount * effect.FirstParameter / 100;
                        }
                        if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))) {
                            Effect_Sharp sharp = (Effect_Sharp)damage.TargetOfDamage.GetEffect(typeof(Effect_Sharp));
                            damage.ExtraDamageDealtPercentage += sharp.DecayingAmount * effect.SecondParameter / 100;
                        }
                        if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis))) {
                            Effect_Analysis analysis = (Effect_Analysis)damage.TargetOfDamage.GetEffect(typeof(Effect_Analysis));
                            damage.ExtraDamageDealtPercentage += analysis.DecayingAmount * effect.DamagePercentageChange / 100;
                        }
                    })
                }
            };
        }
        else if(effect_name == "BackstabsApplyIncision") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.INCISION_PER_PB, PB.SPECIALIZATION__BACKSTAB});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Incision(damage.InjuryDealt * effect.FirstParameter + damage.StaggerDealt * effect.FirstParameter, new(damage.SourceOfDamage)));
                    })
                }
            };
        }
        else if(effect_name == "HealFromBackstabs") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__BACKSTAB});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},  
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.DamagePercentageChange + damage.StaggerDealt / 100 * effect.DamagePercentageChange;
                    })
                }
            };
        }
        else if(effect_name == "HealFromTakedowns") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__TAKEDOWN});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitKnockedOut = new Func<Damage, bool>((damage) => 
                        damage.TargetOfDamage.IsHostile
                    ), 
                    ActionOnUnitKnockedOut = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum * effect.PercentageAmount / 100;
                    }),
                    ConditionCheckForHealthBarBroken = new Func<Damage, bool>((damage) => 
                        damage.TargetOfDamage.IsHostile
                    ), 
                    ActionOnHealthBarBroken = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum * effect.PercentageAmount / 100;
                    })
                }
            };
        }
























        //Tonitrui
        else if(effect_name == "SuperchargeAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Supercharge), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "SuperchargeDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Supercharge), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "ToolCooldownReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__COOLDOWN_REDUCTION_FOR_TOOLS});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAboutToAddCooldown = new Func<Cooldown, bool>((cooldown) => 
                        cooldown.GetType().IsSubclassOf(typeof(Tool)) && cooldown.CooldownTarget == Player.Instance
                    ), 
                    ActionOnAboutToAddCooldown = new Action<Cooldown, Effect_CustomizableEffectOnEvent> ((cooldown, effect) =>  {
                        cooldown.ExtraCooldownReduction += effect.PercentageAmount;
                    })
                }
            };
        }
        else if(effect_name == "SuperchargeMovementSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.MOVEMENT_SPEED_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SUPERCHARGE});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect is Effect_Supercharge && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.MovementSpeed, new (effect_name)) {
                            PercentageModifier = effect.PercentageAmount, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="SuperchargeMovementSpeed" + special_identifier, 
                            IsRemovable=false, 
                            ShowsInUI=true, 
                            EffectIndicatorText=effect.PercentageAmount + "%"
                        });
                    }), 
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => 
                        effect is Effect_Supercharge && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "SuperchargeMovementSpeed" + special_identifier));
                        if(msBuff != null) {
                            msBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if(effect_name == "SuperchargeDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SUPERCHARGE});
            return new List<Effect> { 
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Supercharge), Player.Instance.DamageReduction, calculatedPB, new(effect_name))
            };
        }
        else if(effect_name == "SuperchargeInjuryDealt") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__SUPERCHARGE_DAMAGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    InjuryPercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.Supercharge)
                    )
                }
            };
        }
        else if(effect_name == "SpendingSuperchargeGeneratesBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_ONE_STACKING_EFFECT_SPENT_INTO_ANOTHER_PER_PB, new List<float> {PB.BARRIER_PER_PB, 1 / PB.SUPERCHARGE_PER_PB});
            return new List<Effect> {
                new Effect_Description(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    Identifier="SpendingSuperchargeGeneratesBarrier", 
                    PercentageAmount = calculatedPB
                }
            };
        }
        else if(effect_name == "SuperchargeInjuryFromBasicAttacks") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__BASIC_ATTACK, PB.SPECIALIZATION__SUPERCHARGE_DAMAGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    InjuryPercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.Supercharge)
                    )
                }
            };
        }
        else if(effect_name == "ConvertMovementSpeedToDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {1 / PB.MOVEMENT_SPEED_INCREASE_PER_PB, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, PB.SPECIAL__STAT_BONUS_CONVERSION});
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.Tenacity, Player.Instance.DamageReduction, calculatedPB, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat((Player.Instance.MovementSpeed.Maximum - 1) * calculatedPB, 2), Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.DamageReduction}
                }
            };
        }
        else if(effect_name == "HealFromDistanceTravelled") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEALTH_RESTORED_PER_1_M_TRAVELLED_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForOneFifthSecondElapsedInGame = new Func<bool>(() => 
                        Player.Instance.Health.Current < Player.Instance.Health.Maximum
                    ), 
                    ActionOnOneFifthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        if(Player.Instance.PlayerSavedPosition == null) {
                            Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                        }
                        Player.Instance.Health.Current += Vector2.Distance(Player.Instance.PlayerSavedPosition, Player.Instance.transform.position) * effect.PercentageAmount / 100 / 5;
                        Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                    })
                }
            };
        }
        else if(effect_name == "HealMoreFromHealthPotions") {
            float calculatedPB = CalculatePB(power_budget, PB.EXTRA_MAXIMUM_HEALTH_RESTORED_BY_HEALTH_POTION_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && ability is Ability_Heal
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {RegenerationPercentageModifier = effect.PercentageAmount / 2 }, 2);
                    })
                }
            };
        }
        
        





























        //Proprius
        else if(effect_name == "EnergyGainFromBasicAttacks") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ENERGY_GAIN_FROM_BASIC_ATTACKS});
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_BASIC_ATTACK / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.BasicAttack
                }
            };
        }
        else if(effect_name == "EnergyGainFromDodges") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ENERGY_GAIN_FROM_DODGING});
            return new List<Effect> {
                new Effect_GainMoreEnergyFromSpecifiedSource(new(effect_name)) {
                    IncreaseAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * Constants.ENERGY_FROM_DODGING / 100)},
                    SpecifiedSource = Constants.EnergyGainSource.Dodge
                }
            };
        }
        else if(effect_name == "EnergyGainFromRipostesAndCounters") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__RIPOSTES_COUNTERS});
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
        else if(effect_name == "EnergyGainFromStaggering") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ENERGY_GAIN_FROM_STAGGERING});
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
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamagePercentageChange = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)
                    )
                }
            };
        }
        else if(effect_name == "AnalysisAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Analysis), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "AnalysisDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Analysis), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "AnalysisBoostsAllDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.ANALYSIS_PER_PB, PB.SPECIALIZATION__EVERYTHING_EXCEPT_TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamagePercentageChange = calculatedPB,  
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},   
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis)) && damage.SourceOfDamage.IsNot(Ability.AbilityProperty.Technique)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Analysis analysis = (Effect_Analysis)Player.Instance.GetEffect(typeof(Effect_Analysis));
                        damage.ExtraDamageDealtPercentage += analysis.DecayingAmount * effect.DamagePercentageChange / 100;
                    })
                }
            };
        }
        else if(effect_name == "UltimateTechniqueDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ULTIMATE_TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Ultimate)
                    )
                }
            };
        }
        else if(effect_name == "PropriusTechniqueCooldownReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FAMILY_TECHNIQUES, PB.SPECIALIZATION__COOLDOWN_REDUCTION_FOR_TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForAboutToAddCooldown = new Func<Cooldown, bool>((cooldown) => 
                        cooldown.GetType().IsSubclassOf(typeof(Technique)) && cooldown.GetCooldownTechniqueFamily() == "Proprius" && cooldown.CooldownTarget == Player.Instance
                    ), 
                    ActionOnAboutToAddCooldown = new Action<Cooldown, Effect_CustomizableEffectOnEvent> ((cooldown, effect) =>  {
                        cooldown.ExtraCooldownReduction += effect.PercentageAmount;
                    })
                }
            };
        }
        else if(effect_name == "AnalysisEnergyGain") {
            float calculatedPB = CalculatePB(power_budget, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__ANALYSIS});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect is Effect_Analysis && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.EnergyGain, new (effect_name)) {
                            PercentageModifier = effect.PercentageAmount, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="AnalysisEnergyGain", 
                            IsRemovable=false, ShowsInUI=true, 
                            EffectIndicatorText=effect.PercentageAmount + "%"
                        });
                    }), 
                    ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => 
                        effect is Effect_Analysis && effect.TargetOfEffect == Player.Instance
                    ), 
                    ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "AnalysisEnergyGain" + special_identifier));
                        if(msBuff != null) {
                            msBuff.EndThisEffect();
                        }
                    })
                }
            };
        }
        else if(effect_name == "AnalysisDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__ANALYSIS});
            return new List<Effect> { 
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Analysis), Player.Instance.DamageReduction, calculatedPB, new(effect_name))
            };
        }
        else if(effect_name == "PropriusUltimateTechniqueDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__ULTIMATE_TECHNIQUES, PB.SPECIALIZATION__FAMILY_TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyInjury,Player.Instance.LightInjury,Player.Instance.RangedInjury,Player.Instance.MagicInjury, Player.Instance.HeavyStagger,Player.Instance.LightStagger,Player.Instance.RangedStagger,Player.Instance.MagicStagger },
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Ultimate) && Technique.GetFamily(damage.SourceOfDamage.GetType()) == Ability.AbilityFamily.Proprius
                    )
                }
            };
        }
        else if (effect_name == "TechniqueDamageGivesAnalysis")
        {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.ANALYSIS_PER_PB, PB.REQUIREMENT__DEALING_DAMAGE, PB.SPECIALIZATION__TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Analysis(calculatedPB, new(effect_name)));
                    })
                }
            };
        }
        else if (effect_name == "ConvertEnergyGainIntoHealthAndStaggerBar")
        {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {PB.MAXIMUM_HEALTH_INCREASE_PER_PB, 1 / PB.ENERGY_GAIN_INCREASE_PER_PB, 1 / PB.SPECIAL__TOTAL_STAGGER_BAR_AMOUNT_CONVERSION});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.CONVERT_ONE_PERCENTAGE_OF_ONE_STAT_INTO_ANOTHER_PER_PB, new List<float> {PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB, 1 / PB.ENERGY_GAIN_INCREASE_PER_PB,  1 / PB.SPECIAL__TOTAL_HEALTH_AMOUNT_CONVERSION});
            return new List<Effect> {
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.EnergyGain, Player.Instance.Health, calculatedPB1, new(effect_name) ) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                },
                new Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Player.Instance.EnergyGain, Player.Instance.StaggerBar, calculatedPB2, new(effect_name) ) {},
            };
        }
        else if(effect_name == "HealFromTechniqueDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__TECHNIQUES});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.FirstParameter + damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                }
            };
        }
        else if(effect_name == "HealFromEnergyUsed") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEALTH_RESTORED_PER_1_ENERGY_SPENT_PER_PB);
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 10), "10"},
                    ConditionCheckForAbilityEnergyConsumed = new Func<Ability, float, bool>((ability, amount) => 
                        ability.User == Player.Instance && amount > 0
                    ), 
                    ActionOnAbilityEnergyConsumed = new Action<Ability, float, Effect_CustomizableEffectOnEvent> ((ability, amount, effect) =>  {
                        Player.Instance.Health.Current += Player.Instance.Health.Maximum * amount * calculatedPB / 100;
                    })
                }
            };
        }





































        //Duelist
        else if(effect_name == "GainSharpOnRiposteCounterOrDodge") {
            float calculatedPB1 = CalculatePB(power_budget * 0.3f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.SHARP_PER_PB, PB.REQUIREMENT__RIPOSTE});
            float calculatedPB2 = CalculatePB(power_budget * 0.4f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.SHARP_PER_PB, PB.REQUIREMENT__COUNTER});
            float calculatedPB3 = CalculatePB(power_budget * 0.3f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.SHARP_PER_PB, PB.REQUIREMENT__DODGE});
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3)}, 
                    FirstParameter = calculatedPB1, 
                    SecondParameter = calculatedPB2, 
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(ability.Is(Ability.AbilityProperty.Riposte) ? effect.FirstParameter : effect.SecondParameter, new(effect_name)));
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    TriggersOncePerAbility=true, 
                    FlatAmount = calculatedPB3, 
                    ConditionCheckForDamageWasDodged = new Func<Damage, Ability, bool>((damage, dodge) => 
                        damage.TargetOfDamage == Player.Instance
                    ), 
                    ActionOnDamageWasDodged = new Action<Damage, Ability, Effect_CustomizableEffectOnEvent> ((damage, dodge, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(effect.FlatAmount, new(effect_name)));
                    })
                }
            };
        }
        else if(effect_name == "SharpDamageReductionWithoutLimit") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {1 / (PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER * PB.SHARP_PER_PB)});
            return new List<Effect> {
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Sharp), Player.Instance.DamageReduction, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<String>{Utils.GetFormattedFloat(calculatedPB)}, 
                    IncreaseBasedOnEffectLevel = false
                },
            };
        }
        else if(effect_name == "GainSharpOnBasicAttacksAndOnslaughtOnRipostesAndCounters") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.SHARP_PER_PB, PB.REQUIREMENT__BASIC_ATTACK});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.ONSLAUGHT_PER_PB, PB.SPECIALIZATION__RIPOSTES_COUNTERS});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}, 
                    FirstParameter = calculatedPB1, 
                    TriggersOncePerAbility = true, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(effect.FirstParameter, effect.SourceOfEffect));
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FlatAmount= calculatedPB2, 
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Onslaught(effect.FlatAmount, effect.SourceOfEffect));
                    })
                },
            };
        }
        else if(effect_name == "RestoreHealthOnRiposteOrCounter") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__RIPOSTE_COUNTER_OR_DODGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)}, 
                    FirstParameter = calculatedPB, 
                    TriggersOncePerAbility = false, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte) || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FirstParameter;
                    })
                }
            };
        }
        else if(effect_name == "GainInvincibleOnRiposteOrCounter") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.SECONDS_OF_INVINCIBILITY_PER_PB, new List<float> {PB.REQUIREMENT__RIPOSTE});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.SECONDS_OF_INVINCIBILITY_PER_PB, new List<float> {PB.REQUIREMENT__COUNTER});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}, 
                    FirstParameter = calculatedPB1, 
                    FlatAmount = calculatedPB2,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && (ability.Is(Ability.AbilityProperty.Counter) || ability.Is(Ability.AbilityProperty.Riposte))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Invincible(new(effect_name)) {ShowsInUI = true}, ability.Is(Ability.AbilityProperty.Counter) ? effect.FlatAmount : effect.FirstParameter);
                    })
                }
            };
        }
        else if(effect_name == "EmpowerNextAttackAfterRiposteOrCounter") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__RIPOSTE, PB.RESTRICTION__ONE_TIME_ONLY});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__COUNTER, PB.RESTRICTION__ONE_TIME_ONLY});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}, 
                    FirstParameter = calculatedPB1, 
                    SecondParameter = calculatedPB2,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && (ability.Is(Ability.AbilityProperty.Counter) || ability.Is(Ability.AbilityProperty.Riposte))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(effect_name)) {
                            DamagePercentageChange = ability.Is(Ability.AbilityProperty.Riposte) ? effect.FirstParameter : effect.SecondParameter,
                            ShowsInUI = true,
                            PathToEffectGraphic = "Effect/Empowered",
                            EffectIndicatorText = ability.Is(Ability.AbilityProperty.Riposte) ? (Utils.GetFormattedFloat(effect.FirstParameter, 0) + "%") : (Utils.GetFormattedFloat(effect.SecondParameter, 0) + "%")
                        });
                    })
                }
            };
        }
        else if(effect_name == "ExtraEffectiveButConsumableSharp") {
            float calculatedPB1 = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB * -0.5f);
            float calculatedPB2 = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB * 1.5f, new List<float> {PB.RESTRICTION__CONSUME_ALL_SCALING_STACKING_EFFECT_WHEN_USED});
            return new List<Effect> { 
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB1, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    ConditionCheckForAbilityEnded = new Func<Ability, bool>((ability) => 
                        ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter)) && ability.User.CheckIfUnderEffect(typeof(Effect_Sharp))
                    ), 
                    ActionOnAbilityEnded = new Action<Ability, Effect_CustomizableEffectOnEvent> ((Ability, effect) =>  {
                        Effect sharp = Player.Instance.GetEffect(typeof(Effect_Sharp));
                        sharp.EndThisEffect();
                    })
                },
                new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB2, new(effect_name))
            };
        }
        else if(effect_name == "ReceiveSharpWhileInProximity") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.SHARP_PER_PB, PB.REQUIREMENT__AN_ENEMY_IS_IN_5M_RANGE});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {"5", Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB)}, 
                    FlatAmount = calculatedPB, 
                    ConditionCheckForOneFifthSecondElapsedInGame = new Func<bool>(() => 
                        Utils.GetAllUnits(true, true).FirstOrDefault(enemy => Vector2.Distance(enemy.transform.position, Player.Instance.transform.position) < 5) != null
                    ), 
                    ActionOnOneFifthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.AddEffect(new Effect_Sharp(effect.FlatAmount / 5, effect.SourceOfEffect));
                    })
                }
            };
        }


        //Weaponmaster
        else if(effect_name == "WeaponTechniquesEmpowerNextBasicAttackAndViceVersa") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__BASIC_ATTACK, PB.REQUIREMENT__WEAPON_TECHNIQUE, PB.RESTRICTION__LASTS_5_SECONDS});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__TECHNIQUES, PB.SPECIALIZATION__WEAPONS, PB.REQUIREMENT__BASIC_ATTACK, PB.RESTRICTION__LASTS_5_SECONDS});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB1,
                    DescriptionParameters = new List<String> {"5", Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.IsWeaponDamage
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(
                        new Effect_CustomizableDamageChange(effect.SourceOfEffect) {
                            FirstParameter = effect.FirstParameter, 
                            ShowsInUI=true, 
                            PathToEffectGraphic="UI/AllWeapons", 
                            Identifier="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa - EmpoweredBasicAttack" + special_identifier, 
                            EffectIndicatorText=effect.FirstParameter + "%", 
                            ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage2, effect2) => 
                                damage2.SourceOfDamage.User == Player.Instance && damage2.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                            ), 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage3, effect3) =>  {
                                damage3.ExtraInjuryDealtPercentage += effect3.FirstParameter;
                                damage3.ExtraStaggerDealtPercentage += effect3.FirstParameter;
                                effect3.EndThisEffect();
                            })
                        }
                    , 5);
                })},
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter =  calculatedPB2,
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(
                            new Effect_CustomizableDamageChange(effect.SourceOfEffect) {
                                FirstParameter = effect.FirstParameter, 
                                ShowsInUI=true, 
                                PathToEffectGraphic="UI/Technique", 
                                Identifier="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa - EmpoweredWeaponTechnique" + special_identifier, 
                                EffectIndicatorText=effect.FirstParameter + "%", 
                                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage2, effect2) => 
                                    damage2.SourceOfDamage.User == Player.Instance && damage2.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage2.IsWeaponDamage
                                ), 
                                BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage3, effect3) =>  {
                                    damage3.ExtraInjuryDealtPercentage += effect3.FirstParameter;
                                    damage3.ExtraStaggerDealtPercentage += effect3.FirstParameter;
                                    effect3.EndThisEffect();
                                })
                            }, 
                        5);
                    }
                )}
            };
        }
        else if(effect_name == "BlademastersGarb") {
            float calculatedPB1 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE, PB.SPECIALIZATION__BASIC_ATTACK});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_REDUCTION_INCREASE_PER_PB);
            float calculatedPB3 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_REDUCTION_INCREASE_PER_PB);
            float calculatedPB4 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE, PB.SPECIALIZATION__BASIC_ATTACK});
            return new List<Effect> { 
                new Effect_BlademastersGarb(new(effect_name)) {
                    DescriptionParameters = new List<String>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2), Utils.GetFormattedFloat(calculatedPB3), Utils.GetFormattedFloat(calculatedPB4)},
                    BladedBasicAttackBuff = calculatedPB1,
                    BladedDamageReductionDebuff = calculatedPB2,
                    NonBladedDamageReductionBuff = calculatedPB3,
                    NonBladedBasicAttackDebuff = calculatedPB4
                }
            };
        }
        else if(effect_name == "RestoreHealthOnWeaponDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__WEAPON_DAMAGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)}, 
                    FirstParameter = calculatedPB, 
                    TriggersOncePerAbility = true, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.IsWeaponDamage
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FirstParameter;
                    })
                }
            };
        }
        else if(effect_name == "WeaponAttackSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPONS});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.HeavyAttackSpeed}
                },
                new Effect_ChangeStat(Player.Instance.LightAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.LightAttackSpeed}
                },
                new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, new(effect_name)) {
                    PercentageModifier = calculatedPB,
                    ShowCalculatedStatIncreasesBasedOnFirstStringParam = new List<Stat> { Player.Instance.RangedAttackSpeed}
                }
            };
        }
        else if(effect_name == "IncreasedWeaponDamageButDecreasedMagicDamage") {
            float calculatedPB1 = CalculatePB(power_budget * 1.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__WEAPONS});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__MAGIC});
                return new List<Effect> { 
                    new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                        PercentageModifier = calculatedPB1, 
                        DescriptionParameters=new List<string>{Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}
                    }, 
                    new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                        PercentageModifier = calculatedPB1
                    }, 
                    new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                        PercentageModifier = calculatedPB1
                    }, 
                    new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                        PercentageModifier = calculatedPB1
                    }, 
                    new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                        PercentageModifier = calculatedPB1
                    }, 
                    new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                        PercentageModifier = calculatedPB1
                    }, 
                    new Effect_ChangeStat(Player.Instance.MagicInjury, new(effect_name)) {
                        PercentageModifier = -calculatedPB2
                    }, 
                    new Effect_ChangeStat(Player.Instance.MagicStagger, new(effect_name)) {
                        PercentageModifier = -calculatedPB2
                    }
            };
        }


        //Jailer
        else if(effect_name == "DealingOrTakingDamageAppliesChainedToYou") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.CHAINED_PER_PB, PB.REQUIREMENT__DEALING_OR_TAKING_DAMAGE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)}, 
                    FirstParameter = calculatedPB, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance || damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FirstParameter, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if(effect_name == "ChainedAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "ChainedDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_DECAY_INCREASE_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                }
            };
        }
        else if(effect_name == "GainDamageAndDamageReductionForEachDebuff") {
            float calculatedPB1 = CalculatePB(power_budget * 0.666f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_DEBUFFED, 1 / PB.EXPECTED_AMOUNT_OF_DEBUFFS_ON_PLAYER});
            float calculatedPB2 = CalculatePB(power_budget * 0.334f, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_DEBUFFED, 1 / PB.EXPECTED_AMOUNT_OF_DEBUFFS_ON_PLAYER});
            return new List<Effect> {
                new Effect_GainDamageAndDamageReductionForEachDebuff(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)},
                    DamageGainedPerDebuff = calculatedPB1, 
                    DamageReductionGainedPerDebuff = calculatedPB2, 
                    MaxDebuffs = 10
                }
            };
        }
        else if(effect_name == "ChainedDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__CHAINED});
            return new List<Effect> { 
                new Effect_IncreaseStatBasedOnStackingEffectLevel(typeof(Effect_Chained), Player.Instance.DamageReduction, calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), Utils.GetFormattedFloat(calculatedPB * 0.5f)},
                },
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB * 0.5f,
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Chained))
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Chained burn = (Effect_Chained)damage.SourceOfDamage.User.GetEffect(typeof(Effect_Chained));
                        damage.ExtraDamageReduction += burn.StackingEffectIntensityLevel == 1 ? effect.FirstParameter / 2 : burn.StackingEffectIntensityLevel == 2 ? effect.FirstParameter : burn.StackingEffectIntensityLevel == 3 ? effect.FirstParameter * 2 : 0;
                    })
                }
            };
        }
        else if(effect_name == "DodgingAndCounteringAppliesChained") {
            float calculatedPB1 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.CHAINED_PER_PB, PB.REQUIREMENT__DODGE});
            float calculatedPB2 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.CHAINED_PER_PB, PB.REQUIREMENT__RIPOSTE});
            float calculatedPB3 = CalculatePB(power_budget * 0.333f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.CHAINED_PER_PB, PB.REQUIREMENT__CHAINED});
            float calculatedPB = calculatedPB1 + calculatedPB2 + calculatedPB3;
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    FlatAmount = calculatedPB, 
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, effect.SourceOfEffect));
                    })
                },
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    TriggersOncePerAbility=true, 
                    FlatAmount = calculatedPB, 
                    ConditionCheckForDamageWasDodged = new Func<Damage, bool>((damage) => 
                        damage.TargetOfDamage == Player.Instance
                    ), 
                    ActionOnDamageWasDodged = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if(effect_name == "RestoreStaggerFromDebuffs") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_STAGGER_BAR_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__BEING_DEBUFFED, PB.RESTRICTION__5_SECOND_COOLDOWN});
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    FlatAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "5"},
                    ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                        effect.TargetOfEffect == Player.Instance && effect.Type == Effect.EffectType.Debuff
                    ), 
                    ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectStarted, effect) =>  {
                        Player.Instance.StaggerBar.Current -= effect.FlatAmount;
                    })
                }
            };
        }
        else if(effect_name == "HealFromBeingDamagedByStackingEffects") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> {PB.SPECIALIZATION__ENEMY_STACKING_EFFECT_DAMAGE, PB.SPECIAL__SCALES_WITH_ENEMY_DAMAGE_INSTEAD_OF_PLAYERS});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    DescriptionParameters=new List<string>{Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance && (damage.Is(Damage.DamageProperty.Burn) || damage.Is(Damage.DamageProperty.Freeze) || damage.Is(Damage.DamageProperty.Incision))
                    ), 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.FirstParameter + damage.StaggerDealt / 100 * effect.FirstParameter;
                    })
                } 
            };
        }
        else if(effect_name == "ApplySelfChainedToEnemies") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SCALING_STACKING_EFFECT_ON_PLAYER});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)}, 
                    FirstParameter = calculatedPB, 
                    TriggersOncePerAbility = true, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && Player.Instance.CheckIfUnderEffect(typeof(Effect_Chained))
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Chained playersChained = (Effect_Chained)Player.Instance.GetEffect(typeof(Effect_Chained));
                        damage.TargetOfDamage.AddEffect(new Effect_Chained(playersChained.DecayingAmount * effect.FirstParameter / 100, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if(effect_name == "GainChainedOnHeavyDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.CHAINED_PER_PB, PB.REQUIREMENT__WEAPON_DAMAGE, PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DescriptionParameters=new List<String>{Utils.GetFormattedFloat(calculatedPB)}, 
                    FirstParameter = calculatedPB, 
                    TriggersOncePerAbility = true, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Chained(effect.FirstParameter, effect.SourceOfEffect));
                    })
                }
            };
        }
        else if(effect_name == "ConvertCurrentHealthIntoChained") {
            float currentHealthPercentageLostEachSecond = 2;
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.CHAINED_PER_PB, currentHealthPercentageLostEachSecond * PB.RESTRICTION__LOSE_1P_CURRENT_HEALTH_EACH_SECOND});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    DescriptionParameters = new List<String> {currentHealthPercentageLostEachSecond.ToString(), Utils.GetFormattedFloat(calculatedPB)}, 
                    FlatAmount = calculatedPB, 
                    ConditionCheckForOneFifthSecondElapsedInGame = new Func<bool>(() => 
                        Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.1f && Player.Instance.InCombat
                    ), 
                    ActionOnOneFifthSecondElapsedInGame = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                        Player.Instance.Health.Current -= Player.Instance.Health.Maximum * 0.02f / 5;
                        Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount * (Player.Instance.Health.Maximum * 0.02f / 5 / 100), effect.SourceOfEffect));
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksRestoreHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__BASIC_ATTACK});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<String> { Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FirstParameter;  
                    })
                }
            };
        }
        else if(effect_name == "ConvertChainedIntoHealth") {
            float percentageOfChainedConsumed = 15;
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> {PB.REQUIREMENT__BASIC_ATTACK, PB.REQUIREMENT__CHAINED, percentageOfChainedConsumed * PB.RESTRICTION__CONSUME_1P_OF_STACKING_EFFECT});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> { percentageOfChainedConsumed.ToString(), Utils.GetFormattedFloat(calculatedPB * 10)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                            damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Chained))
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Chained playersChained = (Effect_Chained)Player.Instance.GetEffect(typeof(Effect_Chained));
                        Player.Instance.Health.Current += effect.FirstParameter * playersChained.DecayingAmount * 0.15f;
                        Effect e2 = Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "ConvertedChainedGeneratesBarrier"));
                        if(e2 != null) {
                            Player.Instance.AddEffect(new Effect_Barrier(e2.FirstParameter * playersChained.DecayingAmount * 0.15f, effect.SourceOfEffect));
                        }  
                        playersChained.ChangeDecayingAmount(-playersChained.DecayingAmount * 0.15f);
                    })
                }
            };
        }
        else if(effect_name == "ConvertedChainedGeneratesBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.PERCENTAGE_CONVERSION_OF_ONE_STACKING_EFFECT_SPENT_INTO_ANOTHER_PER_PB, new List<float> {PB.BARRIER_PER_PB, 1 / PB.CHAINED_PER_PB});
            return new List<Effect> {
                new Effect_Description(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB * 10)},
                    Identifier="ConvertedChainedGeneratesBarrier", 
                    FirstParameter = calculatedPB
                }
            };
        }
        else if(effect_name == "GainBurnOnBasicAttackAndIncreaseDamageBasedOnBurn") {
            float calculatedPB1 = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB * 0.25f, new List<float> {PB.BURN_PER_PB, PB.REQUIREMENT__BASIC_ATTACK});
            float calculatedPB2 = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB * 1.25f, new List<float> {PB.SPECIALIZATION__WEAPON_TYPE});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2 * 10)}, 
                    FirstParameter = calculatedPB1,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.User == Player.Instance && ability.Is(Ability.AbilityProperty.BasicAttack)
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Burn(effect.FirstParameter, effect.SourceOfEffect));
                    })
                },
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB2, 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.SourceOfDamage?.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Burn)) && damage.DamageType == Constants.DamageType.Ranged
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Effect_Burn playersBurn = (Effect_Burn)Player.Instance.GetEffect(typeof(Effect_Burn));
                        damage.ExtraDamageDealtPercentage += playersBurn.DecayingAmount * effect.FirstParameter / 100 / 10;
                    })
                }
            };
        }
        else if(effect_name == "CleanseBurnOnWeaponSwitch") {
            float calculatedPB = CalculatePB(power_budget, PB.REMOVE_DAMAGING_STACKING_EFFECT_FROM_PLAYER_PER_PB, new List<float> {PB.BURN_PER_PB, PB.RESTRICTION__30_SECOND_COOLDOWN});
            return new List<Effect> {
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB), "30"}, 
                    FirstParameter = 30, 
                    FlatAmount = calculatedPB,
                    ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                        ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch)) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Burn)) && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Identifier == "CleanseBurnOnWeaponSwitch" + special_identifier) == null
                    ), 
                    ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                        Player.Instance.AddCooldown(new Cooldown(effect.GetType(), effect.FirstParameter, Player.Instance, "CleanseBurnOnWeaponSwitch" + special_identifier));
                        Effect_Burn playersBurn = (Effect_Burn)Player.Instance.GetEffect(typeof(Effect_Burn));
                        playersBurn.ChangeDecayingAmount(-effect.FlatAmount);
                    })
                }
            };
        }


        //Ancient
        else if(effect_name == "AncientHelmet") {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SPECIFIC_ITEM_EQUIPPED});
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.COOLDOWN_REDUCTION_INCREASE_PER_PB);
            return new List<Effect> { 
                new Effect_AncientCrown(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FirstParameter = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.CooldownReduction, new(effect_name)) { 
                    PercentageModifier = calculatedPB2
                }
            };
        }
        else if(effect_name == "AncientGloves") {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SPECIFIC_ITEM_EQUIPPED});
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.ENERGY_GAIN_INCREASE_PER_PB);
            return new List<Effect> { 
                new Effect_AncientGauntlets(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FirstParameter = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.EnergyGain, new(effect_name)) { 
                    PercentageModifier = calculatedPB2
                }
            };
        }
        else if(effect_name == "AncientArmor") {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SPECIFIC_ITEM_EQUIPPED});
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.DAMAGE_INCREASE_PER_PB);
            return new List<Effect> { 
                new Effect_AncientBreastplate(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FirstParameter = calculatedPB1
                },
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(effect_name)) { 
                    PercentageModifier = calculatedPB2
                }
            };

        }
        else if(effect_name == "AncientBoots") {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__SPECIFIC_ITEM_EQUIPPED});
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.ATTACK_SPEED_INCREASE_PER_PB);
            return new List<Effect> { 
                new Effect_AncientGreaves(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FirstParameter = calculatedPB1
                },
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(effect_name)) { 
                    PercentageModifier = calculatedPB2
                }
            };
        }
        else if(effect_name == "AncientFirearmsDamage") {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SPECIFIC_ITEM_EQUIPPED, PB.REQUIREMENT__SPECIFIC_ITEM_EQUIPPED});
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.DAMAGE_INCREASE_PER_PB);
            return new List<Effect> { 
                new Effect_AncientFirearmsDamage(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FirstParameter = calculatedPB1
                },
                new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(effect_name)) { 
                    PercentageModifier = calculatedPB2
                }
            };
        }
        else if(effect_name == "AncientFirearmsDamageReduction") {
            float calculatedPB1 = CalculatePB(power_budget * 0.1f, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {1 / PB.EXPECTED_AMOUNT_OF_SPECIFIC_ITEM_EQUIPPED, PB.REQUIREMENT__SPECIFIC_ITEM_EQUIPPED});
            float calculatedPB2 = CalculatePB(power_budget * 0.9f, PB.DAMAGE_REDUCTION_INCREASE_PER_PB);
            return new List<Effect> { 
                new Effect_AncientFirearmsDamageReduction(new(effect_name)) {
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB1)},
                    FirstParameter = calculatedPB1
                },
                new Effect_ChangeStat(Player.Instance.EnergyGain, new(effect_name)) { 
                    PercentageModifier = calculatedPB2
                }
            };
        }

        //BattleBorn
        else if(effect_name == "GainAttackSpeedAsHealthLowers") {
            float calculatedPB = CalculatePB(power_budget, PB.ATTACK_SPEED_INCREASE_PER_PB, new List<float> {PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM});
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                        stat.Owner == Player.Instance && stat is Health
                    ), 
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeCompositeStat asBuff = (Effect_ChangeCompositeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "GainAttackSpeedAsHealthLowers" + special_identifier);
                        if(asBuff == null) {
                            asBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, effect.SourceOfEffect) {
                                PercentageModifier = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 
                                Identifier="GainAttackSpeedAsHealthLowers" + special_identifier, 
                                IsRemovable=false, 
                                ShowsInUI=true, 
                                EffectIndicatorText=Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%"
                            };
                            Player.Instance.AddEffect(asBuff);
                        }
                        else {
                            asBuff.PercentageModifier = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                            asBuff.EffectIndicatorText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%";
                        }
                    })
                }
            };
        }
        else if(effect_name == "TakingDamageGivesBarrierBasedOnMissingHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.BARRIER_PER_PB, PB.REQUIREMENT__GETTING_DAMAGED, PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_Barrier(effect.FirstParameter * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), effect.SourceOfEffect));
                    })
                }
            };
        }
        else if(effect_name == "LoseHealthWhileAboveHalfAndRegenWhileBelow") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_ABOVE_50P_HEALTH, PB.SPECIAL__SCALES_WITH_CURRENT_HEALTH_INSTEAD_OF_MAXIMUM});
            float calculatedPB2 = CalculatePB(power_budget * 1.5f, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_BELOW_50P_HEALTH, PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM});
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) {
                    FirstParameter = calculatedPB1,
                    SecondParameter = calculatedPB2,
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
                                RegenerationPercentageModifier = -effect.FirstParameter,
                                Identifier = "LoseHealthWhileAboveHalfAndRegenWhileBelow - LoseHealth"
                            });
                        }
                        if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.5f && plusRegen == null) {
                            Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, effect.SourceOfEffect) {
                                RegenerationPercentageModifier = effect.SecondParameter,
                                Identifier = "LoseHealthWhileAboveHalfAndRegenWhileBelow - RestoreHealth"
                            });
                        }
                    })
                }
            };
        }
        else if(effect_name == "GainDamageReductionBasedOnMissingHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM});
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                        stat.Owner == Player.Instance && stat is Health
                    ), 
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "GainDamageReductionBasedOnMissingHealth" + special_identifier);
                        if(drBuff == null) {
                            drBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, effect.SourceOfEffect) {
                                PercentageModifier = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 
                                Identifier="GainDamageReductionBasedOnMissingHealth" + special_identifier, 
                                IsRemovable=false, 
                                ShowsInUI=true, 
                                EffectIndicatorText=Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%"
                            };
                            Player.Instance.AddEffect(drBuff);
                        }
                        else {
                            drBuff.PercentageModifier = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                            drBuff.EffectIndicatorText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%";
                        }
                    })
                }
            };
        }
        else if(effect_name == "ResistDeathAndGainDamageAgainstAttacker") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.RESIST_FATAL_BLOW_AND_HEAL_TO_PERCENTAGE_HEALTH_PER_PB, new List<float> {PB.RESTRICTION__300_SECOND_COOLDOWN});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__PLAYER_SUFFERING_A_FATAL_BLOW, PB.RESTRICTION__ONLY_AGAINST_ENEMY_WHO_LANDED_FATAL_BLOW});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB1,
                    SecondParameter = calculatedPB2,
                    ThirdParameter = 300,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB1), Utils.GetFormattedFloat(calculatedPB2)}, 
                    ConditionCheckOnAboutToHandleFatalBlow = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        Player.Instance.CheckIfEffectIsOnCooldown("ResistDeathAndGainDamageAgainstAttacker" + special_identifier)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), effect.ThirdParameter, Player.Instance, "ResistDeathAndGainDamageAgainstAttacker" + special_identifier));
                        damage.WillBeFatalBlow = false;
                        Player.Instance.Health.Current = Player.Instance.Health.Maximum * effect.FirstParameter / 100;
                        Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(effect_name)) {
                            DamagePercentageChange = calculatedPB2,
                            ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage2, effect2) =>
                                damage2.SourceOfDamage.User == Player.Instance && damage2.TargetOfDamage == damage.SourceOfDamage.User
                            ),
                        });
                    })
                }
            };
        }
        else if(effect_name == "HeavyDamageAppliesBurn") {
            float calculatedPB = CalculatePB(power_budget, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.BURN_PER_PB, PB.SPECIALIZATION__WEAPON_TYPE, PB.REQUIREMENT__DEALING_DAMAGE, PB.RESTRICTION__5_SECOND_COOLDOWN});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    RemainsActiveInOtherStances = true, 
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "5"}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Heavy && Player.Instance.CheckIfEffectIsOnCooldown("HeavyDamageAppliesBurn" + special_identifier)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Burn(effect.FirstParameter, effect.SourceOfEffect));
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), 5, Player.Instance, "HeavyDamageAppliesBurn" + special_identifier));
                    })
                }
            };
        }
        else if(effect_name == "BlazingShadowWatchesYourBack") {
            float calculatedPB1 = CalculatePB(power_budget * 0.5f, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__GETTING_DAMAGED_FROM_BEHIND});
            float calculatedPB2 = CalculatePB(power_budget * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> {PB.BURN_PER_PB, PB.REQUIREMENT__GETTING_DAMAGED_FROM_BEHIND});
            return new List<Effect> {
                new Effect_BlazingShadowWatchesYourBack(new(effect_name)) {
                    ExtraDamageReductionAgainstBackstabs = calculatedPB1,
                    BurnScalingInflictedToBackstabbers = calculatedPB2
                }
            };
        }
        else if(effect_name == "BurnAmountWhileBelowNHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.EFFECT_AMOUNT_INCREASE_PER_PB, new List<float> {PB.RESTRICTION__PLAYER_BELOW_50P_HEALTH});
            return new List<Effect> { 
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, calculatedPB, new(effect_name)){
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "50"}, 
                    ConditionForEffectPowerChange = new Func<Unit, bool>(target => 
                        Player.Instance.Health.Current < Player.Instance.Health.Maximum * 0.5f
                    )
                } 
            };
        }
        else if(effect_name == "BasicAttacksRestoreHealBasedOnMissingHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> {PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM, PB.REQUIREMENT__BASIC_ATTACK});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += effect.FirstParameter * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksDealMoreDamageBasedOnMissingHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM, PB.SPECIALIZATION__BASIC_ATTACK});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    DamagePercentageChange = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)
                    )
                }
            };
        }
        else if(effect_name == "GainDamageBasedOnMissingHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM});
            return new List<Effect> { 
                new Effect_CustomizableEffectOnEvent(new(effect_name)) { 
                    PercentageAmount = calculatedPB, 
                    DescriptionParameters = new List<string> {Utils.GetFormattedFloat(calculatedPB)},
                    ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                        stat.Owner == Player.Instance && stat is Health
                    ), 
                    ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                        Effect_ChangeCompositeStat damageBuff = (Effect_ChangeCompositeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "GainDamageBasedOnMissingHealth" + special_identifier);
                        if(damageBuff == null) {
                            damageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, effect.SourceOfEffect) {
                                PercentageModifier = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), 
                                Identifier="GainDamageBasedOnMissingHealth" + special_identifier, 
                                IsRemovable=false, 
                                ShowsInUI=true, 
                                EffectIndicatorText=Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%"
                            };
                            Player.Instance.AddEffect(damageBuff);
                        }
                        else {
                            damageBuff.PercentageModifier = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                            damageBuff.EffectIndicatorText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%";
                        }
                    })
                }
            };
        }


        //Knight
        else if(effect_name == "GainTenacityAfterBeingHit") {
            float calculatedPB = CalculatePB(power_budget, PB.TENACITY_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__PLAYER_WAS_DAMAGED_IN_LAST_10_SECONDS});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"}, 
                    TriggersOncePerAbility = true, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Tenacity, effect.SourceOfEffect) {
                            PercentageModifier=effect.FirstParameter, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="GainTenacityAfterBeingHit" + special_identifier
                        }, 10);
                    })
                }
            };
        }
        else if(effect_name == "GainDamageReductionAfterBeingHit") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__PLAYER_WAS_DAMAGED_IN_LAST_10_SECONDS});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB, 
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"}, 
                    TriggersOncePerAbility = true, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                        damage.TargetOfDamage == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, effect.SourceOfEffect) {
                            PercentageModifier=effect.FirstParameter, 
                            BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                            Identifier="GainDamageReductionAfterBeingHit" + special_identifier
                        }, 10);
                    })
                }
            };
        }
        else if(effect_name == "BlockXAmountOfDamageOnceEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.BLOCK_DAMAGE_PER_PB, PB.RESTRICTION__10_SECOND_COOLDOWN});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    SecondParameter = 10,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "10"},
                    ShowsInUI = true,
                    HideInUIWhileCooldownWithIdExists = "BlockXAmountOfDamageOnceEveryNSeconds" + special_identifier,
                    PathToEffectGraphic = "UI/Barrier",
                    EffectIndicatorText =  Utils.GetFormattedFloat(calculatedPB, 0),
                    ConditionCheckAfterHitDamageCalculation = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && !Player.Instance.CheckIfEffectIsOnCooldown("BlockXAmountOfDamageOnceEveryNSeconds" + special_identifier)
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.Injury -= effect.FirstParameter;
                        damage.Stagger -= effect.FirstParameter;
                        Player.Instance.AddCooldown(new Cooldown(typeof(Effect), effect.SecondParameter, Player.Instance, "BlockXAmountOfDamageOnceEveryNSeconds" + special_identifier));
                    })
                }
            };
        }
        else if(effect_name == "BlockCrowdControlOnceEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BurnAmountOnPlayer") {
            float calculatedPB = CalculatePB(power_budget, PB.DECREASE_STACKING_EFFECT_RECEIVED_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name))};
        }
        else if(effect_name == "FreezeAmountOnPlayer") {
            float calculatedPB = CalculatePB(power_budget, PB.DECREASE_STACKING_EFFECT_RECEIVED_PER_PB);
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -calculatedPB, new(effect_name))};
        }
        else if(effect_name == "PlundererEmpower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FAMILY_TECHNIQUES});
            return new List<Effect> {
                new Effect_Plunderer(calculatedPB, new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "20", "60"}
                } 
            };
        }
        else if(effect_name == "PlundererDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__WEAPON_TECHNIQUE, PB.SPECIALIZATION__FAMILY_TECHNIQUES, PB.RESTRICTION__LASTS_15_SECONDS});
            return new List<Effect> {
                new Effect_Description(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "15"},
                    Identifier = "PlundererDamageReduction",
                    FirstParameter = calculatedPB,
                    SecondParameter = 15
                } 
            };
        }
        else if(effect_name == "ConvertDamageReductionToHeavyStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {PB.REQUIREMENT__WEAPON_TECHNIQUE, PB.SPECIALIZATION__FAMILY_TECHNIQUES, PB.RESTRICTION__LASTS_15_SECONDS});
            return new List<Effect> {
                new Effect_Description(new(effect_name)) {
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB), "15"},
                    Identifier = "PlundererDamageReduction",
                    FirstParameter = calculatedPB,
                    SecondParameter = 15
                } 
            };
        }


        //Judge
        else if(effect_name == "WhenEnemyExitsCrowdControlApplyExtraFrozenInPlace") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "PushAwayAndFreezeUponFallingBelowHalfHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DamageReductionWhileAbove50PHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_REDUCTION_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DamagingAnEnemyFreezesThemInPlace") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IgnorePortionOfEnemyTenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksPushBackAndDecreaseEnemyTenacity") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ApplySleepToEnemiesHit") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksStunEnemies") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Gunslinger
        else if(effect_name == "DealExtraDamageToEnemiesFarAway") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "WhileInRangedStanceDecreaseDamageReductionButIncreaseDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "RestoreXAmmoEachTimeYouDealDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainMovementSpeedWhenThereIsAnEnemyNearYou") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealMoreDamageAndKnockbackToCloserEnemies") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "RefundAmmoIfEnemyHitByBasicAttackWasClose") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealIncreasedDamageBasedOnFlightTime") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "FinalAmmoDealsMassivelyIncreasedDamageEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }

        //Arbiter
        else if(effect_name == "ConvertAllInjuryToStaggerAgainstNonStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealingDamageProlongsStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StaggeringAnEnemyHealsOnceEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "EnemiesRegainStaggerBarXPercentSlower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealMoreStaggerAndKnockbackTheCloserEnemiesHitAreEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StaggerEnemiesOnceEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StaggeringAppliesXLethargySlowAndProne") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealsIncreasedStaggerOnceEveryNSecondsAndRefundCooldownOnHittingStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnSwitchingToWeaponSpawnAMarkerThatDealsMAssiveStaggerAndStunOnHit") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }

        //IronBlooded
        else if(effect_name == "ReflectInjuryTakenBackAtAttacker") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "RegeneratePortionOfInjuryTakenAsHealthOver30Seconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DamageReductionIsPartiallyEffectiveWhileStaggered") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertXPercentOfStaggerDealtToYouIntoInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DamageReductionIsMoreEffectiveAgainstInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "AfterGettingHitIncreaseDamageOfNextAttack") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }

        //Unbreakable
        else if(effect_name == "ReflectPortionOfBlockedDamageBackAtAttacker") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnceEveryNSecondsWhenAboutToBeStaggeredRestoreStaggerBarInstead") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DamageReductionWhileBlocking") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertXPercentOfInjuryDealtToYouIntoStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "HeavyDamageStealPortionOfEnemyStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksRestorePercentageOfStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Mercenary
        else if(effect_name == "GainDamageReductionBasedOnBarrierAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainPortionOfDamageTakenAsBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StrongBasicAttacksGrantBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnceEveryNSecondsPressingBlockGivesBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnceEveryNSecondsPressingBlockGivesInvincible") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Survivor
        else if(effect_name == "HealthRestorationPower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainHealthRegenerationThatIsDoubledWhenBelowHalfHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealMoreDamageWhileAtFullHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainDamageReductionWhileAtFullHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainInvincibleUponFallingBelow25PHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "HealOnBasicAttacks") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IncisionRestoresHealthInsteadOfDealingInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }

        //Assassin
        else if(effect_name == "ReducedBackstabCooldown") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IncreasedBackstabDamageWhileStealthed") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StealthDuration") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "MovementSpeedDuringStealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "EnterStealthUponFallingBelow25PHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "HeavyDamageAppliesMassiveInjuryAndEvenMoreIfBackstab") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksPerformedWhileInStealthCountAsBackstab") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "TakedownsGiveStealthAndEmpowerNextAttack") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }

        //Executioner
        else if(effect_name == "ConvertStaggerDealtIntoInjury") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "Every5thHitDealsMassivelyIncreasedDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealIncreasedDamageToEnemiesBelow25PHealth") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "TakedownsGrantEmpowered") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "HealOnTakedowns") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ReduceCooldownsOnTakedowns") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "FinishOffLowHealthEnemies") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "TakedownsRestoreAmmo") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnSwitchingToThisWeaponSpawnMarkerThatDealsInjuryOnHit") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Artisan
        else if(effect_name == "UsingToolsIncreasesToolPowerUntilEndOfCombat") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StancePower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "PassivePowerUpPower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ToolPower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ItemPower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "Gain2ExtraChargesOfHealthPotionAndIncreaseEffectivness") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "Gain1ExtraUseOfToolsAndIncreaseToolPower") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Alacrity
        else if(effect_name == "AfterDodgingEmpowerNextAttack") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IncreaseInvincibilityTimeOfDodge") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertAttackSpeedToDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertMovementSpeedToDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertMovementSpeedToDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertAttackSpeedToDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainSuperchargeBasedOnDistanceTravelled") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DodgingDamageAppliesProne") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DodgingDamageGivesAlacrity") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainAlacrityBasedOnDistanceTravelledInLast5Seconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainAlacrityOnLightDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ApplyLethargyOnLightDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Enforcer
        else if(effect_name == "BasicAttacksReduceCooldowns") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "WhileAtFullEnergyBasicAttacksDealMoreDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksGiveOnslaught") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "SharpAndAnalysisAlsoIncreaseBasicAttackDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "NonStopBasicAttackingIncreasesDamageDealtAndSpeed") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ReduceDamageByAFlatAmountWhileBasicAttacking") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "NonStrongBasicAttacksEmpowerYourNextStrongBasicAttack") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Sage
        else if(effect_name == "UsingTechniquesDecreasesRemainingCooldownOfAllOtherTechniques") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IncreaseMagicDamageButDecreaseWeaponDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IncreaseEnergyGainWhileEnergyIsBelowHalf") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealingMagicTechniqueDamageGivesBarrier") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //RoyalGuard
        else if(effect_name == "BasicAttacksGiveAnalysis") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "TechniqueDamageGivesAnalysis") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainDamageReductionBasedOnAnalysisAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "AnalysisAlsoIncreasesWeaponDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "IncreaseWeaponDamageByPortionOfMagicDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksDealIncreasedDamageBasedOnMagicDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //ShadowGifted
        else if(effect_name == "DealMoreDamageBasedOnPositiveStackingEffectsOnYouAndNegativeOnEnemy") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "WhenAttackingAnEnemyWithAllSignatureEffectsAppliedDealMassiveDamageBasedOnAmountAndRemoveAllOfThem") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "AllStackingEffectsAmount") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "AllStackingEffectsDecay") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "GainDamageReductionBasedOnPositiveStackingEffectsOnYouAndNegativeOnEnemy") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnHitApplyRandomStackingEffectYouDoNotCurrentlyPossess") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "LightDamageConvertsAllFreezeIntoBurnOrBurnIntoFreeze") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "RangedDamageAppliesBurnOrFreezeToEqualize") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }


        //Unique
        else if(effect_name == "StrongBasicAttacksReducePercentageOfEnemyStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "StrongBasicAttacksApplyIncisionBasedOnEnemyMaximumHealthEveryNSeconds") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ReplaceAllBasicAttacksWithChargeAndImproveDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ReplaceAllBasicAttacksWithThrowAddPullAndIncreaseStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ReplaceAllBasicAttacksWithThrowAndIncreaseDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksApplyIncision") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksApplyPoison") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksReducePercentageOfEnemyStaggerBar") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealMoreInjuryOrStaggerAndCanSwitchUsingBlock") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "CanBasicAttackNonStopAndDealMoreDamage") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "EbonySpecialScaling") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "BasicAttacksDealIncreasedDamageButAlsoInflictSelfStagger") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "InjuryAndStaggerScaleWithEachOther") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "ConvertAllInjuryIntoStaggerOrViceVersaDependingOnWhichIsLowerForEnemy") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "LightDamageTemporarilyLowersEnemyDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "LightDamageIgnoresPercentageOfEnemyDamageReduction") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "DealExtraDamageToUndamagedEnemies") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "TripleArrowsShotWithoutSpendingExtraAmmo") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }
        else if(effect_name == "OnSwitchingToThisWeaponGainRangedAttackSpeedAndNextBasicAttackStunsEnemy") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {});
            return new List<Effect> { 
                new Effect_CustomizableDamageChange(new(effect_name)) {
                    FirstParameter = calculatedPB,
                    DescriptionParameters = new List<String> {Utils.GetFormattedFloat(calculatedPB)}, 
                    ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    })
                }
            };
        }









        else if(effect_name == "IgnisManor_WeaponTraining") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                }
            };
        }
        else if(effect_name == "IgnisManor_BackstabPowerUp") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    FirstParameter = calculatedPB, 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab) && damage.SourceOfDamage?.User == Player.Instance && damage.IsDamageOverTime == false
                    ),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.TargetOfDamage.AddEffect(new Effect_Burn(Player.Instance.HeavyStagger.Current * 0.05f, new(damage.SourceOfDamage)));
                    })
                }
            };
        }
        else if(effect_name == "IgnisManor_BurningPowerUp") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 10, new(effect_name)),
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    MultiplierChange = -0.05f, 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))
                    )
                }
            };
        }
        else if(effect_name == "IgnisVolcano_BuriedEnergy") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 10, new(effect_name)),
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -10, new(effect_name))
            };
        }
        else if(effect_name == "IgnisVolcano_FireRiver") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_CustomizableDamageChange(new(effect_name)) { 
                    MultiplierChange = -0.1f, 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        damage.TargetOfDamage == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.Burn)
                    )
                }
            };
        }
        else if(effect_name == "IgnisManorOnFire_BurningSword") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                }
            };
        }
        else if(effect_name == "IgnisManorOnFire_MuseumSword") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 15, new(effect_name))
            };
        }
        else if(effect_name == "IgnisManorOnFire_MuseumArmour") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.Health, new(effect_name)) {
                    BaseModifier = -calculatedPB 
                }
            };
        }
        else if(effect_name == "AnimaIsland_3MajorDuels") {
            float calculatedPB = CalculatePB(power_budget, PB.DAMAGE_INCREASE_PER_PB, new List<float> {PB.SPECIALIZATION__FINAL_AMMO});
            return new List<Effect> {
                new Effect_ChangeStat(Player.Instance.HeavyInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB
                },
                new Effect_ChangeStat(Player.Instance.HeavyStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.LightInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.LightStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.RangedInjury, new(effect_name)) {
                    PercentageModifier = calculatedPB 
                },
                new Effect_ChangeStat(Player.Instance.RangedStagger, new(effect_name)) {
                    PercentageModifier = calculatedPB 
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
