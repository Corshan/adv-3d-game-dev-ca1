using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField][Range(10, 100)] private int _maxHealth = 100;
    [SerializeField][Range(10, 100)] private int _damageAmount = 10;
    [SerializeField][Range(10, 20)] private int _healthAmount = 10;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Type _type;
    private int _currentHealth;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    private Animator _anim;
    private NPCController _controller;

    private enum Type
    {
        NPC,
        PLAYER,
    }

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _controller = GetComponent<NPCController>();

        if(_image != null) _image.fillAmount = 1;
    }

    void Update()
    {
       if(_canvas != null) _canvas.SetActive(_currentHealth > 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"{name}  {other.tag}  {other.name}");

        if (other.CompareTag("Bullet") && _type == Type.NPC)
        {

            if (_controller != null && _controller.NpcType == NPCController.Type.TYPE_2_INTELLIGENT_PATROLLER) _anim.SetBool("isHit", true);

            UpdateHealth(_damageAmount);
        }
        else if (other.CompareTag("BulletNpc") && _type == Type.PLAYER)
        {
            UpdateHealth(_damageAmount);
        }
        else if (other.CompareTag("HealthPack") && _currentHealth < _maxHealth)
        {
            _currentHealth = ((_currentHealth + _healthAmount) < _maxHealth) ? _currentHealth + _healthAmount : _maxHealth;
            Destroy(other.gameObject);
        }
    }

    public void UpdateHealth(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        Debug.Log($"{name}   {_currentHealth}");

        if (_anim != null && _currentHealth <= 0) _anim.SetTrigger("dead");

        if (_image != null)
        {
            _image.fillAmount = (float)_currentHealth / (float)_maxHealth;
        }
    }

    public bool IsDead() => _currentHealth <= 0;
}
