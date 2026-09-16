using Unity.VisualScripting;
using UnityEngine;

public class Projectile : DamagingObject {
    public Vector2 StartLocation;
    public Unit Target { get; set; }
    public float MaxFlightDistance = 3;
    public float FlightSpeed = 0.1f;
    public bool IsFlying = false;
    public int DisappearsAfterNHits = 0;
    public float SlowDownOverTime = 0;
    public Unit HomingOntoUnit;
    public bool IgnoresWalls = false;
    public bool DurabilityDoesNotDecreaseOnDestructibleHit = false;
    public bool OnlyDestroyOnTargetHit = false;
    public bool IsFinalAmmo = false;

    private void Start() {
        ApplySpawnState();
    }

    private void OnEnable() {
        ApplySpawnState();
    }

    private void ApplySpawnState() {
        if (SourceAbility != null) {
            Owner = SourceAbility.User;
        }
        StartLocation = transform.position;
    }

    public void Update() {
        if (IsFlying && HomingOntoUnit != null) {
            transform.Translate((HomingOntoUnit.transform.position - transform.position).normalized * FlightSpeed * Time.deltaTime, Space.World);
        }
        else if(IsFlying) {
            transform.Translate(Vector2.up * FlightSpeed * Time.deltaTime, Space.Self);
        }
        if (DealingDamage && !IsDisappearing && Vector2.Distance(transform.position, StartLocation) > MaxFlightDistance) {
            IsDisappearing = true;
            DealingDamage = false;
        }
        if (FlightSpeed > 0)
        {
            FlightSpeed -= Time.deltaTime * SlowDownOverTime;
            if (FlightSpeed < 0)
            {
                FlightSpeed = 0;
            }
        }
    }

    protected override void AdditionalActionsOnDestructibleHit() {
        if(DurabilityDoesNotDecreaseOnDestructibleHit) {
            return;
        }
        DisappearsAfterNHits--;
        if(DisappearsAfterNHits <= 0 && gameObject.IsDestroyed() == false && OnlyDestroyOnTargetHit == false) {
            MakeObjectDisappear(0);
        }
    }

    public void HandleWallHit() {
        if(IgnoresWalls) {
            return;
        }
        IsFlying = false;
        DealingDamage = false;
        FlightSpeed = 0;
        DisappearTimeInSeconds = 5;
        IsDisappearing = true;
        GameObject vfx = Utils.CreateVisualEffect(new(SourceAbility), "WallHit", transform.position.x, transform.position.y);
        vfx.transform.up = transform.up * -1;
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/WallHit" + UnityEngine.Random.Range(1, 4), 0.25f);
        Transform on_hit_vfx = transform.Find("OnHit");
        if(transform.Find("OnHit") != null) {
            on_hit_vfx.gameObject.SetActive(true);
            on_hit_vfx.SetParent(on_hit_vfx.parent.parent);
            on_hit_vfx.transform.position = transform.position;
        }
    }
}                