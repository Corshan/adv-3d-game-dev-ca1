using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderAttack : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject.Find("NPC (Leader)").GetComponent<Leader>().Attack();
    }
}
