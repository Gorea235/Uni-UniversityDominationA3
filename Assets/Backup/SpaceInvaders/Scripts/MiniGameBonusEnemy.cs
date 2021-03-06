﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGameBonusEnemy : MonoBehaviour
{

    #region Private Fields

    float speed = 2f;
    Rigidbody2D rigidBody;

    #endregion

    #region MonoBehaviour

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "RightBoundary") // if the bonus enemy hits the wall, remove it
            Destroy(gameObject);
    }

    #endregion
}
