using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Walking : MonoBehaviour
{
    Vector3 walkingVelocity = new Vector3(0, 0, 0);


    //personal information
    public int id;
    public string color;
    public Vector3 desiredVelocity;
    public Vector3 currentVelocity;
    public Vector3 MaxVelocity;
    public float abs_currentVelocity;

    public List<Vector3> TargetPosition;
    public Vector3 DisapearPosition;
    public int TargetIndex = 0;
    public Vector3 desiredDirection;
    public float relaxationTime = 0.5f;
    public Rigidbody rb;
    
    

    public float limitedAngle;
    

    //PedestianInterForce
    public float V0ab = 2.1f;
    public float sigma = 0.3f;
    public Vector3 PForce = new Vector3(0, 0, 0);
    public Vector3 RForce = new Vector3(0, 0, 0);
    public Vector3 WForce = new Vector3(0, 0, 0);

    public GameObject[] pedestrians;

    //WallInpulsiveForce
    public GameObject[] walls;
    public float U0ab = 10f;
    public float R = 0.2f;

    public GameObject Wall;
    public Vector3 CloseWallPoint;
    // Update is called once per frame
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        desiredVelocity = rb.velocity;
        currentVelocity = rb.velocity;
        MaxVelocity = rb.velocity * 1.3f;
        
    }
    void Update()
    {

        RForce = Vector3.zero;
        PForce = Vector3.zero;
        WForce = Vector3.zero;
        if ( Mathf.Abs(DisapearPosition.x - transform.position.x) < 0.5f )
        {
            if (color == "black")
            {
                this.gameObject.SetActive(false);
                transform.position = new Vector3((Manager.instance.WalkwayDistance + 10), 0, transform.position.z);
                TargetIndex = 0;
                Manager.instance.pedestrainBlack.Enqueue(this.gameObject);
            }
            if (color == "white")
            {
                this.gameObject.SetActive(false);
                transform.position = new Vector3(-(Manager.instance.WalkwayDistance + 10), 0, transform.position.z);
                TargetIndex = 0;
                Manager.instance.pedestrainWhite.Enqueue(this.gameObject);
            }
            return;
        }


        if (absVec3(TargetPosition[TargetIndex] - transform.position) < 3f)
        {
                TargetIndex++;
        }

        Force_OriginalVelcoity();
        FindAllPedestrian();
        FindWall();


        rb.AddForce(RForce + PForce + WForce, ForceMode.Acceleration);

        //rb.velocity = (RForce + PForce + WForce) * Time.deltaTime;
        //rb.AddForce(RForce, ForceMode.Acceleration);
        

        desiredDirection = GetDesiredDirection();
        transform.forward = desiredDirection;
        currentVelocity = rb.velocity;
        abs_currentVelocity = absVec3(currentVelocity);


        if(Vector3.Distance(CloseWallPoint, transform.position) > 4 )
            Wall = null;

        // after subject to all forces;
        //maxVelocityCheck();


    }
    Vector3 repulsivePedestrianForce(GameObject B)
    {
        float theda = 0.001f;
        float V = grad_r_ab(B,0,0);
        float VX = (grad_r_ab(B, theda, 0) - V ) / theda;
        float VZ = (grad_r_ab(B, 0, theda) - V ) / theda;

        Vector3 Force = new Vector3(VX, 0, VZ*2);
        //Debug.Log("repulsivePedestrianForce" + Force);
        PForce = Force;
        return Force; 
    }
    float grad_r_ab(GameObject B, float X, float Z)
    {
        Vector3 vectorBetweenAB = (B.transform.position - transform.position) + new Vector3(X,0,Z);
        float t1 = absVec3(vectorBetweenAB);
        float t2 = absVec3(vectorBetweenAB - B.GetComponent<Walking>().abs_currentVelocity * desiredDirection * Time.deltaTime);
        float t3 = B.GetComponent<Walking>().abs_currentVelocity * Time.deltaTime;
        float b = Mathf.Sqrt((t1 + t2) * (t1 + t2) - t3 * t3) / 2;

        float Vab = V0ab * Mathf.Exp(-b / sigma);
        return Vab;
    }
    Vector3 GetDesiredDirection()
    {
        // simple

        Vector3 vc = (TargetPosition[TargetIndex] - this.transform.position);
        //Debug.Log(vc / absVec3(vc));
        return (vc / absVec3(vc));

    }
    void Force_OriginalVelcoity()
    {
        Vector3 tmpVel = new Vector3(desiredVelocity.x * desiredDirection.x, desiredVelocity.y * desiredDirection.y, desiredVelocity.z * desiredDirection.z);

        Vector3 Force = (1 / relaxationTime) * ( absVec3(desiredVelocity)*desiredDirection - currentVelocity);
        RForce = Force;

        //Debug.Log("currentVelocity" + currentVelocity);
        //Debug.Log("desiredDirection" + desiredDirection);
        //Debug.Log("Relax Force" + Force);

  
    }
    void maxVelocityCheck()
    {
        if(absVec3(currentVelocity) > absVec3(MaxVelocity))
        {
            currentVelocity = MaxVelocity;
        }
    }
    float absVec3(Vector3 vector)
    {
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }
    void FindAllPedestrian()
    {
        PForce = new Vector3(0, 0, 0);
        pedestrians = GameObject.FindGameObjectsWithTag("pedestrian");
        if(pedestrians != null)
        {
            foreach (GameObject p in pedestrians)
            {
                if (p == this.gameObject)
                    continue;
                if (inSight(p))
                // && absVec3(this.gameObject.transform.position - p.transform.position) < 40f 
                {
                    //rb.AddForce(repulsivePedestrianForce(p),ForceMode.VelocityChange);
                    PForce += repulsivePedestrianForce(p) * 100f;
                }
                
            }
        }
        
        
    }
    void FindWall()
    {
        //WForce = new Vector3(0, 0, 0);
        walls = GameObject.FindGameObjectsWithTag("wall");

        
        if (Wall != null)
        {
            //Debug.Log(Vector3.Angle(Wall.transform.forward, this.transform.forward));
            if(Vector3.Angle(Wall.transform.forward, this.transform.forward) > 90)
            {
                CloseWallPoint -= Wall.transform.forward * 0.5f;
            }
            else
            {
                CloseWallPoint += Wall.transform.forward * 0.5f;
            }
        }

        //rb.AddForce(GetWallImpulsiveForce(CloseWallPoint) * -10000000f);
        WForce += GetWallImpulsiveForce(CloseWallPoint) * 10;

        //foreach(GameObject w in walls)
        //{
        //    //rb.AddForce( GetWallImpulsiveForce(w) * -10000000f);
        //    WForce += GetWallImpulsiveForce(w) * 1000000000f;
        //}
    }

    Vector3 GetWallImpulsiveForce(Vector3 wall)
    {
        float theda = 0.0001f;
        float U = Grad_Ua(wall, 0, 0);
        float UX = (Grad_Ua(wall, theda, 0)-U)/theda;
        float UZ = (Grad_Ua(wall, 0, theda)-U)/theda;
        Vector3 Force = new Vector3(UX, 0, UZ*3);
        return Force;
    }
    float Grad_Ua(Vector3 wall, float X, float Z)
    {
        Vector3 wall_ped = (wall - transform.position) + new Vector3(X,0,Z);
        return U0ab * Mathf.Exp(-(absVec3(wall_ped) / R));
    }
    bool inSight(GameObject B_pedestrian)
    {
        Vector3 dir = (B_pedestrian.transform.position - transform.position);
        float angle = Vector3.Angle(this.transform.forward, dir);
        if(angle < limitedAngle)
        {
            return true;
        }
        return false;
    }
    //void OnTriggerEnter(Collider other)
    //{

    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.forward, out hit))
    //    {
    //        //Debug.Log("Point of contact: " + hit.point);
    //        Wall = other.gameObject;
    //        CloseWallPoint = hit.point;
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            //Debug.Log("Point of contact: " + hit.point);
            Wall = other.gameObject;
            CloseWallPoint = hit.point;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        
    }

}
