using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGameEnemy : MonoBehaviour
{

    #region Unity Bindings

    public GameObject enemyBullet;
    public static Renderer[] enemies = new Renderer[55];

    #endregion

    #region Private Fields

    static float speed = 0.5f;
    Vector2 constantVelocity = new Vector2(1, 0) * speed;
    static int direction = 1;
    Rigidbody2D rigidBody;
    float minFireRate = 5f;
    float maxFireRate = 55f;
    float baseFireWait = 2f;

    #endregion

    #region MonoBehaviour
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = constantVelocity;
        transform.root.GetComponentsInChildren<Renderer>().CopyTo(enemies, 0);
        randomiseFireRate();
    }

    private void FixedUpdate()
    {
        Shoot();
    }
    #endregion

    #region Helper Methods

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {

            case "Boundary":
                direction = direction * -1;
                foreach (Renderer enemy in enemies.Where(enemy => enemy != null)) //as we are keeping a reference to a static list, we have to omitt the already destroyed enemies from our search
                {
                    enemy.gameObject.GetComponent<MiniGameEnemy>().ChangeDirection();
                    enemy.gameObject.GetComponent<MiniGameEnemy>().MoveDown();
                }
                break;
            case "BottomBoundary":
                //
                //HANDLE FAILED GAME HERE
                //
                Debug.Log("GAME OVER");
                break;
        }
    }

    void ChangeDirection()
    {
        rigidBody.velocity = constantVelocity * direction;
    }

    void MoveDown()
    {
        rigidBody.velocity = constantVelocity * direction;
        Vector2 newPosition = transform.position;
        newPosition.y += -0.05f;
        transform.position = newPosition;
    }

    void Shoot()
    {
        if (Time.time > baseFireWait)
        {
            randomiseFireRate();
            Instantiate(enemyBullet.transform, transform.position, Quaternion.identity);

        }
    }

    void randomiseFireRate()
    {
        baseFireWait = baseFireWait + Random.Range(minFireRate, maxFireRate);
    }
    #endregion
}
