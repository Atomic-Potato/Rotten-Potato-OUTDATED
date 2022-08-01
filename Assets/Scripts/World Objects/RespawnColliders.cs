using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnColliders : MonoBehaviour
{
    [SerializeField] float respawnTime;
    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject player;
    [SerializeField] GameObject timer;

    float respawnTimer = 0f;
    bool respawn;

    void Update()
    {
        if(respawn)
        {
            respawnTimer += Time.deltaTime;
            player.GetComponent<PlayerController>().enabled = false;

            if(respawnTimer >= respawnTime)
            {
                player.GetComponent<PlayerController>().enabled = true;

                player.transform.position = respawnPoint.position;
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0f); //for somereason it sometimes changes the z position
                
                respawnTimer = 0f;
                respawn = false;

                //Disablign the timer
                timer.SetActive(false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
            respawn = true;
    }
}
