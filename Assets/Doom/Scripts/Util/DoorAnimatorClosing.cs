using UnityEngine;

public class DoorAnimatorClosing : StateMachineBehaviour
{
    /// <summary>
    /// Fired when the animator enters the closing state.
    /// This means that the closing animation is just about to start playing.
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<DoorController>().PlayCloseSound();
    }
}
