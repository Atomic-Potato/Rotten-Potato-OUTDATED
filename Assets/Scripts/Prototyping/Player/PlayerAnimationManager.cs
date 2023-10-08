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

    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource audioStepHigh;
    [SerializeField] AudioSource audioStepLow;

    [Space]
    [Header("Other")]
    [Range(0f, 1f)]
    [SerializeField] float noDashesAlphaValue = 0.75f;
    [Range(0f,10f)] 
    [SerializeField] float timeBetweenRecoveryFlashes;
    [SerializeField] SpriteRenderer spriteRenderer;
    #endregion

    #region Global Variables
    static PlayerAnimationManager _instance;
    public static PlayerAnimationManager Instance => _instance;
    AnimationClip clipCurrent;
    public AnimationClip CurrentClip => clipCurrent;
    public Action<AnimationClip, bool, float> AnimationStateAction;
    Coroutine _waitForAnimationCache;
    Coroutine _recoveryFlashCache;
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
        #region Animation States Handeling
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
        #endregion

        #region Other
        if (pDash.DashesLeft <= 0)
        {
            SetAlpha(noDashesAlphaValue);
        }
        else
        {
            SetAlpha(1f);
        }

        if (pPlayer.IsInRecovery)
        {
            RecoveryFalsh();
        }
        else
        {
            if (_recoveryFlashCache != null)
            {
                StopCoroutine(_recoveryFlashCache);
                _recoveryFlashCache = null;
                if (spriteRenderer.enabled == false)
                {
                    spriteRenderer.enabled = true;
                }
            }
        }
        #endregion
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

    #region Animation Events
    void Event_PlayStepAudioHigh()
    {
        AudioManager.PlayAudioSource(audioStepHigh);
    }

    void Event_PlayStepAudioLow()
    {
        AudioManager.PlayAudioSource(audioStepLow);
    }
    #endregion

    #region Other Methods
    void SetAlpha(float a)
    {
        if (spriteRenderer.color.a != a)
        {
            Color color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
            spriteRenderer.color = color;
        }
    }

    void RecoveryFalsh()
    {
        if (_recoveryFlashCache == null)
        {
            _recoveryFlashCache = StartCoroutine(ExecuteRecoveryFlash());
        }

        IEnumerator ExecuteRecoveryFlash()
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(timeBetweenRecoveryFlashes);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(timeBetweenRecoveryFlashes);
            _recoveryFlashCache = null;
        }
    }
    #endregion
}
