using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsGameStarted {  get; set; }
    public bool IsGameOver { get; set;}
    public bool IsGamePaused { get; set;}

    public bool PlayerIsMove {  get; set;}

    private int gameScore;
    private int gameDiamond;

    private void Awake()
    {
        Instance = this;
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventDefine.AddDiamond, AddDiamond);
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
}
