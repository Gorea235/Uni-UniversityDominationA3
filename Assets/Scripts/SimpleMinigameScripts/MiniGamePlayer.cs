using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePlayer : MonoBehaviour
{
    #region Unity Bindings

    public GameObject bullet;

    #endregion

    #region Private Fields

    float speed = 5;
    Transform shotDisplayed; // Original space invaders allows only for one player bullet at a time in the game

    #endregion

    #region Public Properties

    public static int KillCount { get; set; }

    #endregion

    #region MonoBehaviour

    void FixedUpdate()
    {
        //use GetAxis instead if you prefer smooth movement opposed to classic arcade feel
        float moveInX = Input.GetAxisRaw("Horizontal"); //Axis set in Edit > Project settings > Input
        GetComponent<Rigidbody2D>().velocity = new Vector2(moveInX, 0) * speed;
    }

    void Update()
    {
        FireBullet();
    }

    #endregion

    #region Helper Methods

    void FireBullet()
    {
        if (Input.GetButtonDown("Jump") && shotDisplayed == null) // "Jump" button corresponds to "space" by default
        {
            shotDisplayed = Instantiate(bullet.transform, transform.position, Quaternion.identity);
            Debug.Log("Bullet fired");
            Debug.Log(KillCount);
        }
    }

    #endregion
}
