﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Block")
        {
            this.gameObject.GetComponentInChildren<TransformController>().TransformChanger(collision.gameObject);
            Destroy(this.gameObject);
            collision.gameObject.GetComponent<Block>().Engage(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), 1);
        }
    }
}
