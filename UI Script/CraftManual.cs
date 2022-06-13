using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName; // �̸�
    public Sprite craftImage; // �̓G
    public string craftDesc; // ����
    public string[] craftNeedItem; // �ʿ��� ������
    public int[] craftNeedItemCount; // �ʿ��� ������ ����
    public GameObject go_Prefab; // ���� ��ġ�� ������
    public GameObject go_PreviewPrefab; // �̸����� ������
}

public class CraftManual : MonoBehaviour
{
    // ���º���
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField]
    private GameObject go_BaseUI; // �⺻ ���̽� UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_SelectedTab;

    [SerializeField]
    private Craft[] craft_fire; // ��ںҿ� ��
    [SerializeField]
    private Craft[] craft_build; // ����� ��

    private GameObject go_Preview; // �̸����� �������� ���� ����
    private GameObject go_Prefab; // ������ ������ �������� ���� ����

    [SerializeField]
    private Transform tf_Player;

    // Raycast �ʿ� ���� ����
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    // �ʿ��� UI ���� ���
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

    // �ʿ��� ������Ʈ
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

    // Tab ��� setting
    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch(tabNumber)
        {
            case 0:
                // �� ����
                TabSlotSetting(craft_fire);
                break;
            case 1:
                // ���� ����
                TabSlotSetting(craft_build);
                break;

        }
    }

    // ������ �̵� �� ���� slot clear
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

    // ������ ������ �̵� �� ���� ���๰ UI page setting
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

    // ���� ������ �̵� �� ���� ���๰ UI page setting
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

    // ��ں� tab - ���� tab slot setting
    private void TabSlotSetting(Craft[] _craft_tab)
    {
        ClearSlot();
        craft_SelectedTab = _craft_tab;

        int startSlotNumber = (page - 1) * go_Slots.Length;

        // �ش� tab�� �ִ� slot ���� ��ŭ �̹���, �̸�, ���� setting
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

    // ���� Ŭ�� �� �ش� ������ preview ����
    public void SlotClick(int _slotNumber)
    {
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        // Ŭ�� �� ���Կ� �ִ� ���๰�� �ʿ��� ��ᰡ ������ return
        if (!CheckIngredient())
        {
            return;
        }

        // �ش� ���๰�� preview ����
        go_Preview = Instantiate(craft_SelectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_SelectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated = true;

        GameManager.isOpenCraftManual = false; 

        go_BaseUI.SetActive(false); // ���� UI ��Ȱ��ȭ
    }

    // �ʿ��� ��� �κ��丮���� üũ
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

    // �ʿ� ��� �Ҹ�
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

    // Preview ����(��ġ ��) ���๰ ȸ�� ����
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;

                // �������� 90�� ȸ��
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    go_Preview.transform.Rotate(0f, -90f, 0f);
                }
                // ���������� 90�� ȸ��
                else if (Input.GetKeyDown(KeyCode.E)) 
                {
                    go_Preview.transform.Rotate(0f, +90f, 0f);
                }

                // preview ��ġ �� �ݿø�(x,z�� 1������ ���� ����� ������, y�� 0.1 ������ �� ����鼭 ������)
                _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f) * 0.1f, Mathf.Round(_location.z));
                go_Preview.transform.position = _location;
            }
        }
    }

    // ����
    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            UseIngredient(); // ��� �Ҹ�
            Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation); // �ش� preview ��ġ�� ���๰ ����
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    // ���� ���
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

    // ���� UIâ Ȱ��ȭ
    private void OpenWindow()
    {
        GameManager.isOpenCraftManual = true;

        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    // ���� UIâ ��Ȱ��ȭ
    private void CloseWindow()
    {
        GameManager.isOpenCraftManual = false;

        isActivated = false;
        go_BaseUI.SetActive(false);
    }
}
