using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // print the impact point's normal
        Debug.Log("Normal vector we collided at: " + hit.normal);
    }
}
