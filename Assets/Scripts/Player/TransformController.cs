using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformController : MonoBehaviour {
    GameObject controller;
    public GameObject player;
    public float speed = 0.5f;
	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.Find("Player");
        TransformChanger(obj);
	}

    // Update is called once per frame
    void Update() {
        Rigidbody2D rb2 = controller.GetComponent<Rigidbody2D>();
        if (Input.GetKey("up"))
        {
            Vector2 velocity = rb2.velocity + new Vector2(0, 1);
            velocity.Normalize();
            rb2.velocity = velocity * speed;
        }
        if (Input.GetKey("down"))
        {
            Vector2 velocity = rb2.velocity + new Vector2(0, -1);
            velocity.Normalize();
            rb2.velocity = velocity * speed;
        }
        if (Input.GetKey("right"))
        {
            Vector2 velocity = rb2.velocity + new Vector2(1, 0);
            velocity.Normalize();
            rb2.velocity = velocity * speed;
        }
        if (Input.GetKey("left"))
        {
            Vector2 velocity = rb2.velocity + new Vector2(-1, 0);
            velocity.Normalize();
            rb2.velocity = velocity * speed;
        }
        if(controller.tag == "Block")
        {
            if(Input.GetKey("e"))
            {
                PlayerCreater();
                rb2.velocity = new Vector2(0, 0);
            }
        }
    }


    public void TransformChanger(GameObject obj)
    {
        this.gameObject.transform.SetParent(obj.transform);
        controller = this.gameObject.transform.parent.gameObject;
    }

    public void PlayerCreater()
    {
        MonoBehaviour.Instantiate(player);
        GameObject obj = GameObject.FindWithTag("Player");
        TransformChanger(obj);
    }
}
