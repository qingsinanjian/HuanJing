using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public Transform rayDown;
    public LayerMask platformLayer;
    /// <summary>
    /// �Ƿ������ƶ�
    /// </summary>
    private bool isMoveLeft = false;
    /// <summary>
    /// �Ƿ�������Ծ
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
        if(GameManager.Instance.IsGameStarted == true)
        {
            if (Input.GetMouseButtonDown(0) && isJumping == false)
            {
                isJumping = true;
                Vector3 mousePos = Input.mousePosition;
                if (mousePos.x <= Screen.width / 2)//�������Ļ���
                {
                    isMoveLeft = true;
                }
                else
                {
                    isMoveLeft = false;
                }
                Jump();
            }
            //��Ϸ����
            if(my_Body.velocity.y < 0 && !IsRayPlatform() && !GameManager.Instance.IsGameOver)
            {
                spriteRenderer.sortingLayerName = "Default";
                GetComponent<BoxCollider2D>().enabled = false;
                GameManager.Instance.IsGameOver = true;
                //���ý������
            }
        }        
    }

    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null && hit.collider.CompareTag("Platform"))
        {
            return true;
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
