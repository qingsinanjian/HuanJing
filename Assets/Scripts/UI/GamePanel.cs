using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    private Button btn_Pause;
    private Button btn_Play;
    private Text txt_Score;
    private Text txt_DiamondCount;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowGamePanel, Show);
        EventCenter.AddListener<int>(EventDefine.UpdateScoreText, UpdateScoreText);
        EventCenter.AddListener<int>(EventDefine.UpdateDiamondText, UpdateDiamondText);
        Init();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGamePanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.UpdateScoreText, UpdateScoreText);
        EventCenter.RemoveListener<int>(EventDefine.UpdateDiamondText, UpdateDiamondText);
    }

    private void Init()
    {
        btn_Pause = transform.Find("btn_Pause").GetComponent<Button>();
        btn_Pause.onClick.AddListener(OnPauseButtonClick);
        btn_Play = transform.Find("btn_Play").GetComponent<Button>();
        btn_Play.onClick.AddListener(OnPlayButtonClick);
        txt_Score = transform.Find("txt_Score").GetComponent<Text>();
        txt_DiamondCount = transform.Find("Diamond/txt_DiamondCount").GetComponent<Text>();
        btn_Play.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 更新成绩
    /// </summary>
    /// <param name="score"></param>
    private void UpdateScoreText(int score)
    {
        txt_Score.text = score.ToString();
    }
    /// <summary>
    /// 更新钻石数量显示
    /// </summary>
    /// <param name="diamond"></param>
    private void UpdateDiamondText(int diamond)
    {
        txt_DiamondCount.text = diamond.ToString();
    }

    /// <summary>
    /// 暂停按钮点击
    /// </summary>
    private void OnPauseButtonClick()
    {
        btn_Pause.gameObject.SetActive(false);
        btn_Play.gameObject.SetActive(true);
        Time.timeScale = 0;
        GameManager.Instance.IsGamePaused = true;
    }

    /// <summary>
    /// 开始按钮点击
    /// </summary>
    private void OnPlayButtonClick()
    {
        btn_Play.gameObject.SetActive(false);
        btn_Pause.gameObject.SetActive(true);
        Time.timeScale = 1;
        GameManager.Instance.IsGamePaused = false;
    }
}
