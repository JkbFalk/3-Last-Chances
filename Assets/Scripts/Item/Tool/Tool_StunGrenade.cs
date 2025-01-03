using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Tool_StunGrenade : Item
{
    public bool EnemyWasHit = false;
    private bool _soundPlayed = false;

    public static float Cooldown = 10;
    public static int[] StunDuration = new int[] {10, 12, 14, 16, 20};
    public static float[] MaxRange = new float[] {6.5f, 7.5f, 8.5f, 9.5f, 11};
    public Tool_StunGrenade(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Tool;
        OnUseAbility = typeof(Ability_ThrowItem);
        DamageSources = new List<Ability.DamageSource>()
        {
            new Ability.DamageSource(0.5f * GetMultiplierForGrade(), 0, Constants.DamageType.None, "StunGrenade"),
            new Ability.DamageSource(0, 2 * GetMultiplierForGrade(), Constants.DamageType.None, "AoE")
        };
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description" + (detailed ? "Detailed" : "Simple")), new object[] { Utils.GetFormattedFloat(3 * GetMultiplierForGrade()), Utils.GetFormattedFloat(10 * GetMultiplierForGrade()), Utils.GetFormattedFloat(20 * GetMultiplierForGrade()), 5, Utils.GetFormattedFloat(1 * GetMultiplierForGrade()) }) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n<sprite name=\"Cooldown\"> " + Cooldown.ToString();
    }

    public override void OnUse()
    {
        base.OnUse();
        _soundPlayed = false;
        EnemyWasHit = false;
        GameController.Instance.WaitAndRunMethod(1.5f, ExplodeIfNotHittingEnemy);
    }

    public override void OnEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        EnemyWasHit = true;
        PlaySound();
        AdjustSizeBasedOnGrade(object_hitting.gameObject.name.Contains("AoE") ? object_hitting.transform.parent.gameObject : object_hitting.transform.Find("OnHit/Explosion").gameObject);
        if (object_hitting.name.Contains("StunGrenade"))
        {
            foreach (Transform child in object_hitting.transform)
            {
                child.gameObject.SetActive(true);
            }
            GameController.Instance.WaitAndRunMethod(0.5f, DamageSecondPhase, new string[] {object_hitting.transform.position.x.ToString(), object_hitting.transform.position.y.ToString()});
        }
    }

    public void DamageSecondPhase(string[] position)
    {
        foreach (Unit enemy in ItemUseAbility.AffectedEnemies.Keys)
        {
            float distance = Vector2.Distance(enemy.transform.position, new Vector2(float.Parse(position[0]), float.Parse(position[1])));
            if(enemy.IsBoss) {
                enemy.AddEffect(new Effect_Flinching(new(this)), Constants.DEFAULT_FLINCHING_DURATION);
            }
            else {
                enemy.AddEffect(new Effect_Stun(new(this)), StunDuration[GradeIndex] * Utils.GetValueBasedOnMinAndMax(distance, 0,  MaxRange[GradeIndex], 1, 0.5f));
            }
        }
    }

    private void PlaySound() {
        if(_soundPlayed == false) {
            _soundPlayed = true;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/StunGrenade_Explosion", 0.7f);
        }
    }

    public void ExplodeIfNotHittingEnemy()
    {
        if (ToolObject != null && EnemyWasHit == false)
        {
            foreach (Transform child in ToolObject.transform)
            {
                child.gameObject.SetActive(true);
            }
            Transform on_hit_vfx = ToolObject.transform.Find("OnHit");
            if(on_hit_vfx != null && on_hit_vfx.gameObject.IsDestroyed() == false) {
                GameObject cloned_on_hit_vfx = MonoBehaviour.Instantiate(on_hit_vfx.gameObject);
                cloned_on_hit_vfx.gameObject.SetActive(true);
                cloned_on_hit_vfx.transform.position = ToolObject.transform.position;
                AdjustSizeBasedOnGrade(cloned_on_hit_vfx.transform.Find("Explosion").gameObject);
                foreach (DamagingObject obj in cloned_on_hit_vfx.GetComponentsInChildren<DamagingObject>(true))
                {
                    obj.SourceAbility = ItemUseAbility;
                }
            }  
            MonoBehaviour.Destroy(ToolObject.gameObject);
            GameController.Instance.WaitAndRunMethod(0.5f, DamageSecondPhase,  new string[] {ToolObject.transform.position.x.ToString(), ToolObject.transform.position.y.ToString()});
            PlaySound();
        }
    }

    public void AdjustSizeBasedOnGrade(GameObject game_object) {
        game_object.transform.Find("AoE").GetComponent<CircleCollider2D>().radius = MaxRange[GradeIndex];
        ParticleSystem.MainModule main = game_object.GetComponent<ParticleSystem>().main;
        main.startSize = 5 * MaxRange[GradeIndex];
        ParticleSystem.MainModule main2 = game_object.transform.Find("CenterGlowSparkExplosion").GetComponent<ParticleSystem>().main;
        main.startSize = 1.5f * MaxRange[GradeIndex];
    }
}
