using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    [SerializeField] private GameObject _verticalWall, _horizontalWall, _player, _npc;

    void Start()
    {
        GenerateFromFile();
    }

    void GenerateFromFile()
    {
        TextAsset t1 = (TextAsset)Resources.Load("maze", typeof(TextAsset));
        string map = t1.text.Replace(System.Environment.NewLine, "");

        for (int i = 0; i < map.Length; i++)
        {
            int column, row;
            column = i % 10;
            row = i / 10;

            if (map[i] == '1') Instantiate(_verticalWall, new Vector3(50 - column * 10, 1.5f, 50 - row * 10), Quaternion.identity);
            // else if (map[i] == '2') Instantiate(_npc, new Vector3(50 - column * 10, 1.5f, 50 - row * 10), Quaternion.identity);
        }
    }
}
