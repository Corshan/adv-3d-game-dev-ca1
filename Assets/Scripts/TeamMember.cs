using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TeamMember : MonoBehaviour
{
    public GameObject _leader;
    [SerializeField][Range(1, 5)] private float _distanceToLeader = 5;
    [SerializeField][Range(1, 5)] private int _damageAmount = 10;
    private Animator _anim;
    private AnimatorStateInfo _animInfo;
    private NavMeshAgent _agent;
    private GameObject _target;
    private GameObject _wanderingTarget;
    private float _timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _wanderingTarget = new GameObject();
        
        if(_leader == null){
            if(gameObject.CompareTag("NpcTeam")) _leader = GameObject.FindGameObjectWithTag("Leader");
            else _leader = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _animInfo = _anim.GetCurrentAnimatorStateInfo(0);

        float distance = Vector3.Distance(transform.position, _leader.transform.position);

        if (_animInfo.IsName("Wander")) Wander();

        if (distance < _distanceToLeader) _anim.SetBool("closeToLeader", true);
        else _anim.SetBool("closeToLeader", false);

        if (_animInfo.IsName("Idle")) _agent.isStopped = true;

        if (_animInfo.IsName("MoveTowardsLeader"))
        {
            _agent.isStopped = false;
            _agent.SetDestination(_leader.transform.position);
        }

        if (_animInfo.IsName("GoToTarget"))
        {
            if (_target != null)
            {
                CloseToTarget();
            }
            else
            {
                _anim.SetBool("targetDestroyed", true);
            }
        }

        if (_animInfo.IsName("AttackTarget"))
        {
            if (_target != null)
            {
                _agent.isStopped = true;
                transform.LookAt(new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z));
                if (_animInfo.normalizedTime % 1f >= .98f)
                {
                    var npc = _target.GetComponent<Health>();
                    npc.UpdateHealth(_damageAmount);

                    if (npc.CurrentHealth <= 0) NewTarget();
                }
            }
            else
            {
                _anim.SetBool("targetDestroyed", true);
            }
        }
    }

    private void Wander()
    {
        _timer += Time.deltaTime;

        if (_timer > 4)
        {
            Debug.Log($"{_timer}  {gameObject.name}");
            _timer = 0;
            RaycastHit hit;
            Ray ray = new Ray();
            ray.origin = transform.position + Vector3.up * 0.7f;
            float distanceToOb = 0;
            float castingDistance = 20;
            do
            {
                float randomdirX = Random.Range(-0.5f, 0.5f);
                float randomdirZ = Random.Range(-0.5f, 0.5f);

                ray.direction = transform.forward * randomdirZ + transform.right * randomdirX;
                Debug.DrawRay(ray.origin, ray.direction, Color.red);

                if (Physics.Raycast(ray.origin, ray.direction, out hit, castingDistance))
                {
                    distanceToOb = hit.distance;
                }
                else distanceToOb = castingDistance;

                _wanderingTarget.transform.position = ray.origin + ray.direction * (distanceToOb - 4);
                _target = _wanderingTarget;

            } while (distanceToOb < 1.0f);
            Debug.Log($"{gameObject.name}  Done");
        }

        _agent.SetDestination(_target.transform.position);
    }

    private void NewTarget()
    {
        string targetTag = gameObject.CompareTag("NpcTeam") ? "PlayerTeam" : "NpcTeam";

        var allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        var aliveTargets = new List<GameObject>();

        allTargets.ForEach(t => {
            if(!t.GetComponent<Health>().IsDead()) aliveTargets.Add(t);
        });

        int random = Random.Range(0, aliveTargets.Count);

        _target = aliveTargets.Count == 0 ? null : aliveTargets[random];
    }

    private bool CloseToTarget()
    {
        // if(_target == null) return false;
        _agent.isStopped = false;
        _agent.SetDestination(_target.transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        if (distanceToTarget < 2.25f)
        {
            _anim.SetBool("closeToTarget", true);
            _agent.isStopped = true;
            return true;
        }
        else
        {
            _anim.SetBool("closeToTarget", false);
            return false;
        }
    }

    public void Attack(GameObject target)
    {
        _target = target;
        _anim.SetTrigger("respondToAttack");
    }

    public void Retreat()
    {
        _anim.SetTrigger("retreat");
    }

    public void LeaderDead()
    {
        _anim.SetTrigger("leaderDead");
    }
}
