using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNetController : MonoBehaviour {
    public GameObject player;

	// Use this for initialization
	void Start () {
        Client.instance.posmanager.Init(Client.instance.playerid);

        player.GetComponent<PlayerController>().SetPlayerID(Client.instance.playerid);
        Client.instance.posmanager.PlayerRegister(player);

        Client.instance.SendPlayerInit();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
}
