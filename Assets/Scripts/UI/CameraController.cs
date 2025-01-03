using UnityEngine;

public class CameraController : MonoBehaviour {
    private float _duration;
    private float _magnitude;
    private float _damping;
    private Vector3 _defaultCameraPosition = new Vector3(0, 0, -100);
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
                _instance = Player.Instance.GetComponentInChildren<CameraController>();
                _instance.Camera = _instance.GetComponent<Camera>();
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
            transform.position += (CenteredOnObject.transform.position - transform.position) * 0.1f;
            transform.position = new Vector3(transform.position.x, transform.position.y, -100);
        }
        if (CenteredOnObject == null && _duration > 0) {
            transform.localPosition = new Vector3(Random.insideUnitCircle.x * _magnitude, Random.insideUnitCircle.y * _magnitude, -100);
            _duration -= Time.deltaTime * _damping;
        }
        else if (CenteredOnObject == null && transform.localPosition != _defaultCameraPosition) {
            transform.localPosition = _defaultCameraPosition;
        }
    }

    public void ShakeScreen(float duration = 0.1f, float magnitude = 0.05f, float damping = 1.0f) {
        _duration = duration;
        _magnitude = magnitude;
        _damping = damping;
    }
}