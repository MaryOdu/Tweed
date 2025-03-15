using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GateDoor : MonoBehaviour
{
    private float m_startPos;

    [SerializeField] 
    private float m_endPos;

    [SerializeField] 
    private float m_openSpeed = 0.2f;

    private bool m_doorOpen = false;

    public bool IsOpen
    {
        get
        {
            return m_doorOpen;
        }
    }

    private void Start()
    {
        m_startPos = this.transform.localPosition.x;
    }

    private void Update()
    {
        var tgtPos = (m_doorOpen == true) ? m_endPos : m_startPos;
        float deltaX = Mathf.Lerp(transform.localPosition.x, tgtPos, m_openSpeed * Time.deltaTime);

        if (Mathf.Abs(deltaX) > 0.1f)
        {
            transform.localPosition = new Vector3(deltaX, transform.localPosition.y, transform.localPosition.z);
        }
    }

    public void Open()
    {
        m_doorOpen = true;
    }

    public void Close()
    {
        m_doorOpen = false;
    }
}


