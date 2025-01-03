using UnityEngine;

public class ChangeTransformOverTime : MonoBehaviour {
    private int _scalecounter = 0;
    private int _Xcounter = 0;
    private int _Ycounter = 0;
    private int _alphaCounter = 0;
    private float _scaleChangePerSecond = 0;
    private float _xChangePerSecond = 0;
    private float _yChangePerSecond = 0;
    private float _alphaChangePerSecond = 0;

    public float ScaleTime = 0;
    public float StartScale = 0;
    public float EndScale = 0;

    public float XTime = 0;
    public float StartX = 0;
    public float EndX = 0;

    public float YTime = 0;
    public float StartY = 0;
    public float EndY = 0;

    public float AlphaTime = 0;
    public float StartAlpha = 0;
    public float EndAlpha = 0;

    private SpriteRenderer _spriteRenderer;

    public bool ScaleTimeWithPlayerAttackSpeed = false;

    private void Start() {
        if(ScaleTimeWithPlayerAttackSpeed)
        {
            ScaleTime /= Player.Instance.Actions.CurrentAbilityBeingPerformed.AttackSpeed.Current;
            XTime /= Player.Instance.Actions.CurrentAbilityBeingPerformed.AttackSpeed.Current;
            YTime /= Player.Instance.Actions.CurrentAbilityBeingPerformed.AttackSpeed.Current;
            AlphaTime /= Player.Instance.Actions.CurrentAbilityBeingPerformed.AttackSpeed.Current;
        }
        if (ScaleTime != 0) {
            transform.localScale = new Vector3(StartScale, StartScale, 1);
            _scaleChangePerSecond = (EndScale - StartScale) / ScaleTime;
        }
        if (XTime != 0) {
            transform.localPosition = new Vector3(StartX, transform.localPosition.y, 0);
            _xChangePerSecond = (EndX - StartX) / XTime;
        }
        if (YTime != 0) {
            transform.localPosition = new Vector3(transform.localPosition.x, StartY, 0);
            _yChangePerSecond = (EndY - StartY) / YTime;
        }
        if (AlphaTime != 0) {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, StartAlpha);
            _alphaChangePerSecond = (EndAlpha - StartAlpha) / AlphaTime;
        }
    }

    private void FixedUpdate() {
        if (_scalecounter < ScaleTime * 50) {
            _scalecounter++;
            transform.localScale = new Vector3(transform.localScale.x + _scaleChangePerSecond / 50, transform.localScale.y + _scaleChangePerSecond / 50, 1);
        }
        if (_Xcounter < XTime * 50) {
            _Xcounter++;
            transform.localPosition = new Vector3(transform.localPosition.x + _xChangePerSecond / 50, transform.localPosition.y, 0);
        }
        if (_Ycounter < YTime * 50) {
            _Ycounter++;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + _yChangePerSecond / 50, 0);
        }
        if (_alphaCounter < AlphaTime * 50) {
            _alphaCounter++;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _spriteRenderer.color.a + _alphaChangePerSecond / 50);
        }
    }

    public void SetScaleChangeOverTime(float time, float start_scale, float end_scale) {
        ScaleTime = time;
        StartScale = start_scale;
        EndScale = end_scale;
        _scalecounter = 0;
        transform.localScale = new Vector3(StartScale, StartScale, 1);
        _scaleChangePerSecond = (EndScale - StartScale) / ScaleTime;
    }

    public void SetPositionXChangeOverTime(float time, float start_x, float end_x) {
        XTime = time;
        StartX = start_x;
        EndX = end_x;
        _Xcounter = 0;
        transform.localPosition = new Vector3(StartX, transform.localPosition.y, 0);
        _xChangePerSecond = (EndX - StartX) / XTime;
    }

    public void SetPositionYChangeOverTime(float time, float start_y, float end_y) {
        YTime = time;
        StartY = start_y;
        EndY = end_y;
        _Ycounter = 0;
        transform.localPosition = new Vector3(transform.localPosition.x, StartY, 0);
        _yChangePerSecond = (EndY - StartY) / YTime;
    }

    public void SetAlphaChangeOverTime(float time, float start_alpha, float end_alpha) {
        AlphaTime = time;
        StartAlpha = start_alpha;
        EndAlpha = end_alpha;
        _alphaCounter = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, StartAlpha);
        _alphaChangePerSecond = (EndAlpha - StartAlpha) / AlphaTime;
    }
}