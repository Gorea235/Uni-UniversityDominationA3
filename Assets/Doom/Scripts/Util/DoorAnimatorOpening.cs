using UnityEngine;

public class DoorAnimatorOpening : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<DoorController>().PlayOpenSound();
    }
}
