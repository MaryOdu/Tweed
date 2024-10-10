using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Enemy;

public class UpdateLightColour : MonoBehaviour
{
    private Light m_headLight;
    private AgentAI m_agent;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = this.GetComponent<AgentAI>();
        m_headLight = this.gameObject.FindChild("Headlight").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_agent.State)
        {
            case EnemyState.Patrol:
                m_headLight.color = new Color(0.9f, 0.9f, 1.0f);
                break;
            case EnemyState.Search:
                m_headLight.color = new Color(0.75f, 0.75f, 0);
                break;
            case EnemyState.Alert:
                m_headLight.color = new Color(1.0f, 0, 0);
                break;
        }
    }
}
