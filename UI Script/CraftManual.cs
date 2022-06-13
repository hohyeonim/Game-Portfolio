using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public Sprite craftImage; // 이밎
    public string craftDesc; // 설명
    public string[] craftNeedItem; // 필요한 아이템
    public int[] craftNeedItemCount; // 필요한 아이템 개수
    public GameObject go_Prefab; // 실제 설치될 프리팹
    public GameObject go_PreviewPrefab; // 미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    // 상태변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField]
    private GameObject go_BaseUI; // 기본 베이스 UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_SelectedTab;

    [SerializeField]
    private Craft[] craft_fire; // 모닥불용 탭
    [SerializeField]
    private Craft[] craft_build; // 건축용 탭

    private GameObject go_Preview; // 미리보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제로 생성될 프리팹을 담을 변수

    [SerializeField]
    private Transform tf_Player;

    // Raycast 필요 변수 선언
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    // 필요한 UI 슬롯 요소
    [SerializeField]
    private GameObject[] go_Slots;
    [SerializeField]
    private Image[] image_Slot;
    [SerializeField]
    private Text[] text_SlotName;
    [SerializeField]
    private Text[] text_SlotDesc;
    [SerializeField]
    private Text[] text_SlotNeedItem;

    // 필요한 컴포넌트
    private Inventory theInventory;

    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        TabSlotSetting(craft_fire);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            Window();
        }

        if (isPreviewActivated)
        {
            PreviewPositionUpdate();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Build();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    // Tab 목록 setting
    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch(tabNumber)
        {
            case 0:
                // 불 셋팅
                TabSlotSetting(craft_fire);
                break;
            case 1:
                // 건축 셋팅
                TabSlotSetting(craft_build);
                break;

        }
    }

    // 페이지 이동 시 기존 slot clear
    private void ClearSlot()
    {
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_SlotName[i].text = "";
            text_SlotDesc[i].text = "";
            text_SlotNeedItem[i].text = "";
            go_Slots[i].SetActive(false);
        }
    }

    // 오른쪽 페이지 이동 시 새로 건축물 UI page setting
    public void RightPageSetting()
    {
        if (page < craft_SelectedTab.Length / go_Slots.Length + 1)
        {
            page++;
        }
        else
        {
            page = 1;
        }

        TabSlotSetting(craft_SelectedTab);
    }

    // 왼쪽 페이지 이동 시 새로 건축물 UI page setting
    public void LeftPageSetting()
    {
        if (page != 1)
        {
            page--;
        }
        else
        {
            page = (craft_SelectedTab.Length / go_Slots.Length) + 1;
        }

        TabSlotSetting(craft_SelectedTab);
    }

    // 모닥불 tab - 건축 tab slot setting
    private void TabSlotSetting(Craft[] _craft_tab)
    {
        ClearSlot();
        craft_SelectedTab = _craft_tab;

        int startSlotNumber = (page - 1) * go_Slots.Length;

        // 해당 tab에 있는 slot 갯수 만큼 이미지, 이름, 설명 setting
        for (int i = startSlotNumber; i < craft_SelectedTab.Length; i++)
        {
            if (i == page * go_Slots.Length)
            {
                break;
            }

            go_Slots[i - startSlotNumber].SetActive(true);

            image_Slot[i - startSlotNumber].sprite = craft_SelectedTab[i].craftImage;
            text_SlotName[i - startSlotNumber].text = craft_SelectedTab[i].craftName;
            text_SlotDesc[i - startSlotNumber].text = craft_SelectedTab[i].craftDesc;

            for (int x = 0; x < craft_SelectedTab[i].craftNeedItem.Length; x++)
            {
                text_SlotNeedItem[i - startSlotNumber].text += craft_SelectedTab[i].craftNeedItem[x];
                text_SlotNeedItem[i - startSlotNumber].text += " x " + craft_SelectedTab[i].craftNeedItemCount[x] + "\n";
            }
            
        }
    }

    // 슬롯 클릭 시 해당 아이템 preview 생성
    public void SlotClick(int _slotNumber)
    {
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        // 클릭 한 슬롯에 있는 건축물에 필요한 재료가 없으면 return
        if (!CheckIngredient())
        {
            return;
        }

        // 해당 건축물에 preview 생성
        go_Preview = Instantiate(craft_SelectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_SelectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated = true;

        GameManager.isOpenCraftManual = false; 

        go_BaseUI.SetActive(false); // 건축 UI 비활성화
    }

    // 필요한 재료 인벤토리에서 체크
    private bool CheckIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if (theInventory.GetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i])
            {
                return false;
            }
        }

        return true;
    }

    // 필요 재료 소모
    private void UseIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            theInventory.SetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i], craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i]);
        }
    }

    
    private void Window()
    {
        if (!isActivated)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    // Preview 상태(설치 전) 건축물 회전 구현
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;

                // 왼쪽으로 90도 회전
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    go_Preview.transform.Rotate(0f, -90f, 0f);
                }
                // 오른쪽으로 90도 회전
                else if (Input.GetKeyDown(KeyCode.E)) 
                {
                    go_Preview.transform.Rotate(0f, +90f, 0f);
                }

                // preview 위치 값 반올림(x,z는 1단위로 툭툭 끊기며 움직임, y는 0.1 단위로 덜 끊기면서 움직임)
                _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f) * 0.1f, Mathf.Round(_location.z));
                go_Preview.transform.position = _location;
            }
        }
    }

    // 건축
    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            UseIngredient(); // 재료 소모
            Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation); // 해당 preview 위치에 건축물 생산
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    // 건축 취소
    private void Cancel()
    {
        if (isPreviewActivated)
        {
            Destroy(go_Preview);
        }

        isActivated = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;

        GameManager.isOpenCraftManual = false;

        go_BaseUI.SetActive(false);
    }

    // 건축 UI창 활성화
    private void OpenWindow()
    {
        GameManager.isOpenCraftManual = true;

        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    // 건축 UI창 비활성화
    private void CloseWindow()
    {
        GameManager.isOpenCraftManual = false;

        isActivated = false;
        go_BaseUI.SetActive(false);
    }
}
