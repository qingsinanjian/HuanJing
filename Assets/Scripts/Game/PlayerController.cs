using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public Transform rayDown, rayLeft, rayRight;
    public LayerMask platformLayer, obstacleLayer;
    /// <summary>
    /// 是否向左移动
    /// </summary>
    private bool isMoveLeft = false;
    /// <summary>
    /// 是否正在跳跃
    /// </summary>
    private bool isJumping = false;
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_Body;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        my_Body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 1, Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.down * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.down * 0.15f, Color.red);
        if (GameManager.Instance.IsGameStarted == true)
        {
            if (Input.GetMouseButtonDown(0) && isJumping == false)
            {
                isJumping = true;
                Vector3 mousePos = Input.mousePosition;
                if (mousePos.x <= Screen.width / 2)//点击了屏幕左边
                {
                    isMoveLeft = true;
                }
                else
                {
                    isMoveLeft = false;
                }
                Jump();
            }
            //游戏结束
            if(my_Body.velocity.y < 0 && !IsRayPlatform() && !GameManager.Instance.IsGameOver)
            {
                spriteRenderer.sortingLayerName = "Default";
                GetComponent<BoxCollider2D>().enabled = false;
                GameManager.Instance.IsGameOver = true;
                //调用结束面板
            }

            if(isJumping && IsRayObstacle() && !GameManager.Instance.IsGameOver)
            {
                GameObject go = ObjectPool.Instance.GetDeathEffect();
                go.SetActive(true);
                go.transform.position = transform.position;
                GameManager.Instance.IsGameOver = true;
                Destroy(gameObject);
            }
        }        
    }

    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null && hit.collider.CompareTag("Platform"))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否检测到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);
        if(leftHit.collider != null)
        {
            if (leftHit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        if (rightHit.collider != null)
        {
            if (rightHit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    private void Jump()
    {
        EventCenter.Broadcast(EventDefine.DecidePath);
        if(isMoveLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);
            transform.DOMoveY(nextPlatformLeft.y + 0.8f, 0.15f);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            isJumping = false;
            Vector3 curPlatformPos = collision.transform.position;
            nextPlatformLeft = new Vector3(curPlatformPos.x - vars.nextXPos, curPlatformPos.y + vars.nextYPos, 0);
            nextPlatformRight = new Vector3(curPlatformPos.x + vars.nextXPos, curPlatformPos.y + vars.nextYPos, 0);
        }
    }
}
