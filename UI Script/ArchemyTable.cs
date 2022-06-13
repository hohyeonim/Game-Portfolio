using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ArchemyItem
{
    public string itemName;
    public string itemDesc;
    public Sprite itemImage;

    public string[] needItemName;
    public int[] needItemNumber;

    public float itemCraftingTime; // 포션 제조에 걸리는 시간 (5초, 10초, 100초)

    public GameObject go_ItemPrefab;
}

public class ArchemyTable : MonoBehaviour
{
    public bool GetIsOpen()
    {
        return isOpen;
    }

    private bool isOpen = false;
    private bool isCrafting = false; // 아이템의 제작 시작 여부

    [SerializeField]
    private ArchemyItem[] archemyItems; // 제작할 수 있는 연금 아이템 리스트

    private Queue<ArchemyItem> archemyItemQueue = new Queue<ArchemyItem>(); // 연금 아이템 제작 대기열 (큐)
    private ArchemyItem currentCraftingItem; // 현재 제작중인 연금 아이템

    private float craftingTime; // 포션 제작 시간
    private float currentCraftingTime; // 실제 계산 시간
    private int page = 1; // 연금 제작 테이블의 페이지
    [SerializeField]
    private int theNumberOfSlot; // 한페이지 당 슬롯의 최대 개수(4개)

    [SerializeField]
    private Image[] image_ArchemyItems; // 페이지에 따른 포션 이미지들
    [SerializeField]
    private Text[] text_ArchemyItems; // 페이지에 따른 포션 텍스트들
    [SerializeField]
    private Button[] btn_ArchemyItems; // 페이지에 따른 포션 버튼
    [SerializeField]
    private Slider slider_gauge; // 슬라이더 게이지
    [SerializeField]
    private Transform tf_BaseUI; // 베이스 UI
    [SerializeField]
    private Transform tf_PotionAppearPos; // 포션 나올 위치
    [SerializeField]
    private GameObject go_Liquid; // 동작 시키면 액체 등장.
    [SerializeField]
    private Image[] image_CraftingItems; //대기열 슬롯에 있는 아이템 이미지들

