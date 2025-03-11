using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObAndy : MonoBehaviour
{
    [SerializeField] GameObject AndyRide;
    [SerializeField] GameObject AndyStand;
    // Start is called before the first frame update
    void Start()
    {
        AndyRide.SetActive(false);
        AndyStand.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AndyRide.SetActive(true);
        AndyStand.SetActive(false);
    }
}
