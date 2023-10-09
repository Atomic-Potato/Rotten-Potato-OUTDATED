using System.Collections.Generic;
using UnityEngine;

public class TagsManager : MonoBehaviour 
{
    public static string Tag_Player => "Player";     
    public static string Tag_Enemy => "Enemy";    
    public static string Tag_MediumEnemy => "Medium Enemy"; 
    public static string Tag_Projectile => "Projectile";
    
    public static List<string> EnemyTags;

    static TagsManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        EnemyTags = new List<string>()
        {
            Tag_Enemy,
            Tag_MediumEnemy
        };

        Debug.Log(EnemyTags);
    }
}