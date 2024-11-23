using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Enemy;
using Assets.Scripts.NPC.Sentry;

/// <summary>
/// A behaviour script for managing the light of a given sentry entity.
/// </summary>
public class SentryLight : MonoBehaviour
{
    /// <summary>
    /// The headlight 'Light' component of the sentry.
    /// </summary>
    private Light m_headLight;

    /// <summary>
    /// The Agent / SentryAI component also attatched to this game object.
    /// </summary>
    private SentryAI m_agent;

    /// <summary>
    /// The colour of the sentrys' light when in passive mode.
    /// </summary>
    [SerializeField]
    private Color m_passiveColour;

    /// <summary>
    /// The colour of the sentrys' light when in alert mode.
    /// </summary>
    [SerializeField]
    private Color m_alertColour;

    //[SerializeField]
    //private Color m_friendlyColour; <---------- to use when bots are friendly

    public SentryLight()
    {
        m_passiveColour = new Color(0.9f, 0.9f, 1.0f);
        m_alertColour = new Color(0.75f, 0.75f, 0);
        //m_friendlyColour = new Color(0, 0.5f, 0); <---------- to use when bots are friendly
    }

    // Start is called before the first frame update
    void Start()
    {
        m_agent = this.GetComponent<SentryAI>();
        m_headLight = this.gameObject.FindChild("Headlight").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_agent.State)
        {
            case SentryState.Passive:
                m_headLight.color = m_passiveColour;
                break;
            case SentryState.Alert:
                m_headLight.color = m_alertColour;
                break;
        }

        this.UpdateLightRangeAndAngle();
    }

    /// <summary>
    /// Updates the lights' range, shape and angle.
    /// </summary>
    private void UpdateLightRangeAndAngle()
    {
        m_headLight.range = m_agent.SightRange;
        m_headLight.shape = LightShape.Cone;
        m_headLight.innerSpotAngle = m_agent.SightAngle;
    }
}
