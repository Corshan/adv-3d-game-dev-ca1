using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Type _NPCType;
    [SerializeField] private float _castingDistance = 20;
    [Header("Type 1")]
    [SerializeField] private List<GameObject> _waypoints;
    [SerializeField] private float _distanceToWaypoints;
    private Animator _anim;
    private NavMeshAgent _agent;
    private AnimatorStateInfo _animInfo;
    private int _currentIndex;
    private GameObject _target;
    private GameObject _player;
    private Vector3 _direction;
    enum Type
    {
        NONE,
        TYPE_1_PATROLLER
    }
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _animInfo = _anim.GetCurrentAnimatorStateInfo(0);
        _currentIndex = 0;
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        _animInfo = _anim.GetCurrentAnimatorStateInfo(0);

        switch (_NPCType)
        {
            case Type.TYPE_1_PATROLLER:
                {
                    TypeOne();
                    Look();
                    break;
                }
        }
    }

    private void TypeOne()
    {
        if (_animInfo.IsName("Follow Player")) FollowPlayer();
        else FollowWaypoints();

         _anim.SetBool("isWalking", true);
    }

    private void FollowWaypoints()
    {
        _target = _waypoints[_currentIndex];

        if (Vector3.Distance(transform.position, _target.transform.position) < _distanceToWaypoints)
        {
            _currentIndex++;

            if (_currentIndex > _waypoints.Count - 1) _currentIndex = 0;
        }

        _agent.SetDestination(_target.transform.position);
    }

    private void FollowPlayer()
    {
        _agent.SetDestination(_player.transform.position);
    }

    void Look()
    {
        _direction = (_player.transform.position - transform.position).normalized;
        bool isInFieldOfView = Vector3.Dot(transform.forward.normalized, _direction) > 0.7f;

        Debug.DrawRay(transform.position, _direction * _castingDistance, Color.green);
        Debug.DrawRay(transform.position, transform.forward * _castingDistance, Color.blue);

        Debug.DrawRay(transform.position, (transform.forward - transform.right) * _castingDistance, Color.red);
        Debug.DrawRay(transform.position, (transform.forward + transform.right) * _castingDistance, Color.red);

        Ray ray = new Ray();
        RaycastHit hit;

        ray.origin = transform.position + Vector3.up * 0.7f;

        ray.direction = transform.forward * _castingDistance;
        Debug.DrawRay(ray.origin, ray.direction * _castingDistance, Color.red);

        if (Physics.Raycast(ray.origin, _direction, out hit, _castingDistance) && Vector3.Distance(transform.position, _player.transform.position) < _castingDistance)
        {
            if (isInFieldOfView) _anim.SetBool("canSeePlayer", true);
            else _anim.SetBool("canSeePlayer", false);
        }
    }
}
