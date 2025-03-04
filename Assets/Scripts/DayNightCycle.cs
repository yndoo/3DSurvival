using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f; // �ð��� 0~1�� ���� ���̱� ������ Ÿ���� 0.5�� �� ����. ������ 90�� 
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
        time = (time + timeRate * Time.deltaTime) % 1f; // time�� 0~1�� ��Ű�� ����
        
        UpdateLighting(Sun, sunColor, sunIntensity);
        UpdateLighting(Moon, moonColor, moonIntensity);

        // �� ��ü�� ȯ�汤 ��� ���� (0���� �ϸ� �� ��ü�� ��ο���)
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        // �� ��ü�� �ݻ籤 ���� ����
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // time �ð��� �ش��ϴ� ���� �����Ͽ� ��ȯ

        // 360���� 0.5�� 180���� ����. time�� 0.5�� 90������ ��. �׷��� 0.25�� ���ִ� ��.
        lightSource.transform.eulerAngles = (time - (lightSource == Sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy) // ��Ⱑ ������ �������� ���̾��Űâ�� ���������� ���ֱ�
        {
            go.SetActive(false);
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
