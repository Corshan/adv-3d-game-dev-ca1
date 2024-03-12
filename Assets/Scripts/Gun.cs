using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField][Range(10f, 100f)] private float _bulletForce;
    [SerializeField] private Transform _guntip;
    [SerializeField][Range(1,10)] private float _bulletTimer = 3;
    [SerializeField][Range(1,100)] private int _maxAmmo = 10;
    [SerializeField] private Type _type;
    private int _counter;
    private int _currentAmmo;

    private enum Type {
        NONE,
        PLAYER,
        NPC
    }

    private void Start()
    {
        _currentAmmo = _maxAmmo;
    }

    public void Fire()
    {
        // Debug.Log(_currentAmmo);
        if (_currentAmmo <= 0) return;

        GameObject go = Instantiate(_bulletPrefab, _guntip.position, _guntip.rotation);

        go.name = $"Bullet {_counter}";
        _counter++;

        Rigidbody rb = go.GetComponent<Rigidbody>();

        rb.AddForce(_guntip.forward * _bulletForce, ForceMode.Impulse);

        Destroy(go, _bulletTimer);
        _currentAmmo--;
    }

    public bool HasAmmo() => _currentAmmo > 0;

}
