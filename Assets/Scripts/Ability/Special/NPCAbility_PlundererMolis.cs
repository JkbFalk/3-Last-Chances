using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_PlundererMolis : Ability {
    public static AbilityFamily Family = AbilityFamily.Molis;
    private List<AreaOfEffect> _aoes = new();
    public NPCAbility_PlundererMolis(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 500, Constants.DamageType.Heavy) {Knockback = 1200});
        AddCustomSound("Explosion", "Explosion/Explosion2", 0.9f);
        Properties.Add(AbilityProperty.ImmuneToFlinch);
        TransitionIntoAnimationDuration = 0;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
        TransitionIntoAnimationDuration = 0;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(User.SpriteRenderers["Heavy"]?.Bone?.transform?.Find("Mask")?.gameObject != null) {
            User.SpriteRenderers["Heavy"].Bone.transform.Find("Mask").gameObject.SetActive(false);
        }
    }

    public override void CallAbilityEvent1() {
        List<Vector2> positions = new() {new Vector2(1.5f, 0.5f), new Vector2(2, -1),new Vector2(1.5f, -2.5f),new Vector2(-1.5f, 0.5f),new Vector2(-2, -1),new Vector2(-1.5f, -2.5f)};
        for(int i = 0; i < 6; i++) {
            _aoes.Add(Utils.CreateAreaOfEffect(new(this), "LargeRockSpike" + (i > 2 ? " Flipped" : ""), User.transform.position.x + positions[i].x, User.transform.position.y + positions[i].y));
        }
    }
}