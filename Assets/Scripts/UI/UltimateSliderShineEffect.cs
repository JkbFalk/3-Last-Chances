// FILE: Assets\Scripts\UI\UltimateSliderShineEffect.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltimateSliderShineEffect : MonoBehaviour
{
    [Header("Visual Targets")]
    [SerializeField] private Image _shineGraphic;

    [Header("Wave Settings")]
    [SerializeField] private float _waveSpeed = 0.3f;
    [SerializeField] private float _waveScale = 0.5f; 
    [SerializeField] private float _baseBrightness = 1.8f; 
    [SerializeField] private float _readyBrightness = 3.0f; 
    
    // NEW: Multiplier to darken the base family colors (1.0 = normal, 0.5 = half as bright)
    [SerializeField] private float _colorDarkenMultiplier = 0.65f; 

    [Header("Pulsing Settings")]
    [SerializeField] private float _basePulseSpeed = 2.0f;
    [SerializeField] private float _readyPulseSpeed = 6.0f;
    [SerializeField] private float _pulseIntensity = 0.4f; 

    private int _lastUnlockedCount = -1;
    private Texture2D _waveTexture;

    private void Awake()
    {
        if (_shineGraphic == null)
        {
            _shineGraphic = GetComponent<Image>();
        }
        
        if (_shineGraphic != null)
        {
            _shineGraphic.color = Color.white;
        }
    }

    private void Update()
    {
        if (_shineGraphic == null || SaveFile.Instance == null) return;

        List<Ability.AbilityFamily> unlocked = SaveFile.Instance.UnlockedUltimateFamilies;
        int currentCount = unlocked != null ? unlocked.Count : 0;

        if (currentCount != _lastUnlockedCount)
        {
            RegenerateWaveTexture(unlocked);
            _lastUnlockedCount = currentCount;
        }

        bool isFull = Player.HasInstance() && Player.Instance.UltimateEnergy != null && Player.Instance.UltimateEnergy.IsFull;
        
        float currentBrightness = isFull ? _readyBrightness : _baseBrightness;
        float currentPulseSpeed = isFull ? _readyPulseSpeed : _basePulseSpeed;
        
        float pulseWave = (Mathf.Sin(Time.unscaledTime * currentPulseSpeed) + 1f) * 0.5f;
        float currentPulse = pulseWave * _pulseIntensity;

        Material mat = _shineGraphic.material;
        if (mat != null)
        {
            mat.SetFloat("_Brightness", currentBrightness);
            mat.SetFloat("_WaveSpeed", _waveSpeed);
            mat.SetFloat("_WaveScale", _waveScale);
            mat.SetFloat("_Pulse", currentPulse);
            
            if (_waveTexture != null && mat.GetTexture("_WaveTex") != _waveTexture)
            {
                mat.SetTexture("_WaveTex", _waveTexture);
            }
        }
    }

    private void RegenerateWaveTexture(List<Ability.AbilityFamily> unlocked)
    {
        if (_waveTexture != null) Destroy(_waveTexture);

        int width = 256;
        _waveTexture = new Texture2D(width, 1, TextureFormat.RGBA32, false);
        _waveTexture.wrapMode = TextureWrapMode.Repeat; 
        _waveTexture.filterMode = FilterMode.Bilinear;

        if (unlocked == null || unlocked.Count == 0)
        {
            Color darkGray = new Color(0.3f, 0.3f, 0.3f, 1f);
            for (int i = 0; i < width; i++) _waveTexture.SetPixel(i, 0, darkGray);
        }
        else if (unlocked.Count == 1)
        {
            Color singleColor = Colors.GetFamilyColor(unlocked[0].ToString());
            singleColor = new Color(singleColor.r * _colorDarkenMultiplier, singleColor.g * _colorDarkenMultiplier, singleColor.b * _colorDarkenMultiplier, 1f);
            for (int i = 0; i < width; i++) _waveTexture.SetPixel(i, 0, singleColor);
        }
        else
        {
            List<Color> colors = new List<Color>();
            foreach (var family in unlocked)
            {
                colors.Add(Colors.GetFamilyColor(family.ToString()));
            }

            for (int i = 0; i < width; i++)
            {
                float t = (float)i / width; 
                float scaledT = t * colors.Count;
                int indexA = Mathf.FloorToInt(scaledT) % colors.Count;
                int indexB = (indexA + 1) % colors.Count;
                float localT = scaledT - Mathf.Floor(scaledT);

                Color blendedColor = Color.Lerp(colors[indexA], colors[indexB], localT);
                
                // Darken the blended color
                blendedColor = new Color(blendedColor.r * _colorDarkenMultiplier, blendedColor.g * _colorDarkenMultiplier, blendedColor.b * _colorDarkenMultiplier, 1f);
                
                _waveTexture.SetPixel(i, 0, blendedColor);
            }
        }

        _waveTexture.Apply();
    }

    private void OnDestroy()
    {
        if (_waveTexture != null) Destroy(_waveTexture);
    }
}