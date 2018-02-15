using UnityEngine;

public class DoorAnimatorClosing : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<DoorController>().PlayCloseSound();
    }
}
