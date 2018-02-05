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

    #endregion

    #region MonoBehaviour
    void Start()
    {
        randomiseSpawnRate();
    }

    private void Update()
    {
        Spawn();
    }

    #endregion

    #region Helper Methods

    void Spawn()
    {
        if (Time.time > baseSpawnWait)
        {
            randomiseSpawnRate();
            Instantiate(bonusEnemy.transform, transform.position, Quaternion.identity);

        }
    }

    void randomiseSpawnRate()
    {
        baseSpawnWait = baseSpawnWait + Random.Range(minSpawnRate, maxSpawnRate);
    }
    #endregion
}
