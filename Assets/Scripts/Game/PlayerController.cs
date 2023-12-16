using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

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
    private bool isMove;

    private void Awake()
    {
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        vars = ManagerVars.GetManagerVars();
        my_Body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }

    /// <summary>
    /// 更换皮肤
    /// </summary>
    /// <param name="selectSkin"></param>
    private void ChangeSkin(int selectSkin)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[selectSkin];
    }

    private void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 1, Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.down * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.down * 0.15f, Color.red);
        //如果点击在UI上则退出
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver
            || GameManager.Instance.IsGamePaused)
            return;

        if (Input.GetMouseButtonDown(0) && isJumping == false && nextPlatformLeft != Vector3.zero)
        {
            EventCenter.Broadcast(EventDefine.PlayerMove);
            isMove = true;
            EventCenter.Broadcast(EventDefine.DecidePath);
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
        if (my_Body.velocity.y < 0 && IsRayPlatform() == false && GameManager.Instance.IsGameOver == false)
        {
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            //调用结束面板
            StartCoroutine(DelayShowGameOverPanel());
        }

        if (isJumping && IsRayObstacle() && !GameManager.Instance.IsGameOver)
        {
            GameObject go = ObjectPool.Instance.GetDeathEffect();
            go.SetActive(true);
            go.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;
            StartCoroutine(DelayShowGameOverPanel());
        }

        if (transform.position.y - Camera.main.transform.position.y < -6 && GameManager.Instance.IsGameOver == false)
        {
            GameManager.Instance.IsGameOver = true;
            StartCoroutine(DelayShowGameOverPanel());
        }

    }

    private IEnumerator DelayShowGameOverPanel()
    {
        yield return new WaitForSeconds(1.2f);
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
    }

    private GameObject lastHitGo = null;
    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                if (lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
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
        if (leftHit.collider != null)
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
        if (isJumping)
        {
            if (isMoveLeft)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Pickup"))
        {
            collision.gameObject.SetActive(false);
            EventCenter.Broadcast(EventDefine.AddDiamond);
        }
    }
}
