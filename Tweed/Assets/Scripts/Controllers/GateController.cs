using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private GateDoor m_doorL;
    private GateDoor m_doorR;

    public bool IsOpen
    {
        get
        {
            return m_doorL.IsOpen && m_doorR.IsOpen;
        }
    }

    public bool IsClosed
    {
        get
        {
            return !this.IsOpen;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_doorL = this.gameObject.FindChild("GDoorL").GetComponent<GateDoor>();
        m_doorR = this.gameObject.FindChild("GDoorR").GetComponent<GateDoor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        m_doorL.Open();
        m_doorR.Open();
    }

    public void Close()
    {
        m_doorL.Close();
        m_doorR.Close();
    }
}
