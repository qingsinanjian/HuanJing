using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    /// <summary>
    /// �Ƿ�����һ����Ϸ
    /// </summary>
    public static bool IsAgainGame = false;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinLocked;
    private int diamondCount;

    public void SetIsFirstGame(bool isFirstGame)
    {
        this.isFirstGame = isFirstGame;
    }

    public void SetIsMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;
    }

    public void SetBestScoreArr(int[] bestScoreArr)
    {
        this.bestScoreArr = bestScoreArr;
    }

    public void SetSelectSkin(int selectSkin)
    {
        this.selectSkin = selectSkin;
    }

    public void SetSkinLocked(bool[] skinUnlocked)
    {
        this.skinLocked = skinUnlocked;
    }

    public void SetDiamondCount(int diamondCount)
    {
        this.diamondCount = diamondCount;
    }

    public bool GetIsFirstGame()
    {
        return isFirstGame;
    }

    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }

    public int[] GetBestScoreArr()
    {
        return bestScoreArr;
    }

    public int GetSelectSkin()
    {
        return selectSkin;
    }

    public bool[] GetSkinLocked()
    {
        return skinLocked;
    }

    public int GetDiamondCount()
    {
        return diamondCount;
    }

}
