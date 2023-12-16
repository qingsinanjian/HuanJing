using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    private Image img_Bg;
    private Text txt_Hint;

    private Color bgColor;
    private Color hintColor;

    private void Awake()
    {
        img_Bg = GetComponent<Image>();
        txt_Hint = GetComponentInChildren<Text>();
        bgColor = img_Bg.color;
        hintColor = txt_Hint.color;
        img_Bg.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
        txt_Hint.color = new Color(hintColor.r, hintColor.g, hintColor.b, 0);
        EventCenter.AddListener<string>(EventDefine.Hint, Show);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.Hint, Show);
    }

    private void Show(string hint)
    {
        StopCoroutine("Delay");
        transform.localPosition = new Vector3(0, -70, 0);
        transform.DOLocalMoveY(0, 0.3f).OnComplete(() =>
        {
            StartCoroutine("Delay");
        });
        img_Bg.DOColor(new Color(bgColor.r, bgColor.g, bgColor.b, 0.2f), 0.1f);
        txt_Hint.DOColor(new Color(hintColor.r, hintColor.g, hintColor.b, 1), 0.1f);
        txt_Hint.text = hint;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        transform.DOLocalMoveY(70, 0.3f);
        img_Bg.DOColor(new Color(bgColor.r, bgColor.g, bgColor.b, 0), 0.1f);
        txt_Hint.DOColor(new Color(hintColor.r, hintColor.g, hintColor.b, 0), 0.1f);
    }
}
