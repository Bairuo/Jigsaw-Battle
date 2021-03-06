﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int playerID = 1;
    public string netID;

    public void SetPlayerID(string netID)
    {
        this.netID = netID;
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

        SetInitPosition(new Vector3(playerID, playerID, 0));
    }

    public static void PlayerInit(string netID)
    {
        GameObject prefab = Resources.Load("Players/Player") as GameObject;
        GameObject player = GameObject.Instantiate(prefab);
        player.GetComponent<PlayerController>().SetPlayerID(netID);
        Client.instance.posmanager.PlayerRegister(player);
    }

	// Use this for initialization
	void Start () {
        
	}

    void SetInitPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //private void OnCollisionEnter2D(Collision2D collision)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            if (netID != Client.instance.playerid) return;
            Client.instance.SendEnterBlock(netID, collision.gameObject.GetComponent<Block>().net_id);
            PlayerEnter(this.gameObject, collision.gameObject);
            /*
            if(collision.gameObject.GetComponent<Block>().Engage(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), playerID))
            {
                this.gameObject.GetComponentInChildren<TransformController>().TransformChanger(collision.gameObject);
                this.gameObject.transform.SetParent(GameObject.Find("ObjectController").transform);
                GameObject.Find("ObjectController").gameObject.GetComponent<ObjectController>().init();
            }*/
        }
    }
    
    public void PlayerEnter(GameObject player, GameObject block)
    {
        if (block.GetComponent<Block>().Engage(new Vector2(player.transform.position.x, player.transform.position.y), playerID))
        {
            player.GetComponentInChildren<TransformController>().TransformChanger(block);
            player.transform.SetParent(GameObject.Find("ObjectController").transform);
            GameObject.Find("ObjectController").gameObject.GetComponent<ObjectController>().init();
        }
    }

}
