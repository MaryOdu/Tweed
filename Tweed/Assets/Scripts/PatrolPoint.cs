using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
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
        var renderer = this.GetComponent<MeshRenderer>();
        renderer.enabled = false;

        allPatrolPoints.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
