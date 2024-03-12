using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField][Range(10, 100)] private int _maxhealth = 100;
    [SerializeField][Range(10, 100)] private int _damageAmount = 10;
    private int _currentHealth;
    private Animator _anim;
    private float _timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxhealth;
    }

    // void Update()
    // {
    //     _timer += Time.deltaTime;

    //     if (_timer >= 1)
    //     {
    //         UpdateHealth(_damageAmount);
    //         Debug.Log(_currentHealth);
    //         _timer = 0;
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet")) return;

        UpdateHealth(_damageAmount);
    }

    private void UpdateHealth(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        Debug.Log(_currentHealth);

        if (_currentHealth <= 0) _anim.SetTrigger("dead");
    }
}
