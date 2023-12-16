using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankPanel : MonoBehaviour
{
    private Button btn_Close;
    private Color btnImgColor;
    public Text[] txtScores;
    private GameObject go_ScoreList;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowRankPanel, Show);
        btn_Close = transform.Find("btn_Close").GetComponent<Button>();
        btn_Close.onClick.AddListener(OnCloseButtonClick);
        btnImgColor = btn_Close.GetComponent<Image>().color;
        go_ScoreList = transform.Find("ScoreList").gameObject;
        btn_Close.GetComponent<Image>().color =new Color(btnImgColor.r, btnImgColor.g, btnImgColor.b, 0);
        go_ScoreList.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowRankPanel, Show);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        btn_Close.GetComponent<Image>().DOColor(new Color(btnImgColor.r, btnImgColor.g, btnImgColor.b, 0.2f), 0.3f);
        go_ScoreList.transform.DOScale(Vector3.one, 0.3f);
        int[] arr = GameManager.Instance.GetScoreArr();
        for (int i = 0; i < arr.Length; i++)
        {
            txtScores[i].text = arr[i].ToString();
        }
    }

    private void OnCloseButtonClick()
    {        
        btn_Close.GetComponent<Image>().DOColor(new Color(btnImgColor.r, btnImgColor.g, btnImgColor.b, 0f), 0.3f);
        go_ScoreList.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
