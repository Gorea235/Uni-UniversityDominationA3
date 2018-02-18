using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGameEnemy : MonoBehaviour
{

    #region Unity Bindings

    public GameObject enemyBullet;

    #endregion

    #region Private Fields

    // constant
    const float moveDownAmount = 0.05f;

    // shared
    static float speed = 0.5f;
    static Renderer[] enemies;
    static int totalEnemiesLeft;
    static EnemyDirection currentDirection = EnemyDirection.Left;

    // internal
    Vector2 constantVelocity = new Vector2(1, 0) * speed;
    Rigidbody2D rigidBody;
    float minFireRate = 10f;
    float maxFireRate = 60f;
    float baseFireWait = 2f;
    SpaceInvadersMinigameManager gameManager;
    float startTime;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        // if we haven't initialised the reference for the enemies, do so
        if (enemies == null)
        {
            enemies = transform.root.GetComponentsInChildren<Renderer>();
            totalEnemiesLeft = enemies.Length; // keep track of the number of enemies left
        }

        rigidBody = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("MiniGameManager").GetComponent<SpaceInvadersMinigameManager>();
    }

    void Start()
    {
        startTime = Time.time; // grab the time the game started at
        RandomiseFireRate(); // set next shoot time
        DoDirectionSwitch(EnemyDirection.Right); // start moving
    }

    void FixedUpdate()
    {
        Shoot();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "LeftBoundary": // hit the left wall
                SwitchAllDirections(EnemyDirection.Right);
                break;
            case "RightBoundary": // hit the right wall
                SwitchAllDirections(EnemyDirection.Left);
                break;
            case "BottomBoundary": // hit the bottom (& thus failed)
                gameManager.EndMiniGame(false);
                break;
        }
    }

    void OnDestroy()
    {
        totalEnemiesLeft--;
        if (totalEnemiesLeft <= 0) // if there are no enemies left, the player won
            gameManager.EndMiniGame(true);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Switches the directions of all the enemies.
    /// </summary>
    /// <param name="direction">The direction to switch to.</param>
    void SwitchAllDirections(EnemyDirection direction)
    {
        if (direction != currentDirection) // only switch if it's changed
            foreach (var en in enemies.Where(e => e != null))
                en.gameObject.GetComponent<MiniGameEnemy>().DoDirectionSwitch(direction);
    }

    /// <summary>
    /// Switches the direction of the current enemy.
    /// </summary>
    /// <param name="direction">The direction to switch to.</param>
    void DoDirectionSwitch(EnemyDirection direction)
    {
        rigidBody.velocity = constantVelocity * (int)direction; // set direction
        Vector2 pos = transform.position;
        pos.y -= moveDownAmount;
        transform.position = pos;
        currentDirection = direction;

    }

    /// <summary>
    /// Will shoot a bullet after a given amount of time.
    /// </summary>
    void Shoot()
    {
        // if it's been long enough, shoot a bullet
        if ((Time.time - startTime) > baseFireWait)
        {
            RandomiseFireRate(); // set next shot time
            Instantiate(enemyBullet.transform, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Sets the next point in time the enemy should shoot.
    /// </summary>
    void RandomiseFireRate()
    {
        baseFireWait = baseFireWait + Random.Range(minFireRate, maxFireRate);
    }

    #endregion
}
