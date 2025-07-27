using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameShove : Ability {

    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Ignis;

    private AreaOfEffect _aoe;
    private int _shoveCycle = 1;
    private bool _userFlipped;

    public NPCAbility_FlameShove(Unit ability_user) : base(ability_user)
    {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.2f);
        DamageSources.Add(new DamageSource(30, 60, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
    }

    public void ResetTargets() {
        if(_aoe != null && _aoe.gameObject != null && _aoe.gameObject.IsDestroyed() == false) {
            ResetPotentialTargets();
        }
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlameShove");
        _userFlipped = User.Actions.IsFlipped;
        _aoe.transform.localScale = new Vector2(0.05f, 0.05f);
        GameController.Instance.WaitAndRunMethod(0.025f, AdvanceFlameShove);
        EventManager.OneTenthSecondElapsedInGame.AddListener(ResetTargets);
        GameController.Instance.WaitAndRunMethod(2, new System.Action(() => { EventManager.OneTenthSecondElapsedInGame.RemoveListener(ResetTargets); }));
    }

    public void AdvanceFlameShove() {
        _shoveCycle++;
        _aoe.transform.localScale = new Vector2(0.02f * _shoveCycle, 0.02f * _shoveCycle);
        _aoe.transform.position = new Vector2(_aoe.transform.position.x + (_userFlipped ? -0.1f : 0.1f), _aoe.transform.position.y);
        if(_shoveCycle < 40) {
            GameController.Instance.WaitAndRunMethod(0.025f, AdvanceFlameShove);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(2 * User.MagicStagger.Current / 100, new(this)));
    }
}