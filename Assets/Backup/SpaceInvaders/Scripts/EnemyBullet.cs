using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    #region Unity Bindings

    #endregion

    #region Private Fields

    float speed = 6;
    Rigidbody2D bulletBody;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        bulletBody = GetComponent<Rigidbody2D>();
        bulletBody.velocity = Vector2.down * speed; // set bullet speed
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "BottomBoundary": // if bullet hits the bottom of the screen, remove it
                Destroy(gameObject);
                break;
            case "Player": // if the bullet hits the player, end the game
                Destroy(gameObject);
                Destroy(collision.gameObject);
                GameObject.Find("MiniGameManager").GetComponent<SpaceInvadersMinigameManager>().EndMiniGame(false);
                break;
        }
    }

    #endregion
}
