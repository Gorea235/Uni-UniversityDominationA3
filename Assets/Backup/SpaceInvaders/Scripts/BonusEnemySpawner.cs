using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BonusEnemySpawner : MonoBehaviour
{

    #region Unity Bindings

    public GameObject bonusEnemy;

    #endregion

    #region Private Fields

    float minSpawnRate = 10f;
    float maxSpawnRate = 20f;
    float baseSpawnWait = 4f;
    float startTime;

    #endregion

    #region MonoBehaviour

    void Start()
    {
        startTime = Time.time; // grab the time at which the minigame started
        randomiseSpawnRate(); // init first spawn time
    }

    private void Update()
    {
        Spawn();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Will spawn a bonus enemy after the set amount of time.
    /// </summary>
    void Spawn()
    {
        // if it's been long enough, spawn a bonus enemy
        if ((Time.time - startTime) > baseSpawnWait)
        {
            randomiseSpawnRate();
            Instantiate(bonusEnemy.transform, transform.position, Quaternion.identity);

        }
    }

    /// <summary>
    /// Sets the next point in time the spawner should create the bonus enemy.
    /// </summary>
    void randomiseSpawnRate()
    {
        baseSpawnWait = baseSpawnWait + Random.Range(minSpawnRate, maxSpawnRate);
    }
    #endregion
}
