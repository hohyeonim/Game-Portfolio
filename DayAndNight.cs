using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField]
    private float secondPerRealTimeSecond; // 게임 세계의 100초는 현실 세계의 1초

    [SerializeField]
    private float fogDensityCalc; // 증감량 비율

    [SerializeField]
    private float nightFogDensity; // 밤 상태의 Fog 밀도
    private float dayFogDensity; // 낮 상태의 Fog 밀도
    private float currentFogDensity; // 계산

    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        // 시간에 따른 Sun Rotate 이동
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        // x값 각도에 따른 밤, 낮 구분
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

        // 밤, 낮 Fog 기능 사용(시야)
        if (GameManager.isNight)
        {
            // 밤
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            // 낮
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
