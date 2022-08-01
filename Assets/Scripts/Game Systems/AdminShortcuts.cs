using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminShortcuts : MonoBehaviour
{
    //Enemy Spawning
    //Flyers
    [SerializeField] int flyersToSpawn = 1;
    [SerializeField] Transform flyerSpawnTransform;
    [SerializeField] GameObject flyerPrefab;

    [Space]
    //Rocketator 
    [SerializeField] float spawnHeight;
    [Tooltip("Dont forget that the rocketator always spawns right above player")]
    [SerializeField] Transform playerTransfom;
    [SerializeField] GameObject rocketatoPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
            SpawnFlyer();

        if (Input.GetKeyDown(KeyCode.R))
            SpawnRocketato();
    }

    void SpawnFlyer()
    {
        float flyersLeft = flyersToSpawn;
        while (flyersLeft > 0)
        {
            Instantiate(flyerPrefab, flyerSpawnTransform.position, Quaternion.identity);
            flyersLeft--;
        }
    }

    void SpawnRocketato()
    {
        Instantiate(rocketatoPrefab, new Vector3(playerTransfom.position.x, playerTransfom.position.y + spawnHeight, 0), Quaternion.identity);
    }
}
