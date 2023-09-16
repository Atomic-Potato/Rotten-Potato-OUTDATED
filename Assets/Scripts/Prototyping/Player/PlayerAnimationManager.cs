using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Space]
    [Header("Animation Clips")]
    [Tooltip("If no condition is met, then this clip will be played.")]
    [SerializeField] AnimationClip defaultClip;
    [Space]
    [SerializeField] AnimationClip clipIdle;
    [SerializeField] AnimationClip clipRun;

    AnimationClip clipCurrent;

    void Update()
    {
        if (IsRunning())
        {
            SetAnimationState(clipRun);
        }
        else if (IsIdle())
        {
            SetAnimationState(clipIdle);
        }
        else
        {
            SetAnimationState(defaultClip);
        }
    }

    void SetAnimationState(AnimationClip clip)
    {
        if (clip == clipCurrent)
        {
            return;
        }

        animator.Play(clip.name);
        clipCurrent = clip;
    }

    bool IsRunning()
    {
        return 
            BasicMovement.IS_GROUNDED && 
            BasicMovement.IsMovementActive &&
            PlayerInputManager.Direction.x != 0f;
    }

    bool IsIdle()
    {
        return 
            BasicMovement.IS_GROUNDED && 
            BasicMovement.IsMovementActive &&
            PlayerInputManager.Direction.x == 0f;
    }
}
