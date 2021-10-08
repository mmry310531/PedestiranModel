using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    public int Sence;
    public GameObject door;
    public GameObject pedestrian;
    public GameObject AI;
    public float mean;
    public float deviation;
    public float WalkwayDistance = 30f;
    public int TotalPedestrian = 0;
    public int PedestrianNum = 300;
    
    
    public Queue<GameObject> pedestrainBlack = new Queue<GameObject>();
    public Queue<GameObject> pedestrainWhite = new Queue<GameObject>();
    public float timer = 1f;
    public float StartTimer = 1f;
    public float Frequence = 2f;
    GameObject p1;
    GameObject p2;
    private void Start()
    {
        instance = this;
        CreatePedestrian();

    }

    // Update is called once per frame
    void Update()
    {
        

        timer -= Frequence * Time.deltaTime;
        if(timer <= 0)
        {
            if(pedestrainBlack.Count != 0)
            {
                p1 = pedestrainBlack.Dequeue();
            } 
            if(pedestrainWhite.Count != 0)
            {
                p2 = pedestrainWhite.Dequeue();
            }
            p1.SetActive(true);
            p2.SetActive(true);
            timer = StartTimer;
        }
        


    }

    public void CreatePedestrian()
    {

        //Debug.Log(PedestrianNum);
        for(int i = 0; i < PedestrianNum; i++)
        {
            //Debug.Log("hello");
            var p = Instantiate(pedestrian);
            Walking WalkingCS = p.GetComponent<Walking>();

            WalkingCS.TargetPosition = new List<Vector3>();
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
                    WalkingCS.TargetPosition.Add(new Vector3(door.transform.position.x, 0, door.transform.position.z));
                }
                WalkingCS.TargetPosition.Add(new Vector3(-WalkwayDistance * 10, 0, p.transform.position.z));

                WalkingCS.DisapearPosition = new Vector3(-WalkwayDistance , 0, p.transform.position.z);
                p.SetActive(false);
                WalkingCS.color = "black"; 
                pedestrainBlack.Enqueue(p);

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
                    WalkingCS.TargetPosition.Add(new Vector3(door.transform.position.x, 0, door.transform.position.z));
                }
                WalkingCS.TargetPosition.Add(new Vector3(WalkwayDistance * 10, 0, p.transform.position.z));
                WalkingCS.DisapearPosition = new Vector3(WalkwayDistance, 0, p.transform.position.z);
                p.SetActive(false);
                WalkingCS.color = "white";

                pedestrainWhite.Enqueue(p);
                //p.GetComponent<Walking>().TargetObject = a;

                //a.GetComponent<AI_move>().TargetPosition = new Vector3(WalkwayDistance, 0, p.transform.position.z);
                //a.GetComponent<AI_move>().pedestrian = p;
            }
            TotalPedestrian++;
            //Debug.Log("Vecolity: " + p.GetComponent<Rigidbody>().velocity);
        }


        /*
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
        */
        
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
        float z = Random.Range(-9, 9);
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
