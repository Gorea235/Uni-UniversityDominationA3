using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MiniGameManager
{
    #region Unity Bindings

    #endregion

    #region Private Fields

    float speed = 15;
    Rigidbody2D bulletBody;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        if (!MiniPlayer.ForbidBullet)
        {
            bulletBody = GetComponent<Rigidbody2D>();
            bulletBody.velocity = Vector2.up * speed;
            MiniPlayer.ForbidBullet = true;
        }
    }

    #endregion

    #region Helper Methods

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boundary")
        {
            Destroy(gameObject);
            MiniPlayer.ForbidBullet = false;
        }
    }

    #endregion
}
