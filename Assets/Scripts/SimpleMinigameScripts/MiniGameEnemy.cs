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
    static EnemyDirection currentDirection = EnemyDirection.Left;

    // internal
    Vector2 constantVelocity = new Vector2(1, 0) * speed;
    Rigidbody2D rigidBody;
    float minFireRate = 5f;
    float maxFireRate = 55f;
    float baseFireWait = 2f;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        if (enemies == null)
            enemies = transform.root.GetComponentsInChildren<Renderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        RandomiseFireRate();
        DoDirectionSwitch(EnemyDirection.Right);
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
            case "RightBoundary":
                SwitchAllDirections(EnemyDirection.Left);
                break;
            case "BottomBoundary":
                //
                //HANDLE FAILED GAME HERE
                //
                Debug.Log("GAME OVER");
                break;
        }
    }

    #endregion

    #region Helper Methods

    void SwitchAllDirections(EnemyDirection direction)
    {
        if (direction != currentDirection)
            foreach (var en in enemies.Where(e => e != null))
                en.gameObject.GetComponent<MiniGameEnemy>().DoDirectionSwitch(direction);
    }

    void DoDirectionSwitch(EnemyDirection direction)
    {
        rigidBody.velocity = constantVelocity * (int)direction;
        Vector2 pos = transform.position;
        pos.y -= moveDownAmount;
        transform.position = pos;
        currentDirection = direction;

    }

    void Shoot()
    {
        if (Time.time > baseFireWait)
        {
            RandomiseFireRate();
            Instantiate(enemyBullet.transform, transform.position, Quaternion.identity);
        }
    }

    void RandomiseFireRate()
    {
        baseFireWait = baseFireWait + Random.Range(minFireRate, maxFireRate);
    }

    #endregion
}
