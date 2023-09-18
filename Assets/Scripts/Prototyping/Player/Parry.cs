using System.Collections;
using UnityEngine;

public class Parry : MonoBehaviour
{
    #region Inspector 
    [Range(0f, 1f)]
    [SerializeField] float timeWindowForSpam = 0.2f;
    [Tooltip("An object can be parried once inside this collider.")]
    [SerializeField] Collider2D parryCollisionRange;
    [Tooltip("Time given to free dash.")]
    [Range(0f, 999f)]
    [SerializeField] float freeDashTime;
    [Tooltip("Time scale when slowing down time if parried a projectile.")]
    [Range(0f,1f)]
    [SerializeField] float slowTimeScale;
    [SerializeField] pDash dash;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource audioParry;
    #endregion
    
    #region Global Variable
    public static bool IsGivenFreeDash => _isGivenFreeDash;

    int? _spamCache = null;
    Coroutine _spamWindowCache;
    bool _isSpamming;
    static bool _isGivenFreeDash;
    IParriable _parriableHostile;
    Coroutine _giveFreeDashCache;
    #endregion

    #region Execution
    void Update()
    {
        _isSpamming = IsSpammingParryInput();

        if (_parriableHostile != null)
        {
            if (IsCanParry() && PlayerInputManager.IsPerformedParry)
            {
                if (_parriableHostile is Projectile)
                {
                    if (_giveFreeDashCache != null)
                    {
                        StopFreeDash(false);
                    }
                    GiveFreeDash();
                }
                _parriableHostile.Parry();
               ResetSpam();
               AudioManager.PlayAudioSource(audioParry);
            }
        }

        if (_giveFreeDashCache != null)
        {
            if (pDash.IsDashing)
            {
                StopFreeDash(true);
            }
            else if (pDash.IsDamagedDashing || PlayerInputManager.IsPerformedJump)
            {
                StopFreeDash(false);
            }
        }

    }

    void OnTriggerStay2D(Collider2D other) 
    {
        Collider2D otherCollider = other.gameObject.GetComponent<Collider2D>();
        if (!parryCollisionRange.IsTouching(otherCollider))
        {
            return;
        }

        if (other.gameObject.tag == "Enemy")
        {
            if (_parriableHostile == null)
            {
                _parriableHostile = other.gameObject.GetComponent<Enemy>();
            }
        }
        else if (other.gameObject.tag == "Projectile")
        {
            if (_parriableHostile == null)
            {
                _parriableHostile = other.gameObject.GetComponent<Projectile>();
            }
        }    
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Projectile")
        {
            _parriableHostile = null;
        }  
    }
    #endregion

    bool IsCanParry()
    {
        return _parriableHostile.IsParriable() && !_isSpamming;
    }

    bool IsSpammingParryInput()
    {
        bool isPressingParry = PlayerInputManager.IsPerformedParry;
        
        if (isPressingParry)
        {
            isPressingParry = true;
        }

        if (_spamCache == null && isPressingParry)
        {
            _spamCache = 1;
            if (_spamWindowCache == null)
            {
                _spamWindowCache = StartCoroutine(ResetSpamWindow());
            }
            return false;
        }

        if (_spamCache != null && _spamWindowCache != null && isPressingParry)
        {
            StopCoroutine(_spamWindowCache);
            _spamWindowCache = StartCoroutine(ResetSpamWindow());
            return true;
        }

        if (_spamWindowCache == null && !isPressingParry)
        {
            return false;
        }

        return _isSpamming;

        IEnumerator ResetSpamWindow()
        {
            yield return new WaitForSecondsRealtime(timeWindowForSpam);
            _spamCache = null;
            _spamWindowCache = null;
        }
    }

    void ResetSpam()
    {
        if (_spamWindowCache != null)
        {
            StopCoroutine(_spamWindowCache);
            _spamWindowCache = null;
        }
        _spamCache = null;
    }

    void GiveFreeDash()
    {
        if (_giveFreeDashCache == null)
        {
            _giveFreeDashCache = StartCoroutine(ExecuteGiveFreeDash());
            _isGivenFreeDash = true;
        }

        IEnumerator ExecuteGiveFreeDash()
        {
            dash.StopDash();
            dash.AlterDashCount?.Invoke(1);
            SlowTime();
            yield return new WaitForSecondsRealtime(freeDashTime);
            dash.AlterDashCount?.Invoke(-1);
            RestoreTime();
            _isGivenFreeDash = false;
        }

    }
    
    void SlowTime()
    {
        Time.timeScale = slowTimeScale;
    }

    void RestoreTime()
    {
        Time.timeScale = 1f;
    }

    void StopFreeDash(bool isTookFreeDash)
    {
        RestoreTime();
        if (!isTookFreeDash)
        {
            dash.AlterDashCount?.Invoke(-1);
        }
        StopCoroutine(_giveFreeDashCache);
        _isGivenFreeDash = false;
        _giveFreeDashCache = null;
    }
}
