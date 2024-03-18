using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private Gun _gun;
    [SerializeField] private Health _health;

    // Start is called before the first frame update
    void Start()
    {
        _healthText.text = "Health => 100";
        _ammoText.text = "Ammo => 100";
    }

    // Update is called once per frame
    void Update()
    {
        _healthText.text = $"Health => {_health.CurrentHealth}";
        _ammoText.text = $"Ammo => {_gun.CurrentAmmo}";
    }
}