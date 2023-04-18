using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatEnemyScript : MonoBehaviour
{
       public GameObject player;

    public float maxAngle = 45;
    public float maxDistance = 2;
    public float timer = 1.0f;
    public float visionCheckRate = 1.0f;
    
    public Transform patrolPoints;
    private int patrolIndex;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    { 
        if(SeePlayer())
        {
            agent.Stop();
            Debug.Log("Cat sees player");
            var playerPos = player.transform.position;
            agent.destination = playerPos;
        } 
        else if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            Debug.Log("going to next point");
            GotoNextPoint();
        } else {
            Debug.Log("Going back to patrolling");
            agent.destination = patrolPoints.GetChild(patrolIndex).position;
        }
    }

    public void GotoNextPoint() {
        if(patrolPoints.childCount == 0) return;

        agent.destination = patrolPoints.GetChild(patrolIndex).position;

        patrolIndex = (patrolIndex+1)%patrolPoints.childCount;
    }

    public bool SeePlayer()
    {
        Vector3 vecPlayerTurret = player.transform.position - transform.position;
        if (vecPlayerTurret.magnitude > maxDistance)
        {
            return false;
        }
        Vector3 normVecPlayerTurret = Vector3.Normalize(vecPlayerTurret);
        float dotProduct = Vector3.Dot(transform.forward,normVecPlayerTurret);
        var angle = Mathf.Acos(dotProduct);
        float deg = angle * Mathf.Rad2Deg;
        if (deg < maxAngle)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position,transform.forward);
        
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    return true;
                }
                
            }
        }
        return false;
       
    }
}
