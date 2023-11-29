using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_FollowTarget;
    private Vector3 offset;
    private Vector2 velocity;

    private void Update()
    {
        if(m_FollowTarget == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            m_FollowTarget = GameObject.FindGameObjectWithTag("Player").transform;
            offset = m_FollowTarget.position - transform.position;
        }
    }

    private void FixedUpdate()
    {
        if(m_FollowTarget != null)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, m_FollowTarget.position.x - offset.x, ref velocity.x, 0.05f);
            float posY = Mathf.SmoothDamp(transform.position.y, m_FollowTarget.position.y - offset.y, ref velocity.y, 0.05f);
            if(posY > transform.position.y)
            {
                transform.position = new Vector3(posX, posY, transform.position.z);
            }
        }
    }
}
