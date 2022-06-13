using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField]
    private float waterDrag; // 물속 중력
    private float originDrag;

    [SerializeField]
    private Color waterColor; // 물속 색깔
    [SerializeField]
    private float waterFogDensity; // 물 탁함 정도

    [SerializeField]
    private Color waterNightColor; // 밤 상태의 물속 색깔
    [SerializeField]
    private float waterNightFogDensity; // 밤 상태의 물속 탁함

    private Color originColor;
    private float originFogDensity;

    [SerializeField]
    private Color originNightColor;
    [SerializeField]
    private float originNightFogDensity;

    [SerializeField]
    private string sound_WaterOut;
    [SerializeField]
    private string sound_WaterIn;
    [SerializeField]
    private string sound_Breathe;

    [SerializeField]
    private float breatheTime;
    private float currentBreatheTime;

    [SerializeField]
    private float totalOxygen;
    private float currentOxygen;
    private float temp;

    [SerializeField]
    private GameObject go_BaseUi;
    [SerializeField]
    private Text text_totalOxygen;
    [SerializeField]
    private Text text_currentOxygen;
    [SerializeField]
    private Image image_gauge;

    private StatusController thePlayerStat;


    void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;

        originDrag = 0;
        thePlayerStat = FindObjectOfType<StatusController>();
        currentOxygen = totalOxygen;
        text_totalOxygen.text = totalOxygen.ToString();
    }

    void Update()
    {
        if (GameManager.isWater)
        {
            currentBreatheTime += Time.deltaTime;
            if (currentBreatheTime >= breatheTime)
            {
                SoundManager.instance.PlaySE(sound_Breathe);
                currentBreatheTime = 0;
            }

        }

        DecreaseOxygen();

    }

    // 산소 게이지 감소, 산소게이지가 다 줄어들면 데미지
    private void DecreaseOxygen()
    {
        if (GameManager.isWater)
        {
            currentOxygen -= Time.deltaTime; // 현재 산소 게이지를 매초마다 감소 시켜준다.
            text_currentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString(); // 현재 산소 게이지 text가 소수점으로 뜨지 않도록 RoundToInt를 해준다.
            image_gauge.fillAmount = currentOxygen / totalOxygen; // 산소 게이지 바를 감소 시켜 준다.

            if (currentOxygen <= 1)
            {
                temp += Time.deltaTime;
                
                if (temp >= 1)
                {
                    // 플레이어 데미지
                    thePlayerStat.DecreaseHP(1);
                    text_currentOxygen.text = "0";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetWater(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetOutWater(other);
        }
    }

    // 물 속 플레이어
    private void GetWater(Collider _player)
    {
        SoundManager.instance.PlaySE(sound_WaterIn);

        go_BaseUi.SetActive(true); // 산소 게이지 UI 활성화
        GameManager.isWater = true;
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag; // 플레이어 중력 변경(물속 중력)

        // 물 속에서 낮, 밤 물속 컬러 및 fog시야
        if (!GameManager.isNight)
        {
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFogDensity;
        }
        else
        {
            RenderSettings.fogColor = waterNightColor;
            RenderSettings.fogDensity = waterNightFogDensity;
        }

    }

    // 물 밖 플레이어
    private void GetOutWater(Collider _player)
    {
        if (GameManager.isWater)
        {
            go_BaseUi.SetActive(false); // 산소 게이지 UI 비활성화
            currentOxygen = totalOxygen;
            SoundManager.instance.PlaySE(sound_WaterOut);

            GameManager.isWater = false;
            _player.transform.GetComponent<Rigidbody>().drag = originDrag; // 플레이어 중력 변경(원래 중력)

            // 물 밖에서 밤, 낮 색깔 및 fog 시야
            if (!GameManager.isNight)
            {
                RenderSettings.fogColor = originColor;
                RenderSettings.fogDensity = originFogDensity;
            }
            else
            {
                RenderSettings.fogColor = originNightColor;
                RenderSettings.fogDensity = originNightFogDensity;
            }
        }
    }
}
