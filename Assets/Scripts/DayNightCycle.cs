using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f; // 시간을 0~1로 잡을 것이기 때문에 타임이 0.5일 때 정오. 각도가 90도 
    private float timeRate;
    public Vector3 noon; // Vector 90 0 0

    [Header("Sun")]
    public Light Sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light Moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    private void Start()
    {
        timeRate = 1f / fullDayLength; 
        time = startTime;
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1f; // time을 0~1로 지키기 위함
        
        UpdateLighting(Sun, sunColor, sunIntensity);
        UpdateLighting(Moon, moonColor, moonIntensity);

        // 씬 전체의 환경광 밝기 조절 (0으로 하면 씬 전체가 어두워짐)
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        // 씬 전체의 반사광 강도 조절
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // time 시간에 해당하는 값을 보간하여 반환

        // 360도의 0.5면 180도가 나옴. time의 0.5는 90도여야 함. 그래서 0.25를 빼주는 것.
        lightSource.transform.eulerAngles = (time - (lightSource == Sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy) // 밝기가 완전히 없어지고 하이어라키창에 켜져있으면 꺼주기
        {
            go.SetActive(false);
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
