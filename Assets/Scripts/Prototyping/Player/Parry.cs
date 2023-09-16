using System.Collections;
using UnityEngine;

public class Parry : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] float timeWindowForSpam = 0.2f;
    
    int? _spamCache = null;
    Coroutine _spamWindowCache;
    bool _isSpamming;
    Enemy enemy;

    void Update()
    {
        _isSpamming = IsSpammingParryInput();
        Debug.Log("Is spamming: " + _isSpamming);

        if (enemy != null)
        {
            if (enemy.IsParriable && !_isSpamming && PlayerInputManager.IsPerformedParry)
            {
                enemy.Parry();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (enemy == null)
            {
                enemy = other.gameObject.GetComponent<Enemy>();
            }
        }    
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemy = null;
        }  
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
