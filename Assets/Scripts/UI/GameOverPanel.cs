using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Text txt_Score, txt_BestScore, txt_AddDiamondCount;
    public Button btn_Restart, btn_Rank, btn_Home;

    private void Awake()
    {
        btn_Restart.onClick.AddListener(OnRankButtonClick);
        btn_Rank.onClick.AddListener(OnRankButtonClick);
        btn_Home.onClick.AddListener(OnHomeButtonClick);
        EventCenter.AddListener(EventDefine.ShowGameOverPanel, Show);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGameOverPanel, Show);
    }

    private void Show()
    {
        txt_Score.text = GameManager.Instance.GetGameScore().ToString();
        txt_AddDiamondCount.text = "+" + GameManager.Instance.GetGameDiamond().ToString();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ����һ�ְ�ť���
    /// </summary>
    private void OnRestartButtonClick()
    {

    }
    /// <summary>
    /// ���а�ť���
    /// </summary>
    private void OnRankButtonClick()
    {

    }

    /// <summary>
    /// �����水ť���
    /// </summary>
    private void OnHomeButtonClick()
    {

    }
}
