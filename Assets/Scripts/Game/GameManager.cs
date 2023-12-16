using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private ManagerVars vars;
    private GameData data;

    public bool IsGameStarted {  get; set; }
    public bool IsGameOver { get; set;}
    public bool IsGamePaused { get; set;}

    public bool PlayerIsMove {  get; set;}

    private int gameScore;
    private int gameDiamond;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinUnlocked;
    private int diamondCount;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        Instance = this;
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventDefine.AddDiamond, AddDiamond);

        if (GameData.IsAgainGame)
        {
            IsGameStarted = true;
        }
        InitGameData();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
        EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddDiamond);
    }

    private void AddGameScore()
    {
        if(!IsGameStarted || IsGameOver || IsGamePaused)
        {
            return;
        }
        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdateScoreText, gameScore);
    }

    /// <summary>
    /// ��ҿ�ʼ�ƶ����ô˷���
    /// </summary>
    private void PlayerMove()
    {
        PlayerIsMove = true;
    }

    /// <summary>
    /// ����ɼ�
    /// </summary>
    /// <param name="score"></param>
    public void SaveScore(int score)
    {
        List<int> list = bestScoreArr.ToList();
        //�Ӵ�С����list
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();
        int index = -1;
        for (int i = 0; i < bestScoreArr.Length; i++)
        {
            if(score > bestScoreArr[i])
            {
                index = i;
            }
        }

        if(index == -1)
        {
            return;
        }

        for (int i = bestScoreArr.Length - 1; i > index; i--)
        {
            bestScoreArr[i] = bestScoreArr[i - 1];
        }
        bestScoreArr[index] = score;
        Save();
    }

    /// <summary>
    /// �����߷�����
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr()
    {
        List<int> list = bestScoreArr.ToList();
        //�Ӵ�С����list
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();

        return bestScoreArr;
    }

    /// <summary>
    /// ��ȡ���߷�
    /// </summary>
    /// <returns></returns>
    public int GetBestScore()
    {
        return bestScoreArr.Max();
    }

    /// <summary>
    /// ��ȡ��Ϸ�ɼ�
    /// </summary>
    /// <returns></returns>
    public int GetGameScore()
    {
        return gameScore;
    }
    /// <summary>
    /// ������Ϸ��ʯ����
    /// </summary>
    private void AddDiamond()
    {
        gameDiamond++;
        EventCenter.Broadcast(EventDefine.UpdateDiamondText, gameDiamond);
    }
    /// <summary>
    /// ��õ�ǰѡ�е�Ƥ��
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSelectedSkin()
    {
        return selectSkin;
    }

    /// <summary>
    /// ���õ�ǰ��ѡ�е�Ƥ��
    /// </summary>
    /// <param name="index"></param>
    public void SetSelectedSkin(int index)
    {
        selectSkin = index;
        Save();
    }

    /// <summary>
    /// ��ȡ��ǰƤ���Ƿ����
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool GetSkinUnlocked(int index)
    {
        return skinUnlocked[index];
    }

    /// <summary>
    /// ���õ�ǰƤ���ѽ���
    /// </summary>
    /// <param name="index"></param>
    public void SetSkinUnlocked(int index)
    {
        skinUnlocked[index] = true;
        Save();
    }

    /// <summary>
    /// ��ȡ������ʯ����
    /// </summary>
    /// <returns></returns>
    public int GetAllDiamond()
    {
        return diamondCount;
    }

    public void UpdateAllDiamond(int value)
    {
        diamondCount += value;
        Save();
    }

    /// <summary>
    /// ��óԵ�����ʯ��
    /// </summary>
    /// <returns></returns>
    public int GetGameDiamond()
    {
        return gameDiamond;
    }

    private void InitGameData()
    {
        Read();
        if(data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else
        {
            isFirstGame = true;
        }
        //�����һ�ο�ʼ��Ϸ
        if(isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            skinUnlocked = new bool[vars.skinSpriteList.Count];
            skinUnlocked[0] = true;
            diamondCount = 10;
            data = new GameData();
            Save();
        }
        else
        {
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectSkin = data.GetSelectSkin();
            skinUnlocked = data.GetSkinLocked();
            diamondCount = data.GetDiamondCount();
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            //ʹ��using�����Զ��ͷ��ļ���
            using(FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data"))
            {
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetBestScoreArr(bestScoreArr);
                data.SetSkinLocked(skinUnlocked);
                data.SetDiamondCount(diamondCount);
                data.SetSelectSkin(selectSkin);
                bf.Serialize(fs, data);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// ��ȡ
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using(FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open))
            {
               data = (GameData)bf.Deserialize(fs);
            }
        }
        catch (System.Exception e)
        {

        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        skinUnlocked = new bool[vars.skinSpriteList.Count];
        skinUnlocked[0] = true;
        diamondCount = 10;
        Save();
    }
}
