
using UnityEngine;
using UnityEngine.UI;
public class Effect_WindRushReactionWindow : Effect
{
    public static readonly Color FreeBorderColor = new Color(0.12f, 0.85f, 0.35f, 1f);

    public Effect_WindRushReactionWindow(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Id = Ability_WindRush.REACTION_WINDOW_EFFECT_ID;
        ShowsInUI = true;
        PathToUIGraphic = "Ability/WindRush";
        UIText = Label.Get("FreeCostIndicator");
        Type = EffectType.Buff;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }

    public override void OnStart()
    {
        base.OnStart();
        ApplySlotVisuals(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        ApplySlotVisuals(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ApplySlotVisuals(false);
        Energy.MarkAbilitiesWithNotEnoughEnergy();
    }

    public static void ApplySlotVisuals(bool isFree)
    {
        if (Player.Instance?.CurrentStance?.Abilities == null) return;

        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities)
        {
            if (ability?.Type == typeof(Ability_WindRush))
            {
                if (isFree)
                {
                    if (ability.AbilityGraphic != null)
                    {
                        var stroke = ability.AbilityGraphic.transform.Find("Stroke")?.GetComponent<Image>();
                        if (stroke != null)
                        {
                            stroke.color = FreeBorderColor;
                        }
                    }
                    if (ability.CostLabel != null)
                    {
                        ability.CostLabel.text = "";
                    }
                }
                else
                {
                    ability.RefreshDisplayForEquippedAbility();
                }
            }
        }
    }
}