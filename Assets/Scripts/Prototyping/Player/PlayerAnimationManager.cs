using System;
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

    static PlayerAnimationManager _instance;
    public static PlayerAnimationManager Instance => _instance;

    AnimationClip clipCurrent;
    public AnimationClip CurrentClip => clipCurrent;

    public Action<AnimationClip> AnimationStateAction;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;    
        }

        AnimationStateAction = SetAnimationState;
    }

    void Update()
    {
        if (IsRunning())
        {
            AnimationStateAction?.Invoke(clipRun);
        }
        else if (IsIdle())
        {
            AnimationStateAction?.Invoke(clipIdle);
        }
        else
        {
            AnimationStateAction?.Invoke(defaultClip);
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
