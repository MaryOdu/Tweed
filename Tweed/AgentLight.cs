using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Enemy;

public class GuardLight : MonoBehaviour
{
    private Light m_headLight;
    private GuardAI m_agent;

    [SerializeField]
    private Color m_patrolColour;

    [SerializeField]
    private Color m_searchColour;

    [SerializeField]
    private Color m_alertColour;

    //[SerializeField]
    //private Color m_friendlyColour; <---------- to use when bots are friendly

    public GuardLight()
    {
        m_patrolColour = new Color(0.9f, 0.9f, 1.0f);
        m_searchColour = new Color(0.75f, 0.75f, 0);
        m_alertColour = new Color(1.0f, 0, 0);
        //m_friendlyColour = new Color(0, 0.5f, 0); <---------- to use when bots are friendly
    }

    // Start is called before the first frame update
    void Start()
    {
        m_agent = this.GetComponent<GuardAI>();
        m_headLight = this.gameObject.FindChild("Headlight").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_agent.State)
        {
            case EnemyState.Patrol:
                m_headLight.color = m_patrolColour;
                break;
            case EnemyState.Search:
                if (m_agent.RemainingSearchTime < 10.0f)
                {
                    this.BlinkSearchToPatrol();
                }
                else
                {
                    m_headLight.color = m_searchColour;
                }
                break;
            case EnemyState.Alert:
                m_headLight.color = m_alertColour;
                break;
        }

        this.UpdateLightRangeAndAngle();
    }

    private void UpdateLightRangeAndAngle()
    {
        m_headLight.range = m_agent.SightRange;
        m_headLight.spotAngle = m_agent.SightRange;
    }

    private void BlinkSearchToPatrol()
    {
        var colour = (int)m_agent.RemainingSearchTime % 2 == 0 ? m_patrolColour : m_searchColour;
        m_headLight.color = colour;
    }
}
