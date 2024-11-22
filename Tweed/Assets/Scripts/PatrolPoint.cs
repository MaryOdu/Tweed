using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField]
    private bool m_showAtRuntime;

    private MeshRenderer meshRenderer;

    private static List<GameObject> allPatrolPoints = new List<GameObject>();

    public static List<GameObject> GetAllPatrolPoints()
    {
        return allPatrolPoints;
    }

    public PatrolPoint()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        this.meshRenderer = this.GetComponent<MeshRenderer>();
        allPatrolPoints.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        this.meshRenderer.enabled = !m_showAtRuntime;
    }
}
