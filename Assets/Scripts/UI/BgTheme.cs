using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgTheme : MonoBehaviour
{
    private SpriteRenderer sr;
    private ManagerVars vars;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        sr = GetComponent<SpriteRenderer>();
        int randValue = Random.Range(0, vars.bgThemeSpriteList.Count);
        sr.sprite = vars.bgThemeSpriteList[randValue];
    }
}