    // 필요한 컴포넌트
    [SerializeField]
    private ArchemyToolTip theToolTip;
    private AudioSource theAudio;
    private Inventory theInven;
    [SerializeField]
    private AudioClip sound_ButtonClick;
    [SerializeField]
    private AudioClip sound_Beep;
    [SerializeField]
    private AudioClip sound_Activate;
    [SerializeField]
    private AudioClip sound_ExitItem;

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    private void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
        ClearSlot();
        PageSetting();
    }

    void Update()
    {
        if (!IsFinish())
        {
            Crafting();
        }

        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWindow();
            }
        }
    }

    // 물약 생성 중에 archemytable 위에 플라스크 병에 액체 생성
    private bool IsFinish()
    {
        if (archemyItemQueue.Count == 0 && !isCrafting)
        {
            go_Liquid.SetActive(false);
            slider_gauge.gameObject.SetActive(false);
            return true;
        }
        else
        {
            go_Liquid.SetActive(true);
            slider_gauge.gameObject.SetActive(true);
            return false;
        }
    }

    // 생산 중
    private void Crafting()
    {
        if (!isCrafting && archemyItemQueue.Count != 0)
        {
            DequeueItem(); 
        }

        if (isCrafting)
        {
            // 생산 게이지 바
            currentCraftingTime += Time.deltaTime;
            slider_gauge.value = currentCraftingTime;

            if (currentCraftingTime >= craftingTime)
            {
                ProductionComplete(); // 물약 생산
            }
        }
    }

    // Dequeue을 이용하여 풀링(반복 : 생산 -> 파괴 -> 생산 -> 파괴)
    private void DequeueItem()
    {
        PlaySE(sound_Activate);

        isCrafting = true;
        currentCraftingItem = archemyItemQueue.Dequeue();

        craftingTime = currentCraftingItem.itemCraftingTime;
        currentCraftingTime = 0;
        slider_gauge.maxValue = craftingTime;

        CraftingImageChange();
    }

    // 여러 물약 생산 시 생산이 완료된 물약은 대기 공간에서 사라지고 다음 생산 물약으로 이미지 교체
    private void CraftingImageChange()
    {
        image_CraftingItems[0].gameObject.SetActive(true);

        // 위에서 Dequeue를 했으므로 Count에 1을 더함
        for (int i = 0; i < archemyItemQueue.Count + 1; i++)
        {
            image_CraftingItems[i].sprite = image_CraftingItems[i + 1].sprite;
            if (i + 1 == archemyItemQueue.Count + 1)
            {
                image_CraftingItems[i + 1].gameObject.SetActive(false);
            }
        }
    }

    public void Window()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    // 연금 테이블 UI 활성화
    private void OpenWindow()
    {
        isOpen = true;
        GameManager.isOpenArchemyTable = true;
        tf_BaseUI.localScale = new Vector3(1f, 1f, 1f);
    }

    // 연금 테이블 UI 비활성화
    private void CloseWindow()
    {
        isOpen = false;
        GameManager.isOpenArchemyTable = false;
        tf_BaseUI.localScale = new Vector3(0f, 0f, 0f);
    }

    // 물약 버튼 클릭 시 인벤토리에 재료가 있는지 확인 후 생산
    public void ButtonClick(int _buttonNum)
    {
        PlaySE(sound_ButtonClick);

        if (archemyItemQueue.Count < 3)
        {
            int archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);

            // 인벤토리에서 재료 검색
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                if (theInven.GetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i]) < archemyItems[archemyItemArrayNumber].needItemNumber[i])
                {
                    PlaySE(sound_Beep);
                    return;
                }
            }

            // 인벤토리 재료 감산
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                theInven.SetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i], archemyItems[archemyItemArrayNumber].needItemNumber[i]);
            }

            // 제작 시작
            archemyItemQueue.Enqueue(archemyItems[archemyItemArrayNumber]);

            image_CraftingItems[archemyItemQueue.Count].gameObject.SetActive(true);
            image_CraftingItems[archemyItemQueue.Count].sprite = archemyItems[archemyItemArrayNumber].itemImage;
        }
        else
        {
            PlaySE(sound_Beep);
        }
    }

    // 물약 생산 완료
    private void ProductionComplete()
    {
        isCrafting = false;
        image_CraftingItems[0].gameObject.SetActive(false);

        PlaySE(sound_ExitItem);

        // 해당 위치에 물약 생산
        Instantiate(currentCraftingItem.go_ItemPrefab, tf_PotionAppearPos.position, Quaternion.identity);
    }

    // 이전 페이지 이동
    public void UpButton()
    {
        PlaySE(sound_ButtonClick);

        if (page != 1)
        {
            page--;
        }
        else
        {
            page = 1 + (archemyItems.Length / theNumberOfSlot);
        }

        ClearSlot();
        PageSetting();
    }

    // 다음 페이지 이동
    public void DownButton()
    {
        PlaySE(sound_ButtonClick);

        if (page < 1 + (archemyItems.Length / theNumberOfSlot))
        {
            page++;
        }
        else
        {
            page = 1;
        }

        ClearSlot();
        PageSetting();
    }

    // 페이지 이동 시 기존에 있던 슬롯 Clear
    private void ClearSlot()
    {
        for (int i = 0; i < theNumberOfSlot; i++)
        {
            image_ArchemyItems[i].sprite = null;
            image_ArchemyItems[i].gameObject.SetActive(false);
            btn_ArchemyItems[i].gameObject.SetActive(false);
            text_ArchemyItems[i].text = "";
        }
    }

    // 페이지 이동 시 새로운 슬롯 setting
    private void PageSetting()
    {
        int pageArrayStartNumber = (page - 1) * theNumberOfSlot;

        for (int i = pageArrayStartNumber; i < archemyItems.Length; i++)
        {
            if (i == page * theNumberOfSlot)
            {
                break;
            }

            image_ArchemyItems[i - pageArrayStartNumber].sprite = archemyItems[i].itemImage;
            image_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            btn_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            text_ArchemyItems[i - pageArrayStartNumber].text = archemyItems[i].itemName + "\n" + archemyItems[i].itemDesc;
        }
    }

    // 해당 물약 아이템 tooltip UI 활성화(재료)
    public void ShowToolTip(int _buttonNum)
    {
        int _archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);
        theToolTip.ShowToolTip(archemyItems[_archemyItemArrayNumber].needItemName, archemyItems[_archemyItemArrayNumber].needItemNumber);
    }

    public void HideToolTip()
    {
        theToolTip.HideToolTip();
    }
}
