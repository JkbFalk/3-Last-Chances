using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D.IK;

public class Tool_CrimsonFeather : Item
{
    public bool EnemyWasHit = false;
    private bool _soundPlayed = false;

    public static float[] CooldownPerGrade = new float[5] {60, 50, 40, 30, 20};
    public Tool_CrimsonFeather(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Tool;
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description"), new object[] { Utils.GetFormattedFloat(5 * GetMultiplierForGrade())}) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n[CD] " + CooldownPerGrade[GradeIndex].ToString();
    }

    public override void ExtraBehaviourOnEquip() {
        EventManager.UnitWouldBeDefeated.AddListener(Activate);
    }

    public override void ExtraBehaviourOnUnequip() {
        EventManager.UnitWouldBeDefeated.RemoveListener(Activate);
    }

    public void Activate(Damage damage) {
        if(damage.TargetOfDamage is Player && damage.OverkillInjury > 0 && Player.Instance.ToolCooldown == null && SaveFile.Instance.ToolRemainingAmounts[typeof(Tool_CrimsonFeather)] > 0) {
            Player.Instance.Health.Current += 5 * GetMultiplierForGrade();
            Player.Instance.AddEffect(new Effect_Stun(new(this)), 1);
            Player.Instance.AddEffect(new Effect_Invincible(new(this)), 2);
            Player.Instance.PlayAnimation("CrimsonFeather", 0.05f);
            Player.Instance.AddCooldown(this);
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/CrimsonFeather1", 1);
            GameController.Instance.WaitAndRunMethod(0.3f, PlayRessSound);
            SaveFile.Instance.ToolRemainingAmounts[typeof(Tool_CrimsonFeather)]--;
        }
    }

    public void PlayRessSound() {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/CrimsonFeather2", 1);
        Utils.CreateVisualEffect(new(this), "CrimsonFeather", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
    }
}
