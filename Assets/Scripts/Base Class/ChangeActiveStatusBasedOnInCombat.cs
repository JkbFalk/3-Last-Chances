using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeActiveStatusBasedOnInCombat : MonoBehaviour
{
    public bool ChangeInteractableInsteadOfActive = false;
    public bool WhenInCombatChangeToActive = false;
    public bool WhenInCombatChangeToInactive = false;
    public bool WhenNotInCombatChangeToActive = false;
    public bool WhenNotInCombatChangeToInactive = false;
}
