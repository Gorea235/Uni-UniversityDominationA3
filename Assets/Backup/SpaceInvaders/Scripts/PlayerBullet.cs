using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    #region Unity Bindings

    #endregion

    #region Private Fields

    SpaceInvadersMinigameManager gameManager;
    float speed = 8;
    Rigidbody2D bulletBody;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        gameManager = GameObject.Find("MiniGameManager").GetComponent<SpaceInvadersMinigameManager>();
        bulletBody = GetComponent<Rigidbody2D>();
        bulletBody.velocity = Vector2.up * speed; // set bullet speed
    }

    #endregion

    #region Helper Methods

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Boundary": // if we hit the top, remove the bullet
                Destroy(gameObject);
                break;
            case "Enemy": // if we hit an enemy remove it and add a kill to the score
                Destroy(gameObject);
                Destroy(collision.gameObject);
                gameManager.AddKill();
                break;
            case "BonusEnemy": // if we hit a bonus enemy, remove it and add a bonus kill to the score
                Destroy(gameObject);
                Destroy(collision.gameObject);
                gameManager.AddBonusKill();
                break;
        }
    }

    #endregion
}
