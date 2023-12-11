using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacle;
    private bool startTimer;
    private float fallTime;
    private Rigidbody2D my_Body;

    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
    }

    public void Init(Sprite sprite, float fallTime, int obstacleDir)
    {
        my_Body.bodyType = RigidbodyType2D.Static;
        startTimer = true;
        this.fallTime = fallTime;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;
        }

        if(obstacleDir == 0)//³¯ÓÒ±ß
        {
            if(obstacle != null)
            {
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y, obstacle.transform.localPosition.z);
            }        
        }
    }

    private void Update()
    {
        if(GameManager.Instance.IsGameStarted == false || GameManager.Instance.PlayerIsMove == false)
        {
            return;
        }
        if(startTimer)
        {
            fallTime -= Time.deltaTime;
            if(fallTime < 0)
            {
                //µôÂä
                startTimer = false;
                if(my_Body.bodyType != RigidbodyType2D.Dynamic)
                {
                    my_Body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DelayHide());
                }
            }
        }
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
