using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Manager : MonoBehaviour
{
    public int Sence;
    public GameObject door;
    public GameObject pedestrian;
    public GameObject AI;
    public float mean;
    public float deviation;
    public float WalkwayDistance = 40f;
    public int TotalPedestrian = 0;
    private void Start()
    {
         TotalPedestrian = 0;

    }

    // Update is called once per frame
    void Update()
    {
        CreatePedestrian();
    }

    public void CreatePedestrian()
    {
        if (Input.anyKeyDown)
        {

            var p = Instantiate(pedestrian);
            p.GetComponent<Walking>().TargetPosition = new List<Vector3>();
            //var a = Instantiate(AI);


            Vector3 position = getPosition();
            p.transform.position = position;
            //a.transform.position = position;

            p.GetComponent<Walking>().id = TotalPedestrian++;
            if (p.transform.position.x > 0)
            {
                p.GetComponent<Rigidbody>().velocity = new Vector3(-1f * getGaussinVelocity(mean, deviation), 0, 0);
                p.GetComponent<MeshRenderer>().material.color = Color.black;
                if (Sence == 1)
                {
                    p.GetComponent<Walking>().TargetPosition.Add(new Vector3(door.transform.position.x, 0, door.transform.position.z));
                }
                p.GetComponent<Walking>().TargetPosition.Add(new Vector3(-WalkwayDistance, 0, p.transform.position.z));

                //p.GetComponent<Walking>().TargetObject = a;

                //a.GetComponent<AI_move>().TargetPosition = new Vector3(-WalkwayDistance, 0, p.transform.position.z);
                //a.GetComponent<AI_move>().pedestrian = p;


            }
            else
            {
                p.GetComponent<Rigidbody>().velocity = new Vector3(getGaussinVelocity(mean, deviation), 0, 0);
                p.GetComponent<MeshRenderer>().material.color = Color.white;

                if (Sence == 1)
                {
                    p.GetComponent<Walking>().TargetPosition.Add(new Vector3(door.transform.position.x, 0, door.transform.position.z));
                }
                p.GetComponent<Walking>().TargetPosition.Add(new Vector3(WalkwayDistance, 0, p.transform.position.z));
                //p.GetComponent<Walking>().TargetObject = a;

                //a.GetComponent<AI_move>().TargetPosition = new Vector3(WalkwayDistance, 0, p.transform.position.z);
                //a.GetComponent<AI_move>().pedestrian = p;
            }
            //Debug.Log("Vecolity: " + p.GetComponent<Rigidbody>().velocity);

            
        }
        
    }
    public float getGaussinVelocity(float mean, float deviation)
    {

        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);


        return (v1*s * deviation) + mean;
    }
    public Vector3 getPosition()
    {
        float z = Random.Range(-10, 10);
        float x = WalkwayDistance * randomBool();
        return new Vector3(x, 0, z);
    }
    public int randomBool()
    {
        if(Random.value > 0.5)
        {
            return 1;
        }
        return -1;
    }

}
