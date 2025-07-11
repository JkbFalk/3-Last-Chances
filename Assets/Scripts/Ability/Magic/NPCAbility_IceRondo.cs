using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NPCAbility_IceRondo : Ability {
    private GameObject _iceSpikes;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    public static float Cooldown = 6;
    public NPCAbility_IceRondo(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.25f;
        DamageSources.Add(new DamageSource(250, 50, Constants.DamageType.Magic));
        AddCustomSound("Use", "Ice/Ice_Use1", 0.2f);
        HitSoundType = Constants.HitSoundTypeEnum.SmallSharp;
        HitSoundVolume = 0.3f;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RootedInPlace(new(this)) };
        Properties.Add(Property.ImmuneToFlinch);
    }

    public override void CallAbilityEvent1()
    {
        _iceSpikes = Utils.CreateAreaOfEffect(new(this), "Criminal_FreezeCaster_IceSpikes").transform.parent.parent.parent.gameObject;
        int sortOrder = User.SpriteRenderers["Lower Body"].SpriteRenderer.sortingOrder;
        foreach (Transform child in _iceSpikes.transform)
        {
            child.GetComponent<Animator>().Play("Ice Spike");
            child.GetComponent<Animator>().speed = User.MagicAttackSpeed.ScaledWithCombatSpeed;
            child.GetComponent<SortingGroup>().sortingOrder = child.name == "In Front" ? sortOrder + 30 : sortOrder - 30;
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage) {
        damage.TargetOfDamage.AddEffect(new Effect_Freeze(150, new(this)));
    }
}