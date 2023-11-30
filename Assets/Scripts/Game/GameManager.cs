using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsGameStarted {  get; set; }
    public bool IsGameOver { get; set;}
    public bool IsGamePaused { get; set;}

    private int gameScore;

    private void Awake()
    {
        Instance = this;
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
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
}
