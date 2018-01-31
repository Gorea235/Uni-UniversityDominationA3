using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    float speed = 5;
    bool isOnlyBullet; ////Original space invaders allows only for one player bullet at a time in the game
    public GameObject bullet;

    public bool ForbidBullet
    {
        get { return isOnlyBullet; }
        set { isOnlyBullet = value; }
    }

    private void FixedUpdate()
    {
        //use GetAxis instead if you prefer smooth movement opposed to classic arcade feel
        float moveInX = Input.GetAxisRaw("Horizontal"); //Axis set in Edit > Project settings > Input
        
        GetComponent<Rigidbody2D>().velocity = new Vector2(moveInX, 0) * speed;
    }


    void Update ()
    {
            FireBullet();
        
	}

    void FireBullet()
    {
        if (Input.GetButtonDown("Jump") && !ForbidBullet) // "Jump" button corresponds to "space" by default
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
            Debug.Log("Bullet fired");
        }
    }
}
