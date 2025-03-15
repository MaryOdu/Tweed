using Assets.Scripts;
using Assets.Scripts.Enemy;
using Assets.Scripts.Events;
using Assets.Scripts.NPC;
using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    [SerializeField]
    private bool m_interactable;

    [SerializeField]
    private bool m_interactOnce = false;

    private bool m_triggered;

    private AudioSource m_interactSound;

    [SerializeField]
    private SwitchEvent m_switchEvent;

    [SerializeField]
    private SwitchController m_switchController;

    public ControlPanel()
    {
        m_interactable = true;
        m_interactOnce = false;
    }

    void Start()
    {
        m_interactSound = this.GetComponent<AudioSource>();
    }

    public void FireTrigger()
    {
        if (m_interactable == false || (m_interactOnce && m_triggered))
        {
            return;
        }

        m_triggered = true;
        m_switchEvent.Invoke(new SwitchEventsArgs { Sender = this.gameObject });

        if (m_interactSound != null)
        {
            m_interactSound.Play();
        }
    }
}
