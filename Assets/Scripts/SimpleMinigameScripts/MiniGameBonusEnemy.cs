using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGameBonusEnemy : MonoBehaviour
{

    #region Private Fields

    float speed = 2f;
    int timesCollided= 0; //as it will collide with two walls, we keep track of when precisely to destroy it
    Rigidbody2D rigidBody;

    #endregion

    #region MonoBehaviour
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
    }

    #endregion

    #region Helper Methods

    void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Boundary":
                timesCollided++;
                if (timesCollided == 2)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    #endregion
}
