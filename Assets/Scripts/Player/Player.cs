using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

// Note:
// This script execution is ran before the default execution order
public class Player : MonoBehaviour
{
    #region Inspector Variables
    public bool IsImmortal;
    [MinMax(1, 999)]
    [SerializeField] int hitPoints = 4;
    [MinMax(1,999)]
    [SerializeField] int maxHitPoints = 20;
    [Range(0f, 10f)]
    [SerializeField] float recoveryTime = 2f;

    [Space]
    [SerializeField] Rigidbody2D rigidbody;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource audioHurt;
    #endregion

    #region Global Variables
    static Player _instance;
    public static Player Instance => _instance;
    public GameObject Object => gameObject;
    public int HitPoints => hitPoints;
    public Rigidbody2D Rigidbody => rigidbody;
    public Action UpdateHitPoints;
    public Action<int> Damage;
    public Func<int, bool> Heal;
    public Action Respawn;

    int _initialHitPoints;
    static bool _isInRecovery;
    public static bool IsInRecovery => _isInRecovery;
    Coroutine _recoverCache;
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

        _initialHitPoints = hitPoints;

        Damage = ApplyDamage;
        Heal = ApplyHeal;
        Respawn = ExecuteRespawn;
        UpdateHitPoints = ExecuteUpdateHitpoints;
    }


    void ApplyDamage(int damagePoints)
    {
        if (_isInRecovery)
        {
            return;
        }

        hitPoints -= damagePoints;
        UpdateHitPoints();
        AudioManager.PlayAudioSource(audioHurt);

        if (hitPoints <= 0)
        {
            if (IsImmortal)
            {
                hitPoints = _initialHitPoints;
                UpdateHitPoints();
            }
            else
            {
                Kill();
            }
        }

        Recover();

        void Recover()
        {
            if (_recoverCache == null)
            {
                _recoverCache = StartCoroutine(ExecuteRecover());
            }

            IEnumerator ExecuteRecover()
            {
                _isInRecovery = true;
                yield return new WaitForSeconds(recoveryTime);
                _isInRecovery = false;
                _recoverCache = null;
            }
        }
    }

    bool ApplyHeal(int health)
    {
        if (hitPoints >= maxHitPoints || health < 0)
            return false; // failed to heal

        hitPoints += health;
        UpdateHitPoints();
        return true;
    }


    public void Kill()
    {
        Object.SetActive(false);
        Respawn?.Invoke();
    }

    void ExecuteRespawn()
    {
        ReloadScene();

        void ReloadScene()
        {
           Scene scene = SceneManager.GetActiveScene();
           SceneManager.LoadScene(scene.name);
        }
    }
    void ExecuteUpdateHitpoints(){}
}
