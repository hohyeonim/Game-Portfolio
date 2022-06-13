using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField]
    private float waterDrag; // ���� �߷�
    private float originDrag;

    [SerializeField]
    private Color waterColor; // ���� ����
    [SerializeField]
    private float waterFogDensity; // �� Ź�� ����

    [SerializeField]
    private Color waterNightColor; // �� ������ ���� ����
    [SerializeField]
    private float waterNightFogDensity; // �� ������ ���� Ź��

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

    // ��� ������ ����, ��Ұ������� �� �پ��� ������
    private void DecreaseOxygen()
    {
        if (GameManager.isWater)
        {
            currentOxygen -= Time.deltaTime; // ���� ��� �������� ���ʸ��� ���� �����ش�.
            text_currentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString(); // ���� ��� ������ text�� �Ҽ������� ���� �ʵ��� RoundToInt�� ���ش�.
            image_gauge.fillAmount = currentOxygen / totalOxygen; // ��� ������ �ٸ� ���� ���� �ش�.

            if (currentOxygen <= 1)
            {
                temp += Time.deltaTime;
                
                if (temp >= 1)
                {
                    // �÷��̾� ������
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

    // �� �� �÷��̾�
    private void GetWater(Collider _player)
    {
        SoundManager.instance.PlaySE(sound_WaterIn);

        go_BaseUi.SetActive(true); // ��� ������ UI Ȱ��ȭ
        GameManager.isWater = true;
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag; // �÷��̾� �߷� ����(���� �߷�)

        // �� �ӿ��� ��, �� ���� �÷� �� fog�þ�
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

    // �� �� �÷��̾�
    private void GetOutWater(Collider _player)
    {
        if (GameManager.isWater)
        {
            go_BaseUi.SetActive(false); // ��� ������ UI ��Ȱ��ȭ
            currentOxygen = totalOxygen;
            SoundManager.instance.PlaySE(sound_WaterOut);

            GameManager.isWater = false;
            _player.transform.GetComponent<Rigidbody>().drag = originDrag; // �÷��̾� �߷� ����(���� �߷�)

            // �� �ۿ��� ��, �� ���� �� fog �þ�
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
