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
    private Button btn_Back;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
        parent = transform.Find("ScrollRect/Parent");
        txt_Name = transform.Find("txt_Name").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBackButtonClick);
        vars = ManagerVars.GetManagerVars();
        Init();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }

    private void OnBackButtonClick()
    {
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
            go.GetComponentInChildren<Image>().sprite = vars.skinSpriteList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
    }

    private void Update()
    {
        int selectedIndex = Mathf.RoundToInt(parent.localPosition.x / -160.0f);
        if(Input.GetMouseButtonUp(0))
        {
            parent.DOLocalMoveX(selectedIndex * -160, 0.2f);
            //parent.localPosition = new Vector3(currentIndex * (-160), 0);
        }
        SetItemSize(selectedIndex);
        RefreshUI(selectedIndex);
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
        txt_Name.text = vars.skinNameList[selectedIndex];
    }
}
