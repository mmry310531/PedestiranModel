using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_move : MonoBehaviour
{
    public GameObject pedestrian;
    public NavMeshAgent AI_controller;
    public Vector3 TargetPosition;
    private void Start()
    {
        AI_controller.isStopped = false;
    }
    void Update()
    {
        AI_controller.isStopped = false;

        if (Vector3.Magnitude(pedestrian.transform.position - transform.position) >= 2f)
        {
            AI_controller.isStopped = true;
            return;
        }
        
        AI_controller.SetDestination(TargetPosition);
    }
}
