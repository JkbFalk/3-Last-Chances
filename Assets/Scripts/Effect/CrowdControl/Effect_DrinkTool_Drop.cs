using UnityEngine;

public class Effect_DrinkTool_Drop: Effect_HardCrowdControl
{

    public Effect_DrinkTool_Drop(SourceOfEffect source_of_effect) : base(source_of_effect) {
        ScaleWithControlAndTenacity = false;
    }
}