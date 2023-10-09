using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour 
{
    public static string Tag_Player => "Player";     
    public static string Tag_Enemy => "Enemy";    
    public static string Tag_MediumEnemy => "Medium Enemy"; 
    public static string Tag_Projectile => "Projectile";
    
    public static List<string> EnemyTags;

    void Awake()
    {
        EnemyTags = new List<string>()
        {
            Tag_Enemy,
            Tag_MediumEnemy
        };
    }
}