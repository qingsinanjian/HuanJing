using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAudio : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private ManagerVars vars;

    private void Awake()
    {
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.AddListener(EventDefine.PlayClickAudio, PlayAudio);
        vars = ManagerVars.GetManagerVars();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.RemoveListener(EventDefine.PlayClickAudio, PlayAudio);
    }

    private void PlayAudio()
    {
        m_AudioSource.PlayOneShot(vars.buttonClip);
    }

    /// <summary>
    /// ÒôÐ§ÊÇ·ñ¿ªÆô
    /// </summary>
    /// <param name="value"></param>
    private void IsMusicOn(bool value)
    {
        m_AudioSource.mute = !value;
    }
}
