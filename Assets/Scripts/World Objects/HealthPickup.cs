﻿using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] int health = 1;

    public void Heal()
    {
        bool isHealed = Player.Instance.Heal(health);
        if (isHealed)
        {
            Destroy(gameObject);
        }
    }
}
