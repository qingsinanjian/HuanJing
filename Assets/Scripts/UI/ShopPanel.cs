using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    private ManagerVars vars;
    private Transform parent;
    private Text txt_Name;
    private Text txt_Diamond;
    private Button btn_Back;
    private Button btn_Select;
    private Button btn_Buy;

    private int selectIndex;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
        parent = transform.Find("ScrollRect/Parent");
        txt_Name = transform.Find("txt_Name").GetComponent<Text>();
        txt_Diamond = transform.Find("Diamond/Text").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBackButtonClick);
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Select.onClick.AddListener(OnSelectButtonClick);
        btn_Buy = transform.Find("btn_Buy").GetComponent<Button>();
        btn_Buy.onClick.AddListener(OnBuyButtonClick);
        vars = ManagerVars.GetManagerVars();
        
    }

    private void Start()
    {
        Init();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }
    /// <summary>
    /// 返回按钮点击
    /// </summary>
    private void OnBackButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 购买按钮点击
    /// </summary>
    private void OnBuyButtonClick()
    {
        int price = int.Parse(btn_Buy.GetComponentInChildren<Text>().text);
        if(price > GameManager.Instance.GetAllDiamond())
        {
            EventCenter.Broadcast(EventDefine.Hint, "钻石不足");
            return;
        }
        GameManager.Instance.UpdateAllDiamond(-price);
        GameManager.Instance.SetSkinUnlocked(selectIndex);
        parent.GetChild(selectIndex).GetChild(0).GetComponent<Image>().color = Color.white;
    }
    /// <summary>
    /// 选择按钮点击
    /// </summary>
    private void OnSelectButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ChangeSkin, selectIndex);
        GameManager.Instance.SetSelectedSkin(selectIndex);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Init()
    {
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.skinSpriteList.Count + 2) * 160, 282);
        for (int i = 0; i < vars.skinSpriteList.Count; i++)
        {
            GameObject go = Instantiate(vars.skinChooseItemPre, parent);
            if(GameManager.Instance.GetSkinUnlocked(i) == false)
            {
                go.GetComponentInChildren<Image>().color = Color.gray;
            }
            else
            {
                go.GetComponentInChildren<Image>().color = Color.white;
            }
            go.GetComponentInChildren<Image>().sprite = vars.skinSpriteList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        //打开页面直接定位到选中的皮肤
        parent.transform.localPosition = new Vector3(GameManager.Instance.GetCurrentSelectedSkin() * -160, 0);
    }

    private void Update()
    {
        selectIndex = Mathf.RoundToInt(parent.localPosition.x / -160.0f);
        if(Input.GetMouseButtonUp(0))
        {
            parent.DOLocalMoveX(selectIndex * -160, 0.2f);
            //parent.localPosition = new Vector3(currentIndex * (-160), 0);
        }
        SetItemSize(selectIndex);
        RefreshUI(selectIndex);
    }

    private void SetItemSize(int index)
    {
        for (int i = 0;i < parent.childCount;i++)
        {
            if(index == i)
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160);
            }
            else
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
        }
    }

    private void RefreshUI(int selectedIndex)
    {
        txt_Diamond.text = GameManager.Instance.GetAllDiamond().ToString();
        txt_Name.text = vars.skinNameList[selectedIndex];
        if (GameManager.Instance.GetSkinUnlocked(selectedIndex) == false)
        {
            btn_Select.gameObject.SetActive(false);
            btn_Buy.gameObject.SetActive(true);
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPriceList[selectedIndex].ToString();
        }
        else
        {
            btn_Select.gameObject.SetActive(true);
            btn_Buy.gameObject.SetActive(false);
        }
    }
}
