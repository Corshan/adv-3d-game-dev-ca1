using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    [SerializeField] private GameObject _wall, _ground, _player, _playerTeamMember, _npcLeader;
    [SerializeField] private GameObject _ammoPoint, _healthPoint, _collectable, _waypoint;
    [SerializeField][Range(10, 50)] private float _offset = 10;
    [SerializeField][Range(2, 10)] private int _teamMemberNo = 5;
    void Start()
    {
        GenerateFromFile();
    }

    void GenerateFromFile()
    {
        TextAsset t1 = (TextAsset)Resources.Load("maze", typeof(TextAsset));
        string map = t1.text.Replace(System.Environment.NewLine, "");

        GameObject go = Instantiate(_ground, new Vector3(0, 0, 0), Quaternion.identity);

        go.transform.localScale = new Vector3(_offset * 10, 1, _offset * 10);
        go.transform.position = new Vector3((_offset * 5) - _offset / 2, 0, (_offset * 5) - _offset / 2);

        Vector3 playerPos = new();
        Vector3 leaderPos = new();

        for (int i = 0; i < map.Length; i++)
        {
            int column, row;
            column = i % 10;
            row = i / 10;

            if (map[i] == '1')
            {
                Instantiate(_wall, new Vector3(column * _offset, 2.5f, row * _offset), Quaternion.identity)
                                    .transform.localScale = new Vector3(_offset, 5, _offset);
            }
            else if (map[i] == '2')
            {
                playerPos = new Vector3(column * _offset, 2.5f, row * _offset);
            }
            else if (map[i] == '3')
            {
                Instantiate(_ammoPoint, new Vector3(column * _offset, 2.5f, row * _offset), Quaternion.identity);
            }
            else if (map[i] == '4')
            {
                Instantiate(_healthPoint, new Vector3(column * _offset, 2.5f, row * _offset), Quaternion.identity);
            }
            else if (map[i] == '5')
            {
                leaderPos = new Vector3(column * _offset, 0, row * _offset);
            }
            else if (map[i] == '6')
            {
                Instantiate(_collectable, new Vector3(column * _offset, 2.5f, row * _offset), Quaternion.identity);
            }
            else if (map[i] == '7')
            {
                Instantiate(_waypoint, new Vector3(column * _offset, 2.5f, row * _offset), Quaternion.identity);
            }
        }

        go.GetComponent<NavMeshSurface>().BuildNavMesh();

        Instantiate(_player, playerPos, Quaternion.identity);
        Instantiate(_npcLeader, leaderPos, Quaternion.identity);
    }
}
