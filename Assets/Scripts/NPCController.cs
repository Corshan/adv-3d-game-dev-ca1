using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public Type NpcType => _NPCType;
    [SerializeField] private Type _NPCType;
    [SerializeField][Range(10, 50)] private float _castingDistance = 20;
    [Header("Type 1 & 2 - Settings")]
    [SerializeField][Range(2, 10)] private float _distanceToWaypoints = 2;
    [SerializeField][Range(0, 1)] private float _ammoPercentage = 0.2f;
    [SerializeField][Range(0, 1)] private float _healthPercentage = 0.2f;
    [SerializeField][Range(1, 10)] private float _fleeDistance = 2;
    [Header("Type 4 - Settings")]
    [SerializeField][Range(1, 20)] private float _fireDistance = 3;
    [SerializeField] private Transform _ambushSpot;
    private Gun _gun;
    private float _gunTimer;
    private Animator _anim;
    private NavMeshAgent _agent;
    private AnimatorStateInfo _animInfo;
    private int _currentIndex;
    private GameObject _target;
    private GameObject _player;
    private Vector3 _direction;
    private GameObject[] _allBCs;
    private bool _isDead = false;
    private Health _health;
    private bool _isAmmoPacks;
    private bool _isHealthPacks;
    private Vector3 _startingPos;
    private List<GameObject> _waypoints;

    public enum Type
    {
        NONE,
        TYPE_1_PATROLLER,
        TYPE_2_INTELLIGENT_PATROLLER,
        TYPE_3_HUNTER,
        TYPE_4_SNIPER,
        TYPE_5_TEAM,
    }
    // Start is called before the first frame update
    void Start()
    {
        _waypoints = GameObject.FindGameObjectsWithTag("Waypoint").ToList();
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _animInfo = _anim.GetCurrentAnimatorStateInfo(0);
        _currentIndex = (_NPCType == Type.TYPE_1_PATROLLER) ? 0 : Random.Range(0, _waypoints.Count);
        _player = GameObject.FindGameObjectWithTag("Player");
        _gun = GetComponent<Gun>();
        _health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        _animInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (_animInfo.IsName("Death") || _isDead)
        {
            _isDead = true;
            _agent.isStopped = true;
            return;
        }

        switch (_NPCType)
        {
            case Type.TYPE_1_PATROLLER:
                {
                    TypeOne();
                    break;
                }
            case Type.TYPE_2_INTELLIGENT_PATROLLER:
                {
                    TypeTwo();
                    break;
                }
            case Type.TYPE_3_HUNTER:
                {
                    TypeThree();
                    break;
                }
            case Type.TYPE_4_SNIPER:
                {
                    TypeFour();
                    break;
                }
        }

        _anim.SetBool("isPatrolling", true);
    }

    private void TypeOne()
    {
        Senses();

        if (_animInfo.IsName("Follow Player") || _animInfo.IsName("Fire Gun")) FireGun();
        else FollowWaypoints();
    }

    private void TypeTwo()
    {
        Senses();

        if (_animInfo.IsName("Follow Player") || _animInfo.IsName("Fire Gun")) FireGun();
        else if (_animInfo.IsName("Patrolling")) FollowRandomWaypoints();
        else if (_animInfo.IsName("Find Ammo") && _isAmmoPacks) FindAmmoPack();
        else if (_animInfo.IsName("Find Health Pack") && _isHealthPacks) FindhealthPack();
        else if (_animInfo.IsName("Flee")) Flee();

        CheckAmmo();
        CheckHealth();

        if (Vector3.Distance(transform.position, _player.transform.position) > _fleeDistance) _anim.SetBool("isFleeing", false);
    }

    private void TypeThree()
    {
        if (_animInfo.IsName("Follow Player")) FireGun();
        else if (_animInfo.IsName("Find Ammo") && _isAmmoPacks) FindAmmoPack();
        else if (_animInfo.IsName("Find Health Pack") && _isHealthPacks) FindhealthPack();
        else if (_animInfo.IsName("Flee")) Flee();

        CheckAmmo();
        CheckHealth();

        if (Vector3.Distance(transform.position, _player.transform.position) > _fleeDistance) _anim.SetBool("isFleeing", false);
    }

    private void TypeFour()
    {
        Look();

        if (_animInfo.IsName("Fire Gun") || _animInfo.IsName("Player In Range")) FireGun(sniper: true);
        else if (_animInfo.IsName("Find Ammo") && _isAmmoPacks) FindAmmoPack();
        else if (_animInfo.IsName("Find Health Pack") && _isHealthPacks) FindhealthPack();
        else if (_animInfo.IsName("Flee")) Flee();
        else if (_animInfo.IsName("Go To Ambush")) GoToAmbushSpot();
        else if (_animInfo.IsName("Back to Start Point")) GotoStartPos();

        CheckAmmo();
        CheckHealth();

        if (Vector3.Distance(transform.position, _player.transform.position) > _fleeDistance) _anim.SetBool("isFleeing", false);
        else _anim.SetBool("isFleeing", true);


        if (Vector3.Distance(transform.position, _player.transform.position) < _fireDistance) _anim.SetBool("isInRange", true);
        else _anim.SetBool("isInRange", false);
    }

    public void TriggerAmbush()
    {
        _startingPos = transform.position;
        _anim.SetTrigger("ambush");
    }

    private void GoToAmbushSpot()
    {
        _agent.SetDestination(_ambushSpot.position);
        if (Vector3.Distance(transform.position, _ambushSpot.position) <= 2) _anim.SetBool("throwGenade", true);
    }

    private void GotoStartPos()
    {
        _agent.SetDestination(_startingPos);
        if (Vector3.Distance(transform.position, _startingPos) <= 2) _anim.SetBool("isAtStartingPoint", true);
    }

    private void Flee()
    {
        _anim.SetBool("isFleeing", true);

        Vector3 fleeDirection = (this.transform.position - _player.transform.position).normalized;
        Vector3 newGoal = this.transform.position + fleeDirection * _fleeDistance / 2;

        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(newGoal, path);

        if (path.status != NavMeshPathStatus.PathInvalid) _agent.SetDestination(path.corners[path.corners.Length - 1]);

        _anim.SetBool("isHit", false);
    }

    private void FireGun(bool sniper = false)
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (_gunTimer >= 3 && _gun.HasAmmo() && distance <= _fireDistance)
        {
            _gun.Fire();
            _gunTimer = 0;
            _anim.SetBool("isFiring", true);
            _agent.isStopped = true;

            if (sniper) transform.LookAt(_player.transform.position);
        }
        else
        {
            if (!sniper) FollowPlayer();
            _anim.SetBool("isFiring", false);
            _agent.isStopped = false;
        }

        _gunTimer += Time.deltaTime;
    }

    private void CheckAmmo()
    {
        float percent = _gun.CurrentAmmo / 100f * _gun.MaxAmmo;

        if (percent <= _ammoPercentage)
        {
            _anim.SetBool("ammoLow", true);

            GameObject[] ammoPacks = GameObject.FindGameObjectsWithTag("Ammo");
            _isAmmoPacks = ammoPacks.Length > 0;

            if (_isAmmoPacks) _anim.SetBool("ammoPackAvaliable", true);
            else _anim.SetBool("ammoPackAvaliable", false);
        }
        else _anim.SetBool("ammoLow", false);
    }

    private void CheckHealth()
    {
        float percent = (_health.MaxHealth == 100) ? _health.CurrentHealth / 100f : _health.CurrentHealth / 100f * _health.MaxHealth;
        // Debug.Log(percent);

        if (percent <= _healthPercentage)
        {
            _anim.SetBool("healthLow", true);

            GameObject[] healthPacks = GameObject.FindGameObjectsWithTag("HealthPack");
            _isHealthPacks = healthPacks.Length > 0;

            if (_isHealthPacks) _anim.SetBool("healthPackAvaliable", true);
            else _anim.SetBool("healthPackAvaliable", false);
        }
        else _anim.SetBool("healthLow", false);
    }

    private void FollowWaypoints()
    {
        _target = _waypoints[_currentIndex];

        if (Vector3.Distance(transform.position, _target.transform.position) < _distanceToWaypoints)
        {
            _currentIndex++;

            if (_currentIndex > _waypoints.Count - 1) _currentIndex = 0;
            // Debug.Log($"{name}   {_currentIndex}");
        }

        _agent.SetDestination(_target.transform.position);
    }

    private void FollowRandomWaypoints()
    {
        _target = _waypoints[_currentIndex];

        if (Vector3.Distance(transform.position, _target.transform.position) < _distanceToWaypoints)
        {
            _currentIndex = Random.Range(0, _waypoints.Count);
            _target = _waypoints[_currentIndex];
            Debug.Log($"{name}   {_currentIndex}");
        }

        _agent.stoppingDistance = 0;
        _agent.SetDestination(_target.transform.position);
    }

    private void FindAmmoPack()
    {
        GameObject[] ammoPacks = GameObject.FindGameObjectsWithTag("Ammo");

        _isAmmoPacks = ammoPacks.Length > 0;

        _target = FindCloset(ammoPacks);

        if (_target != null) _agent.SetDestination(_target.transform.position);
        _agent.stoppingDistance = 0;
    }

    private void FindhealthPack()
    {
        GameObject[] healthPacks = GameObject.FindGameObjectsWithTag("HealthPack");

        _isHealthPacks = healthPacks.Length > 0;

        _target = FindCloset(healthPacks);

        if (_target != null) _agent.SetDestination(_target.transform.position);
        _agent.stoppingDistance = 0;
    }

    private GameObject FindCloset(GameObject[] items)
    {
        float closet = float.MaxValue;
        GameObject closetGo = null;

        foreach (var go in items)
        {
            float distance = Vector3.Distance(go.transform.position, transform.position);

            if (distance < closet)
            {
                closet = distance;
                closetGo = go;
            }
        }

        return closetGo;
    }

    private void FollowPlayer()
    {
        _agent.stoppingDistance = 2;
        _agent.SetDestination(_player.transform.position);
    }

    private void Senses()
    {
        Look();
        SmellBreadCrumb();
        Listen();
    }

    private void Look()
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
            if (isInFieldOfView && hit.collider.CompareTag("Player")) _anim.SetBool("canSeePlayer", true);
            else _anim.SetBool("canSeePlayer", false);
        }
        else _anim.SetBool("canSeePlayer", false);
    }

    private void SmellBreadCrumb()
    {

        _allBCs = GameObject.FindGameObjectsWithTag("BC");
        float minDistance = 2;
        bool detectedBC = false;

        for (int i = 0; i < _allBCs.Length - 1; i++)
        {
            if (Vector3.Distance(transform.position, _allBCs[i].transform.position) < minDistance)
            {
                detectedBC = true;
                break;
            }
        }

        if (detectedBC) _anim.SetBool("canSmellPlayer", true);
        else _anim.SetBool("canSmellPlayer", false);
    }

    private void Listen()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance < 3) _anim.SetBool("canHearPlayer", true);
        else _anim.SetBool("canHearPlayer", false);
    }
}
