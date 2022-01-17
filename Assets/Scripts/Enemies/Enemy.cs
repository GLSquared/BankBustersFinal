using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

/*
    Luke Zammit
    26.12.2021
*/

public class Enemy : MonoBehaviour
{
    // Enemy state machine
    public enum State
    {
        Spawning,
        Idle,
        Following,
        Surround,
        Attacking,
        Dead,
    };

    public bool DebugEnabled = false;

    // Enemy Layer
    public int EnemyLayer = 6;

    // Enemy current State
    public State currentState = State.Idle;

    // General Enemy properties
    [Range(1, 1000)] public float MaxHealth = 100f;
    [Range(1, 1000)] public float health = 100f;

    // Enemy sight
    [SerializeField] public GameObject viewAt;
    public bool UseViewAngle = true;
    [Range(1, 180)]  public float ViewAngle = 90f;
    [Range(1, 1000)] public float ViewDistance = 20f;

    // Engagement Properties
    //[Range(1, 100)] public float MinimumEngageDistance = 10.0f;
    public float MinimumEngageDistance;
    [Range(1, 100)] public float MaximumEngageDistance = 15.0f;

    // Nav Mesh Data
    NavMeshAgent agent;
    [Range(1, 10)]  public float WalkSpeed = 5f;
    [Range(1, 420)] public float TurnSpeed = 260f;
    [Range(1, 10)]  public float Acceleration = 8f;
                    private bool pathfind = false;

    // Combat data
    private int Behaviour = 0;
    public float currentWalkSpeed = 0f;
    private bool crouching = false;
    private bool shooting = false;
    private bool dead = false;

    // Target
    public GameObject currentTarget;

    private Object[] weapons;
    private Object[] items;
    
    private void setPathfind(Transform target, bool val)
    {
        agent.destination = target.position - new Vector3(0,1f,0);
        pathfind = val;
    }

    public bool isDead()
    {
        return dead;
    }

    public bool isCrouching()
    {
        return crouching;
    }

    public bool isShooting()
    {
        return shooting;
    }

    public bool isPathfinding()
    {
        return pathfind;
    }

    public void UpdateState(State newState)
    {
        currentState = newState;
    }

    public void Heal(float healAmount)
    {
        if (currentState != State.Dead)
        {
            health = Mathf.Clamp(health + healAmount, health, MaxHealth);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health = Mathf.Clamp(health - damageAmount, 0, health);

        if (health <= 0)
        {
            UpdateState(State.Dead);
            Dead();
        }
    }

    private void LogicDebugStart()
    {
        print("Minimum Engange Distance: "+MinimumEngageDistance);
        print("Behaviour : "+Behaviour);
    }

    private void LogicDebug()
    {
        agent.speed         = WalkSpeed;
        agent.angularSpeed  = TurnSpeed;
        agent.acceleration  = Acceleration;
    }
    
    public void LogicStart()
    {
        weapons = Resources.LoadAll("Weapon Drops");
        items = Resources.LoadAll("ItemDrops");

        UpdateState(State.Spawning);

        MinimumEngageDistance = Random.Range(5,10);
        Behaviour = Random.Range(1, 4);
        
        agent = GetComponent<NavMeshAgent>();
        agent.speed         = WalkSpeed;
        agent.angularSpeed  = TurnSpeed;
        agent.acceleration  = Acceleration;
        agent.stoppingDistance = MinimumEngageDistance;


        /*
        if (Behaviour == 3)
        {
            crouching = true;
        }
        */

        if (DebugEnabled) {
            LogicDebugStart();
        }
    }

    public void LogicUpdate()
    {
        if (DebugEnabled)
        {
            LogicDebug();
        }

        // Update values
        currentWalkSpeed = agent.velocity.magnitude;

        // Update using states
        switch (currentState) 
        {
            case State.Dead :
                Dead();
                return;
            case State.Spawning :
                Spawning();
                break;
            case State.Idle :
                Idle();
                break;
            case State.Following :
                Following();
                break;
            case State.Attacking:
                Fire();
                break;
            default :
                print("Idle - Enemy");
                break;
        }

        CheckForTarget();
    }

    private float DistanceTo(Transform start, Transform target)
    {
        return (start.position - target.position).magnitude;
    }

    public bool CanSeeTarget(GameObject target, int ignoreLayer)
    {
        /*
        int mask = 1 << ignoreLayer;
        mask = ~mask;
        */

        RaycastHit hit;
        if ( Physics.Raycast(transform.position, (target.transform.position + new Vector3(0, 1.5f, 0) - transform.position ) , out hit, ViewDistance) )
            if (hit.collider != null && hit.collider.tag == "Player")
                return true;

        return false;
    }

    private bool IsInViewAngle(GameObject target)
    {   
        float angle = Vector3.Angle((target.transform.position - transform.position), transform.forward);

        return angle <= ViewAngle/2 && angle >= -ViewAngle/2;
    }

    private void CheckForTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            foreach (GameObject player in players)
            {
                if (currentTarget == player)
                    continue;

                if (!UseViewAngle || (UseViewAngle && IsInViewAngle(player)) )
                {
                    if (CanSeeTarget(player, EnemyLayer))
                    {
                        print(player);
                        if (currentTarget && DistanceTo(transform, currentTarget.transform) >= DistanceTo(transform, player.transform))
                        {
                            currentTarget = player;

                        }
                        else
                        {
                            currentTarget = player;

                        }
                    }
                }
            }
        }
    }

    private void Spawning()
    {
        UpdateState(State.Idle);
    }

    private void Idle()
    {

        if (currentTarget) {
            UpdateState(State.Following);
            return;
        }
    }

    private void Following()
    {

        if (dead)
        {
            UpdateState(State.Dead);
        }

        if (currentTarget == null)
        {
            UpdateState(State.Idle);
            return;
        }


        if (!CanSeeTarget(currentTarget, EnemyLayer))
        {
            setPathfind(currentTarget.transform, true);
        }
        
        if (agent.destination != null)
        {
            if (CanSeeTarget(currentTarget, EnemyLayer))
            {
                UpdateState(State.Attacking);
                return;
                //setPathfind(transform, false);
            }

        } else {
            setPathfind(null, false);
        }
    }

    public void Dead()
    {
        DropWeapon();
        dead = true;
        Destroy(gameObject);
    }

    private void Fire()
    {
        
        if (CanSeeTarget(currentTarget, EnemyLayer))
        {
             float step = TurnSpeed * Time.deltaTime;

             Quaternion target = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
             transform.rotation = Quaternion.RotateTowards(transform.rotation, target, step);

            shooting = true;

        }
        else
        {
            shooting = false;
            UpdateState(State.Following);
            return;
        }

        
    }

    private void DropWeapon()
    {
        int ranInt = Random.Range(0, 2);

        if (ranInt == 0)
        {
            int wepRandInt = Random.Range(0, weapons.Length);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
            {
                GameObject wep = (GameObject)Instantiate(weapons[wepRandInt], hit.point + new Vector3(0, .2f, 0), Quaternion.identity);
                wep.AddComponent<WeaponDestroy>();
                wep.name = weapons[wepRandInt].name;
            }
        }

        if (ranInt == 1)
        {
            int itemRanInt = Random.Range(0, items.Length);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
            {
                GameObject item = (GameObject)Instantiate(items[itemRanInt], hit.point + new Vector3(0, .2f, 0), Quaternion.identity);
                item.AddComponent<WeaponDestroy>();
                item.name = items[itemRanInt].name;
            }
        }
    }
}
