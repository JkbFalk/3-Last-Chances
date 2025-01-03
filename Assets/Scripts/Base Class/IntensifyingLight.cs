using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IntensifyingLight : MonoBehaviour
{
    public float MinIntensity = 0.1f;
    public float MaxIntensity = 0.5f;
    public float MinInnerRadius = 0.1f;
    public float MaxInnerRadius = 0.5f;
    public float SecondsToFinishFullCycle = 1;
    private Light2D _light2D;
    private float _timeCounter = 0;
    void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        _timeCounter += Time.deltaTime;
        float progress = (_timeCounter / SecondsToFinishFullCycle) % 1;
        if(progress < 0.5f) {
            _light2D.intensity = MinIntensity + (MaxIntensity - MinIntensity) * progress * 2;
            _light2D.pointLightInnerRadius = MinInnerRadius + (MaxInnerRadius - MinInnerRadius) * progress * 2;
            _light2D.pointLightOuterRadius = (MinInnerRadius + (MaxInnerRadius - MinInnerRadius) * progress * 2) * 4;
        }
        else {
            _light2D.intensity = MaxIntensity - (MaxIntensity - MinIntensity) * (progress - 0.5f) * 2;
            _light2D.pointLightInnerRadius = MaxInnerRadius - (MaxInnerRadius - MinInnerRadius) * (progress - 0.5f) * 2;
            _light2D.pointLightOuterRadius = (MaxInnerRadius - (MaxInnerRadius - MinInnerRadius) * (progress - 0.5f) * 2) * 4;
        }
    }
}
