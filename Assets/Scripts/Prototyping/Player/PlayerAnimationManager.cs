using System;
using System.Collections;
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
    [SerializeField] AnimationClip clipIdleSpecial;
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

    public Action<AnimationClip, bool, float> AnimationStateAction;

    Coroutine _waitForAnimationCache;
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
        if (IsRunning())
        {
            AnimationStateAction?.Invoke(clipRun, false, -1f);
        }
        else if (IsIdle())
        {
            AnimationStateAction?.Invoke(clipIdle, false, -1f);
        }
        else if (pDash.IsDashing && !pDash.IsDamagedDashing)
        {
            AnimationStateAction?.Invoke(clipDash, false, -1f);
        }
        else if (pDash.IsDamagedDashing)
        {
            AnimationStateAction?.Invoke(clipDamageDash, false, -1f);
        }
        else if (pDash.IsHolding)
        {
            AnimationStateAction?.Invoke(clipAirHold, false, -1f);
        }
        else if (IsJumping())
        {
            AnimationStateAction?.Invoke(clipJump, false, -1f);
        }
        else if (IsFalling())
        {
            AnimationStateAction?.Invoke(clipFall, false, -1f);
        }
        else
        {
            AnimationStateAction?.Invoke(defaultClip, false, -1f);
        }

        if (PlayerInputManager.IsPerformedTheFunny)
        {
            AnimationStateAction?.Invoke(clipIdleSpecial, true, clipIdleSpecial.length);
        }

    }

    /// <summary>
    /// Switches the current animation clip.
    /// </summary>
    /// <param name="clip">The clip to be played</param>
    /// <param name="length">
    /// If not ommited, 
    /// the function will not swittch states
    /// until length time passes.
    /// </param>
    /// <param name="endCurrent" >
    /// If current is forced to be played till end
    /// then this will overide it and replace it with a new clip
    /// </param>
    void SetAnimationState(AnimationClip clip, bool endCurrent = false, float length = -1f)
    {
        if (clip == clipCurrent)
        {
            return;
        }

        if (endCurrent)
        {
            if (_waitForAnimationCache != null)
            {
                StopCoroutine(_waitForAnimationCache);
                _waitForAnimationCache = null;
            }
        }
        
        if (_waitForAnimationCache == null)
        {
            if (length > 0f)
            {
                animator.Play(clip.name);
                clipCurrent = clip;
                _waitForAnimationCache = StartCoroutine(WaitForAnimation());
            }
            else
            {
                animator.Play(clip.name);
                clipCurrent = clip;
            }
        }

        IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(length);
            _waitForAnimationCache = null;
        }
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
