using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameEnemy : MonoBehaviour
{

    #region Unity Bindings

    public GameObject enemyBullet;

    #endregion

    #region Private Fields

    float speed = 0.5f;
    Rigidbody2D rigidBody;
    //   int movementDirection = 1; // right: 1 | left: -1

    #endregion




    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
    }

    #region Helper Methods

    void OnTriggerEnter2D(Collider2D collision)
    {
        Renderer[] enemies = transform.root.gameObject.GetComponentsInChildren<Renderer>();
        switch (collision.tag)
        {
            case "Boundary":
                foreach (Renderer enemy in enemies)
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
        //movementDirection = (movementDirection == 1) ? -1 : 1;
        rigidBody.velocity = (rigidBody.velocity * -1);
    }

    void MoveDown()
    {
        Vector2 newPosition = transform.position;
        newPosition.y += -0.05f;
        transform.position = newPosition;
    }

    #endregion
}
