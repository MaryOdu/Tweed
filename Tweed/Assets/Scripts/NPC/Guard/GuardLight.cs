using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Enemy;

public class GuardLight : MonoBehaviour
{
    /// <summary>
    /// The guards' headlight.
    /// </summary>
    private Light m_headLight;

    /// <summary>
    /// The guards' behaviour/AI/Agent.
    /// </summary>
    private GuardAI m_agent;

    /// <summary>
    /// The colour of the guards headlight when on patrol.
    /// </summary>
    [SerializeField]
    private Color m_patrolColour;

    /// <summary>
    /// The colour of the guards headlight when searching for the player.
    /// </summary>
    [SerializeField]
    private Color m_searchColour;

    /// <summary>
    /// The colour of the guards headlight when alerted.
    /// </summary>
    [SerializeField]
    private Color m_alertColour;

    [SerializeField]
    private Color m_passiveColour; //<---------- to use when bots are passive to the player

    private bool m_isPassive;

    public bool UsePassiveColour
    {
        get
        {
            return m_isPassive;
        }
        set
        {
            m_isPassive = value;
        }
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public GuardLight()
    {
        m_patrolColour = new Color(0.9f, 0.9f, 1.0f);
        m_searchColour = new Color(0.75f, 0.75f, 0);
        m_alertColour = new Color(1.0f, 0, 0);
        m_passiveColour = new Color(0, 0.5f, 0);// <---------- to use when bots are passive to the player
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
            case GuardState.Patrol:
                m_headLight.color = this.UsePassiveColour ? m_passiveColour : m_patrolColour;
                break;
            case GuardState.Search:
                if (m_agent.RemainingSearchTime < 10.0f)
                {
                    this.BlinkSearchToPatrol();
                }
                else
                {
                    m_headLight.color = m_searchColour;
                }
                break;
            case GuardState.Alert:
                m_headLight.color = m_alertColour;
                break;
        }

        this.UpdateLightRangeAndAngle();
    }

    /// <summary>
    /// Updates the headlight paramets depending upon the Agents visability. The cone of the light is to reflect the agents cone of vision.
    /// </summary>
    private void UpdateLightRangeAndAngle()
    {
        m_headLight.range = m_agent.SightRange * 1.5f;
        m_headLight.innerSpotAngle = m_agent.SightAngle;
    }

    /// <summary>
    /// Will blink the agents headlight between patrol and search colour when the agent is transitioning from search back to patrol.
    /// </summary>
    private void BlinkSearchToPatrol()
    {
        var colour = (int)m_agent.RemainingSearchTime % 2 == 0 ? m_patrolColour : m_searchColour;
        m_headLight.color = colour;
    }
}
