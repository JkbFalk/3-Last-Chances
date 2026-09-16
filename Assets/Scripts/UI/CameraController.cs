using UnityEngine;

public class CameraController : MonoBehaviour {
    private float _duration;
    private float _magnitude;
    private float _damping;
    public bool CurrentlyScouting = false;
    public Camera Camera;

    public GameObject CenteredOnObject;

    private static CameraController _instance = null;

    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameController.Instance.GetComponentInChildren<CameraController>();
                _instance.Camera = _instance.GetComponent<Camera>();
                _instance.transform.eulerAngles = new Vector3(-Constants.WORLD_TILT_ANGLE, 0, 0);
            }
            return _instance;
        }
    }

    private void Update() {
        float expectedFoV = 2 + (Settings.Instance.FieldOfView <= 20 ? Settings.Instance.FieldOfView / 10 : 2 + (Settings.Instance.FieldOfView - 20) / 5);
        if(CurrentlyScouting && CameraController.Instance.Camera.orthographicSize < 12) {
            CameraController.Instance.Camera.orthographicSize += Constants.SCOUT_ZOOM_SPEED;
        }
        if(!CurrentlyScouting && CameraController.Instance.Camera.orthographicSize > expectedFoV) {
            CameraController.Instance.Camera.orthographicSize = CameraController.Instance.Camera.orthographicSize - Constants.SCOUT_RETURN_SPEED < expectedFoV ? expectedFoV : CameraController.Instance.Camera.orthographicSize - Constants.SCOUT_RETURN_SPEED;
        }
        if(CenteredOnObject != null && Vector2.Distance(transform.position, CenteredOnObject.transform.position) > 0.1f) {
            Vector2 newPosition = transform.parent.position + (CenteredOnObject.transform.position - transform.parent.position) * 0.15f;
            transform.parent.position = new Vector3(newPosition.x, newPosition.y, -100);
        }
        else if(CenteredOnObject == null && Player.HasInstance() && GameController.Instance.GameplayMode != Constants.GameplayMode.OnStartScreen) {
            transform.parent.position = new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y, -100);
        }
        if (CenteredOnObject == null && _duration > 0) {
            transform.localPosition = new Vector3(Random.insideUnitCircle.x * _magnitude, Random.insideUnitCircle.y * _magnitude - Constants.CAMERA_DISTANCE_AWAY_FROM_PLAYER, 0);
            _duration -= Time.deltaTime * _damping;
        }
        else if (_duration <= 0) {
            transform.localPosition = new Vector3(0, -Constants.CAMERA_DISTANCE_AWAY_FROM_PLAYER, 0);
        }
    }

    public void ShakeScreen(float duration = 0.1f, float magnitude = 0.05f, float damping = 1.0f) {
        if(Settings.Instance.ScreenShake <= 0)
        {
            return;
        }
        _duration = duration * Settings.Instance.ScreenShake;
        _magnitude = magnitude * Settings.Instance.ScreenShake;
        _damping = damping * Settings.Instance.ScreenShake;
    }
}