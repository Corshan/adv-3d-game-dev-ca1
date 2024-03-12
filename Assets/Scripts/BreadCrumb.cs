using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadCrumb : MonoBehaviour
{
    private Vector3 _prevPos;
    private float _counter = 0;
    [SerializeField] private float _distanceToBc = 1;
    [SerializeField] private float _timeToDestroy = 3;
    [SerializeField] private GameObject _breadCrumbPrefab;

    // Update is called once per frame
    void Update()
    {
        DropBreadCrumb();
    }

    private void DropBreadCrumb()
    {
        float distance = Vector3.Distance(_prevPos, transform.position);

        if (distance > _distanceToBc)
        {
            _prevPos = transform.position;

            GameObject go = Instantiate(_breadCrumbPrefab, transform.position, Quaternion.identity);
            go.name = $"BC {_counter}";
            _counter++;

            Destroy(go, _timeToDestroy);
        }
    }
}
