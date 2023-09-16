using System.Collections;
using UnityEngine;

public class Parry : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] float timeWindowForSpam = 0.2f;
    [Tooltip("An object can be parried once inside this collider")]
    [SerializeField] Collider2D parryCollisionRange;
    
    int? _spamCache = null;
    Coroutine _spamWindowCache;
    bool _isSpamming;
    IParriable parriableHostile;

    void Update()
    {
        _isSpamming = IsSpammingParryInput();

        if (parriableHostile != null)
        {
            if (IsCanParry() && PlayerInputManager.IsPerformedParry)
            {
                parriableHostile.Parry();
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
            if (parriableHostile == null)
            {
                parriableHostile = other.gameObject.GetComponent<Enemy>();
            }
        }
        else if (other.gameObject.tag == "Projectile")
        {
            if (parriableHostile == null)
            {
                parriableHostile = other.gameObject.GetComponent<Projectile>();
            }
        }    
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Projectile")
        {
            parriableHostile = null;
        }  
    }

    bool IsCanParry()
    {
        return parriableHostile.IsParriable() && !_isSpamming; 
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
}
