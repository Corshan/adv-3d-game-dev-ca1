using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<GameObject> _collectables;
    // Start is called before the first frame update
    void Start()
    {
        _collectables = GameObject.FindGameObjectsWithTag("Collectable").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        _collectables = GameObject.FindGameObjectsWithTag("Collectable").ToList();
    }

    public int GetCollectableTotal => 10-_collectables.Count;
}
