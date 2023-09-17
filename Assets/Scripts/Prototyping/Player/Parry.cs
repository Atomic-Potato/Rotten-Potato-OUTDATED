using System.Collections;
using UnityEngine;

public class Parry : MonoBehaviour
{
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
    
    int? _spamCache = null;
    Coroutine _spamWindowCache;
    bool _isSpamming;
    IParriable _parriableHostile;
    Coroutine _giveFreeDashCache;

    #region Execution
    void Update()
    {
        _isSpamming = IsSpammingParryInput();

        if (_parriableHostile != null)
        {
            if (IsCanParry() && PlayerInputManager.IsPerformedParry)
            {
                Debug.Log(_parriableHostile);
                if (_parriableHostile is Projectile)
                {
                    GiveFreeDash();
                }

                _parriableHostile.Parry();

            }
        }

        if (_giveFreeDashCache != null)
        {
            if (pDash.IsDashing)
            {
                RestoreTime();
                StopCoroutine(_giveFreeDashCache);
                _giveFreeDashCache = null;
            }
            else if (pDash.IsDamagedDashing || PlayerInputManager.IsPerformedJump)
            {
                RestoreTime();
                dash.DecrementDashes(1);
                StopCoroutine(_giveFreeDashCache);
                _giveFreeDashCache = null;
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
            yield return new WaitForSeconds(timeWindowForSpam);
            _spamCache = null;
            _spamWindowCache = null;
        }
    }

    void GiveFreeDash()
    {
        if (_giveFreeDashCache == null)
        {
            _giveFreeDashCache = StartCoroutine(ExecuteGiveFreeDash());
        }

        IEnumerator ExecuteGiveFreeDash()
        {
            dash.IncrementDashes(1);
            SlowTime();
            yield return new WaitForSeconds(freeDashTime);
            dash.DecrementDashes(1);
            RestoreTime();
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
}
