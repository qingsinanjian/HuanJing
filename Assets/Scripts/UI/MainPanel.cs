using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private Button btn_Start;
    private Button btn_Shop;
    private Button btn_Rank;
    private Button btn_Sound;
    private Button btn_Reset;

    private ManagerVars vars;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.ShowMainPanel, Show);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        Init();        
    }

    private void Start()
    {
        if (GameData.IsAgainGame)
        {
            EventCenter.Broadcast(EventDefine.ShowGamePanel);
            gameObject.SetActive(false);
        }

        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowMainPanel, Show);
    }

    /// <summary>
    /// 皮肤更换,更换商店UI皮肤图片
    /// </summary>
    /// <param name="skinIndex"></param>
    private void ChangeSkin(int skinIndex)
    {
        btn_Shop.transform.GetChild(0).GetComponent<Image>().sprite = vars.skinSpriteList[skinIndex];
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Init()
    {
        btn_Start = transform.Find("btn_Start").GetComponent<Button>();
        btn_Start.onClick.AddListener(OnStartButtonClick);
        btn_Shop = transform.Find("btns/btn_Shop").GetComponent<Button>();
        btn_Shop.onClick.AddListener(OnShopButtonClick);
        btn_Rank = transform.Find("btns/btn_Rank").GetComponent<Button>();
        btn_Rank.onClick.AddListener(OnRankButtonClick);
        btn_Sound = transform.Find("btns/btn_Sound").GetComponent<Button>();
        btn_Sound.onClick.AddListener(OnSoundButtonClick);
        btn_Reset = transform.Find("btns/btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnResetButtonClick);
    }

    /// <summary>
    /// 开始按钮点击后调用此方法
    /// </summary>
    private void OnStartButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowGamePanel);
        GameManager.Instance.IsGameStarted = true;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 商店按钮点击后调用此方法
    /// </summary>
    private void OnShopButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowShopPanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Rank按钮点击后调用此方法
    /// </summary>
    private void OnRankButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }

    /// <summary>
    /// 声音按钮点击后调用此方法
    /// </summary>
    private void OnSoundButtonClick()
    {

    }

    /// <summary>
    /// 重置按钮点击
    /// </summary>
    private void OnResetButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowResetPanel);
    }
}
