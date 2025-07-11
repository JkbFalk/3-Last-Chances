using UnityEngine;

public class BA_Bow_F : BasicAttack {
    public float ChargeAmount;
    public bool StartedCharging = false;
    public bool FastShot = false;
    public bool ShotArrow = false;
    public static int AmmoRequiredToUseAbility = 1;
    private readonly float _jumpDistance = 0.9f;
    private int _flipped;
    private float _movementSpeed;
    private bool _barrageEquipped = false;
    private bool _canRecast = false;
    public int BarrageComboNumber = 1;

    public BA_Bow_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Ranged));
        _barrageEquipped = SaveFile.Instance.EquippedRangedWeapon is Bow_Barrage;
        if(_barrageEquipped) {
            NameOfAnimationToAutoPlay = "BA_Bow_Barrage";
        }
    }

    public override void OnAbilityStart()
    {
        Utils.CopyItemAppearanceForPlayer(Constants.ItemType.Tool, "Projectile_BowBasicAttack");
        base.OnAbilityStart();
    }

    public override void CallAbilityEvent1()
    {
        if(!_barrageEquipped) {
            Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Draw" + Utils.GetRandomSoundNumber("Bow_Draw"), 0.6f);
        }
    }

    public override void CallAbilityEvent2()
    {
        if(FastShot || _barrageEquipped) {
            if(!_barrageEquipped) {
                User.PlayAnimation("Bow_FF", 0, 0);
                Player.Instance.AddEffect(new Effect_Backstep(new(this)), 0.5f);
                Player.Instance.ApplyForce(new Vector2(-0.8f * _flipped, 0.8f) * 3 * _jumpDistance * _movementSpeed, this);
                GameController.Instance.WaitAndRunMethod(0.015f, Angle2);
                GameController.Instance.WaitAndRunMethod(0.03f, Angle3);
                GameController.Instance.WaitAndRunMethod(0.045f, Angle4);
                GameController.Instance.WaitAndRunMethod(0.06f, Angle5);
            }
            _canRecast = true;
            Projectile proj = Utils.CreateProjectile(new(this), "BowBasicAttack");
            proj.FlightSpeed = 10;
            DamageSources.Add(new DamageSource(300, 300, Constants.DamageType.Ranged));
            _flipped = User.Actions.IsFlipped ? -1 : 1;
            _movementSpeed = 1 + User.MovementSpeed.Current / 100;
        }
        else {
            Properties.Add(Property.StrongBasicAttack);
            StartedCharging = true;
            User.Actions.SetFaceVariant("Eyes Squinted");
        }
    }

    public void Angle2() {
        Player.Instance.ApplyForce(new Vector2(-0.7f * _flipped, 0.5f) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void Angle3() {
        Player.Instance.ApplyForce(new Vector2(-1f * _flipped, 0) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void Angle4() {
        Player.Instance.ApplyForce(new Vector2(-0.7f * _flipped, -0.8f) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void Angle5() {
        Player.Instance.ApplyForce(new Vector2(-0.6f * _flipped, -1f) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public override void CallAbilityEvent3()
    {
        ShotArrow = true;
        User.ApplyForce(User.Actions.IsFlipped ? Vector2.right * PlayerControls.BasicAttackButtonHoldDuration * 2 : Vector2.left * PlayerControls.BasicAttackButtonHoldDuration * 2 , this);
        GameObject vfx = Utils.CreateVisualEffect(new(this), "Friction");
        vfx.transform.SetParent(User.SpriteRenderers["Right Foot"].Bone.parent.transform);
        vfx.transform.position = vfx.transform.parent.position;
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.eulerAngles = Vector3.zero;
        vfx.transform.localScale = new Vector2(PlayerControls.BasicAttackButtonHoldDuration / 5, PlayerControls.BasicAttackButtonHoldDuration / 5);
        Projectile proj = Utils.CreateProjectile(new(this), "BowBasicAttack");
        proj.FlightSpeed = 10 + PlayerControls.BasicAttackButtonHoldDuration * 2.5f;
        DamageSources.Clear();
        DamageSources.Add(new DamageSource(300 * (PlayerControls.BasicAttackButtonHoldDuration / 2), 300 * (PlayerControls.BasicAttackButtonHoldDuration / 2), Constants.DamageType.Ranged));
    }

    public override void OnBasicAttackButtonPress()
    {
        if(_barrageEquipped && Player.Instance.Ammo > 0 && _canRecast) {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Bow_F(User);
            ((BA_Bow_F)User.Actions.CurrentAbilityBeingPerformed).BarrageComboNumber = BarrageComboNumber + 1;
        }
    }

    public override void OnBasicAttackButtonRelease()
    {
        if(StartedCharging && !ShotArrow && !_barrageEquipped) {
            User.PlayAnimation("Bow_F", 0.05f, 0.7f);
        }
        else if(FastShot == false){
            FastShot = true;
        }
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, _barrageEquipped ? "Bow/Bow_Release11" : "Bow/Bow_Release" + Utils.GetRandomSoundNumber("Bow_Release"), 0.6f);
    }
}