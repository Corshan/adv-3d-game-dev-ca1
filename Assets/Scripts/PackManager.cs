using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private string _spawnPointTag;
    [SerializeField] private string _packTag;
    private List<GameObject> _spawnPoints;
    private List<GameObject> _packs;

    // Start is called before the first frame update
    void Start()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag(_spawnPointTag).ToList();

        foreach (var point in _spawnPoints)
        {
            Instantiate(_prefab, point.transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _packs = GameObject.FindGameObjectsWithTag(_packTag).ToList();

        if (_packs.Count <= 0) SpawnPacks();
    }

    private void SpawnPacks()
    {
        foreach (var point in _spawnPoints)
        {
            Instantiate(_prefab, point.transform.position, Quaternion.identity);
        }
    }
}
