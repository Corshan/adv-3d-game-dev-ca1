using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<GameObject> _collectables;
    public int GetCollectableTotal => 10 - _collectables.Count;
    private float _timer = 180;
    public float time => _timer;
    private Health _playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Main Menu"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            _collectables = GameObject.FindGameObjectsWithTag("Collectable").ToList();
            _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (SceneManager.GetActiveScene().name.Equals("Main Menu")) return;

        _collectables = GameObject.FindGameObjectsWithTag("Collectable").ToList();
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            SceneManager.LoadScene("Main Menu");
        }

        if (_collectables.Count == 0)
        {
            if (SceneManager.GetActiveScene().name.Equals("Level 1")) SceneManager.LoadScene("Level 2");
            else SceneManager.LoadScene("Main Menu");
        }

        if (_playerHealth != null && _playerHealth.IsDead()) SceneManager.LoadScene("Main Menu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

}
