using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAmbush : MonoBehaviour
{
    [SerializeField] private NPCController _npc;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Ambush");
        if(!other.CompareTag("Player")) return;

        _npc.TriggerAmbush();
    }
}
