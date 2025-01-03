using TMPro;

public class Stagger : Stat {
public Constants.DamageType Category;
    public Stagger(Constants.DamageType category, Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find(category.ToString() + "Stagger/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = stat_owner != null && stat_owner.ScaleStatsWithLevel ? base_amount * Utils.GetExpectedPowerForLevel(stat_owner.Level) : base_amount;
        Maximum = Base;
        Current = Base;
        Category = category;
    }

    public override void AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount()
    {
        base.AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
        if(Owner is not Player) {
            return;
        }
        float highest;
        Steamworks.SteamUserStats.GetStat("HIGHEST_DAMAGE_STAT", out highest);
        if(highest > Maximum) {
            return;
        }
        Steamworks.SteamUserStats.SetStat("HIGHEST_DAMAGE_STAT", Maximum);
        Steamworks.SteamUserStats.StoreStats();
        if(Maximum > 776.5f && Maximum < 777.5f) {
            return;
        }
        bool jackpot;
        Steamworks.SteamUserStats.GetAchievement("JACKPOT", out jackpot);
        if(jackpot == false) {
            Steamworks.SteamUserStats.SetAchievement("JACKPOT");
        }
    }

    public override string ToString()
    {
        return Category == Constants.DamageType.Heavy ? "HeavyStagger" : Category == Constants.DamageType.Light ? "LightStagger"  : Category == Constants.DamageType.Ranged ? "RangedStagger"  : Category == Constants.DamageType.Magic ? "MagicStagger" : "Stagger";
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat(Current) + " (" + Utils.GetFormattedFloat(Base) + ")";
    }
}