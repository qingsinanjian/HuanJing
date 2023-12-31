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
    private AudioSource m_AudioSource;


    private void Awake()
    {
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        vars = ManagerVars.GetManagerVars();
        my_Body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }

    /// <summary>
    /// 音效是否开启
    /// </summary>
    /// <param name="value"></param>
    private void IsMusicOn(bool value)
    {
        m_AudioSource.mute = !value;
    }

    /// <summary>
    /// 更换皮肤
    /// </summary>
    /// <param name="selectSkin"></param>
    private void ChangeSkin(int selectSkin)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[selectSkin];
    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点到UI
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 1, Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.down * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.down * 0.15f, Color.red);
        //if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    //如果点击在UI上则退出
        //    int fingerId = Input.GetTouch(0).fingerId;
        //    if (EventSystem.current.IsPointerOverGameObject(fingerId)) return;
        //}
        //else
        //{
        //    if (EventSystem.current.IsPointerOverGameObject()) return;
        //}

        if(IsPointerOverGameObject(Input.mousePosition)) { return; }


        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver
            || GameManager.Instance.IsGamePaused)
            return;

        if (Input.GetMouseButtonDown(0) && isJumping == false && nextPlatformLeft != Vector3.zero)
        {
            EventCenter.Broadcast(EventDefine.PlayerMove);
            isMove = true;
            m_AudioSource.PlayOneShot(vars.jumpClip);
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
            m_AudioSource.PlayOneShot(vars.fallClip);
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            //调用结束面板
            StartCoroutine(DelayShowGameOverPanel());
        }

        if (isJumping && IsRayObstacle() && !GameManager.Instance.IsGameOver)
        {
            m_AudioSource.PlayOneShot(vars.hitClip);
            GameObject go = ObjectPool.Instance.GetDeathEffect();
            go.SetActive(true);
            go.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;
            StartCoroutine(DelayShowGameOverPanel());
        }

        if (transform.position.y - Camera.main.transform.position.y < -6 && GameManager.Instance.IsGameOver == false)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
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
            m_AudioSource.PlayOneShot(vars.diamondClip);
            collision.gameObject.SetActive(false);
            EventCenter.Broadcast(EventDefine.AddDiamond);
        }
    }
}
