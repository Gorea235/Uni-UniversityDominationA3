using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour {

    public GameObject player;

    float speed = 15;
    Rigidbody2D bulletBody;

   

	// Use this for initialization
	void Start () {
        if (!player.GetComponent<PlayerController>().ForbidBullet)
        {
            bulletBody = GetComponent<Rigidbody2D>();
            bulletBody.velocity = Vector2.up * speed;
            //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>
            player.GetComponent<PlayerController>().ForbidBullet = true;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boundary")
        {
            Destroy(gameObject);
            player.GetComponent<PlayerController>().ForbidBullet = false;
        }
    }

}
