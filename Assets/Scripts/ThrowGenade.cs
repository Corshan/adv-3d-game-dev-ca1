using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGenade : StateMachineBehaviour
{
    public GameObject grenade;
    GameObject clone;
    Rigidbody r;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.LookAt(GameObject.Find("Player").transform.position);
        clone = Instantiate(grenade, animator.gameObject.transform.Find("Guntip").transform.position, Quaternion.identity);
        r = clone.GetComponent<Rigidbody>();
        r.AddForce(animator.gameObject.transform.forward * 10, ForceMode.Impulse);
    }
}
