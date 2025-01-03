using UnityEngine;

public class AreaOfEffect : DamagingObject {

    public bool DestroyAfterSourceAbilityEnds = false;

    public void Start() {
        Owner = SourceAbility?.User;
        if(DestroyAfterSourceAbilityEnds)
        {
            SourceAbility.ObjectsToDestroyOnceAbilityEnds.Add(this);
        }
    }

    public void StartDealingDamage(float damage_window_duration = 0)
    {
        DealingDamage = true;
        if(damage_window_duration > 0)
        {
            GameController.Instance.WaitAndRunMethod(damage_window_duration, TurnOffDealingDamage);
        }
    }

    public void TurnOffDealingDamage()
    {
        DealingDamage = false;
    }
}