using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField]
    private float secondPerRealTimeSecond; // ���� ������ 100�ʴ� ���� ������ 1��

    [SerializeField]
    private float fogDensityCalc; // ������ ����

    [SerializeField]
    private float nightFogDensity; // �� ������ Fog �е�
    private float dayFogDensity; // �� ������ Fog �е�
    private float currentFogDensity; // ���

    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        // �ð��� ���� Sun Rotate �̵�
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        // x�� ������ ���� ��, �� ����
        if (transform.eulerAngles.x >= 170)
        {
            if (transform.eulerAngles.x >= 340)
            {
                GameManager.isNight = false;
            }
            else
            {
                GameManager.isNight = true;
            }
        }
        else
        {
            GameManager.isNight = false;
        }    

        // ��, �� Fog ��� ���(�þ�)
        if (GameManager.isNight)
        {
            // ��
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            // ��
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
