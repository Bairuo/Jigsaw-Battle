using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int playerID = 1;


    public void SetPlayerID(string netID)
    {
        int id = int.Parse(netID);
        if (id % 2 == 0)    // 服务器端id是正数
        {
            playerID = id;
        }
        else
        {
            playerID = -id;
        }
        GetComponentInChildren<TransformController>().controllerID = netID;
    }

    public static void PlayerInit(string netID)
    {
        GameObject prefab = Resources.Load("Players/Player") as GameObject;
        GameObject player = GameObject.Instantiate(prefab);
        player.GetComponent<PlayerController>().SetPlayerID(netID);
    }

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
            if(collision.gameObject.GetComponent<Block>().Engage(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), playerID))
            {
                this.gameObject.GetComponentInChildren<TransformController>().TransformChanger(collision.gameObject);
                this.gameObject.transform.SetParent(GameObject.Find("ObjectController").transform);
                GameObject.Find("ObjectController").gameObject.GetComponent<ObjectController>().init();
            }
        }
    }
}
