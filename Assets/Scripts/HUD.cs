using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _collectableText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Gun _gun;
    [SerializeField] private Health _health;
    [SerializeField] private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        if(_gameManager == null) _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _healthText.text = "Health => 100";
        _ammoText.text = "Ammo => 100";
        _timeText.text = "Time: 3:00";
    }

    // Update is called once per frame
    void Update()
    {
        _healthText.text = $"Health => {_health.CurrentHealth}";
        _ammoText.text = $"Ammo => {_gun.CurrentAmmo}";
        _collectableText.text = $"Items Collected => {_gameManager.GetCollectableTotal}/10";
        
        string time = TimeSpan.FromSeconds(_gameManager.time).ToString("mm\\:ss");
        _timeText.text = $"Time: {time}";
    }
}
