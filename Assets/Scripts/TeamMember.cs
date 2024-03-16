using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamMember : MonoBehaviour
{
    [SerializeField] private GameObject _leader;
    [SerializeField][Range(1, 5)] private float _distanceToLeader = 5;
    private Animator _anim;
    private AnimatorStateInfo _animInfo;
    private NavMeshAgent _agent;
    private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        _animInfo = _anim.GetCurrentAnimatorStateInfo(0);

        float distance = Vector3.Distance(transform.position, _leader.transform.position);

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
                _agent.isStopped = false;
                _agent.SetDestination(_target.transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
                if (distanceToTarget < 2.25f)
                {
                    _anim.SetBool("closeToTarget", true);
                    _agent.isStopped = true;
                }
                else
                {
                    _anim.SetBool("closeToTarget", false);
                }
            }
            else
            {
                _anim.SetBool("targetDestroyed", true);
            }
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
}
