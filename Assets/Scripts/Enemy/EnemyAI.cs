using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] State state;
    private Transform target;

    Rigidbody rigidbody;

    public float currentPower;
    public float currentMass;
    public int currentScore;

    private Player player;
    private bool onGround;

    private NavMeshAgent navMeshAgent;

    enum State
    {
        GetFood,
        Attack,
        None
    }


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        player = GameObject.Find("Player").GetComponent<Player>();

        currentPower = 2;
        currentMass = 1.2f;
        onGround = true;
        state = State.GetFood;

    }

    private void Update()
    {
        rigidbody.mass = currentMass;
        RaycastCheck();
        CheckStateOfTheAI();
        StateCheck();
        AIFallen();
        FindClosestFood();

        if (!navMeshAgent.enabled && onGround == true)
        {
            navMeshAgent.enabled = true;
        }
    }

    //if the ai is powerfull than the player attack if not try to get stronger by eating food 
    void CheckStateOfTheAI()
    {
        if(player.currentMass >= this.currentMass)
        {
            state = State.GetFood;
        }
        else if (player.currentMass < this.currentMass)
        {
            state = State.Attack;
        }
    }

    //check which state is in enemy
    void StateCheck()
    {
        switch(state)
        {
            case State.GetFood:
                if (FindClosestFood() != null)
                {
                    navMeshAgent.SetDestination(FindClosestFood().position); ;
                }
                else
                {
                    state= State.Attack;
                }
                break;

            case State.Attack:
                navMeshAgent.SetDestination(player.transform.position);
                break;
        }
    }

    //Check Collison as like player
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.TryGetComponent(out Rigidbody enemyRigidbody) && other.gameObject.TryGetComponent(out Player player))
            {
                if (this.currentMass > player.currentMass)
                {
                    //Throw Back player
                    Vector3 awayFromPlayer = (other.gameObject.transform.position - this.transform.position);
                    enemyRigidbody.AddForce(awayFromPlayer * this.currentPower, ForceMode.Impulse);
                }
                else if (this.currentMass < player.currentMass)
                {
                    //Throw Back this
                    Vector3 awayFromEnemy = (this.transform.position - other.gameObject.transform.position);
                    rigidbody.AddForce(awayFromEnemy * player.currentPower, ForceMode.Impulse);
                }
                else if (this.currentMass == player.currentMass)
                {
                    //Throw back two of them
                    Vector3 awayFromEnemy = (this.transform.position - other.gameObject.transform.position);
                    Vector3 awayFromPlayer = (other.gameObject.transform.position - this.transform.position);
                    rigidbody.AddForce(awayFromEnemy * player.currentPower, ForceMode.Impulse);
                    enemyRigidbody.AddForce(awayFromPlayer * this.currentPower, ForceMode.Impulse);
                }
            }
            else
            {
                return;
            }
        }

        if (other.gameObject.tag == "Food")
        {
            Destroy(other.gameObject);
            this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            this.currentMass += 0.2f;
            this.currentPower += 1f;
            this.currentScore += 100;
        }
    }

    public Transform FindClosestFood()
    {
        GameObject[] boost;
        boost = GameObject.FindGameObjectsWithTag("Food");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in boost)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest.transform;
    }


    //check if the enemy is on the ground if not disable navmeshagent this system is not working very well but this is the best i can do in 5 hours lol
    void RaycastCheck()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down) /2, out RaycastHit hit))
        {
            if(!hit.collider.CompareTag("Ground"))
            {
                onGround = false;
                navMeshAgent.enabled = false;
            }
            if(hit.collider.CompareTag("Ground"))
            {
                onGround = true;
                navMeshAgent.enabled = true;
            }
        }
    }

    void AIFallen()
    {
        if (this.transform.position.y < -3f)
        {
            Destroy(gameObject);
            player.currentScore += this.currentScore;
        }
    }
}

