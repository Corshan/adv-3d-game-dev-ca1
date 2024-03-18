using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Leader : MonoBehaviour
{
    [SerializeField] private GameObject[] _waypoints;
    private List<GameObject> _teamMembers;
    [SerializeField][Range(1, 5)] private float _distanceToWaypoint = 2.5f;
    [SerializeField] private Type _type;
    private enum Type
    {
        PLAYER,
        NPC,
    }
    private int _waypointIndex = 0;
    private float _patrolTimer = 0.0f;
    private Animator _anim;
    private AnimatorStateInfo _animInfo;
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();

        string targetTag = gameObject.CompareTag("Leader") ? "PlayerTeam" : "NpcTeam";

        _teamMembers = GameObject.FindGameObjectsWithTag(targetTag).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (_type == Type.PLAYER)
        {
            PlayerInput();
        }
        else
        {
            if (_animInfo.IsName("Death")) return;

            if (GetComponent<Health>().IsDead())
            {
                foreach (var member in _teamMembers)
                {
                    member.GetComponent<TeamMember>().LeaderDead();
                }
            }

            _patrolTimer += Time.deltaTime;
            _animInfo = _anim.GetCurrentAnimatorStateInfo(0);
            if (_animInfo.IsName("Idle"))
            {
                if (_patrolTimer >= 5)
                {
                    _patrolTimer = 0;
                    _anim.SetTrigger("startPatrol");
                }
            }

            if (_animInfo.IsName("Patrol"))
            {
                if (_patrolTimer >= 4)
                {
                    DetectEnemies();
                    _patrolTimer = 0;
                }

                if (Vector3.Distance(gameObject.transform.position, _waypoints[_waypointIndex].transform.position) < _distanceToWaypoint)
                {
                    _waypointIndex++;
                    if (_waypointIndex >= _waypoints.Length)
                    {
                        _waypointIndex = 0;
                    }
                }

                _agent.SetDestination(_waypoints[_waypointIndex].transform.position);
                _agent.isStopped = false;
            }

            if (_animInfo.IsName("Attack"))
            {
                _agent.isStopped = true;
                CheckHealth();
                CheckTargets();
            }
        }
    }

    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Retreat();
        }
    }

    void DetectEnemies()
    {
        if (Vector3.Distance(gameObject.transform.position, GameObject.Find("Player").transform.position) < 10)
        {
            _anim.SetTrigger("closeToEnemy");
        }
    }

    private void CheckHealth()
    {
        foreach (var member in _teamMembers)
        {
            if (member.GetComponent<Health>().IsDead())
            {
                _anim.SetTrigger("startPatrol");
                _teamMembers.Remove(member);
                Retreat();
            }
        }
    }

    private void CheckTargets()
    {
        string targetTag = gameObject.CompareTag("Leader") ? "PlayerTeam" : "NpcTeam";
        Debug.Log($"{gameObject.name}  {targetTag}");

        var allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        var aliveTargets = new List<GameObject>();

        allTargets.ForEach(t =>
        {
            if (!t.GetComponent<Health>().IsDead()) aliveTargets.Add(t);
        });

        Debug.Log($"{gameObject.name}  {aliveTargets.Count}");
        if (aliveTargets.Count == 0) _anim.SetTrigger("startPatrol");
    }

    public void Attack()
    {
        GameObject[] allTargets;

        if (_type == Type.NPC)
        {
            allTargets = GameObject.FindGameObjectsWithTag("PlayerTeam");
        }
        else
        {
            allTargets = GameObject.FindGameObjectsWithTag("NpcTeam");
        }

        int targetIndex = 0;
        foreach (var member in _teamMembers)
        {
            Debug.Log($"{member.name}  Attack: {targetIndex}");
            member.GetComponent<TeamMember>().Attack(allTargets[targetIndex]);

            targetIndex = targetIndex < allTargets.Length ? targetIndex++ : 0;
        }
    }

    void Retreat()
    {
        foreach (var teamMember in _teamMembers)
        {
            teamMember.GetComponent<TeamMember>().Retreat();
        }
    }
}
