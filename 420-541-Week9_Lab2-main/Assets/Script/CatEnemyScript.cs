using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CatEnemyScript : MonoBehaviour
{
    public GameObject player;

    public float maxAngle = 45;
    public float maxDistance = 10;
    public float timer;
    public float visionCheckRate = 1.0f;
    
    public Transform patrolPoints;
    private int patrolIndex;
    private bool chasing = false;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update() {
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0.0f, 2.0f);
        if (SeePlayer()) {
            chasing = true;
            timer = 2.0f;
            agent.destination = player.transform.position;
        }
        else if (timer > 0) {
            agent.destination = player.transform.position;
        }
        else if (chasing) {
            chasing = false;
            agent.destination = patrolPoints.GetChild(patrolIndex).position;
        } 
        else if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            GotoNextPoint();
        } 
    }

    private void GotoNextPoint() {
        if(patrolPoints.childCount == 0) return;

        agent.destination = patrolPoints.GetChild(patrolIndex).position;

        patrolIndex = (patrolIndex+1)%patrolPoints.childCount;
    }

    private bool SeePlayer()
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
                if (hit.collider.CompareTag("Player"))
                {
                    // Debug.Log($"cat sees player: {vecPlayerTurret.magnitude}");
                    return true;
                }
                
            }
        }
        return false;
       
    }
    
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
