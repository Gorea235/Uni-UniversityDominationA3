﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MiniGameManager
{
    #region Unity Bindings

    #endregion

    #region Private Fields

    float speed = 8;
    Rigidbody2D bulletBody;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        bulletBody = GetComponent<Rigidbody2D>();
        bulletBody.velocity = Vector2.up * speed;
    }

    #endregion

    #region Helper Methods

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Boundary":
                Destroy(gameObject);
                break;

            case "Enemy":
                Destroy(gameObject);
                Destroy(collision.gameObject);
                MiniGamePlayer.KillCount++;
                break;

            case "BonusEnemy":
                Destroy(gameObject);
                Destroy(collision.gameObject);
                MiniGamePlayer.KillCount += 10;
                break;
        }
    }

    #endregion
}
