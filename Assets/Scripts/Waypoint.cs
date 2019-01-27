using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public float minDuration = 1, maxDuration = 3;
    public float arrivalRadius = 0.5f;
    public List<Waypoint> adjacents; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, arrivalRadius);
        foreach (var adj in adjacents)
            Gizmos.DrawLine(transform.position, adj.transform.position);
    }
}
