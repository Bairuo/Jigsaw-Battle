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
            rb2.velocity = new Vector2(0, speed);
        }
        if (Input.GetKey("down"))
        {
            rb2.velocity = new Vector2(0, -speed);
        }
        if (Input.GetKey("right"))
        {
            rb2.velocity = new Vector2(speed, 0);
        }
        if (Input.GetKey("left"))
        {
            rb2.velocity = new Vector2(-speed, 0);
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
