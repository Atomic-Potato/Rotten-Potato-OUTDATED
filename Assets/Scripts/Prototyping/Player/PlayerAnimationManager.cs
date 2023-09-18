using System;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] Animator animator;

    [Space]
    [Header("Animation Clips")]
    [Tooltip("If no condition is met, then this clip will be played.")]
    [SerializeField] AnimationClip defaultClip;
    [Space]
    [SerializeField] AnimationClip clipIdle;
    [SerializeField] AnimationClip clipRun;
    [SerializeField] AnimationClip clipJump;
    [SerializeField] AnimationClip clipFall;
    [SerializeField] AnimationClip clipDash;
    [SerializeField] AnimationClip clipDamageDash;
    [SerializeField] AnimationClip clipAirHold;
    #endregion

    #region Global Variables
    static PlayerAnimationManager _instance;
    public static PlayerAnimationManager Instance => _instance;

    AnimationClip clipCurrent;
    public AnimationClip CurrentClip => clipCurrent;

    public Action<AnimationClip> AnimationStateAction;
    #endregion

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
        AnimationClip nextClip = null;
        if (IsRunning())
        {
            nextClip = clipRun;
        }
        else if (IsIdle())
        {
            nextClip = clipIdle;
        }
        else if (IsJumping())
        {
            nextClip = clipJump;
        }
        else if (IsFalling())
        {
            nextClip = clipFall;
        }
        else if (pDash.IsDashing && !pDash.IsDamagedDashing)
        {
            nextClip = clipDash;
        }
        else if (pDash.IsDamagedDashing)
        {
            nextClip = clipDamageDash;
        }
        else if (pDash.IsHolding)
        {
            nextClip = clipAirHold;
        }
        else
        {
            nextClip = defaultClip;
        }

        AnimationStateAction?.Invoke(nextClip);
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

    #region States Conditions
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

    bool IsJumping()
    {
        return 
            !BasicMovement.IS_GROUNDED &&
            pPlayer.Instance.Rigidbody.velocity.y > 0f;
    }

    bool IsFalling()
    {
        return 
            !BasicMovement.IS_GROUNDED &&
            pPlayer.Instance.Rigidbody.velocity.y < 0f;
    }
    #endregion
}
