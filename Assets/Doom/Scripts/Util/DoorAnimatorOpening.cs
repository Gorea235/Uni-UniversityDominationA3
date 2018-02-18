using UnityEngine;

public class DoorAnimatorOpening : StateMachineBehaviour
{
    /// <summary>
    /// Fired when the animator enters the opening state.
    /// This means that the opening animation is just about to start playing.
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<DoorController>().PlayOpenSound();
    }
}
