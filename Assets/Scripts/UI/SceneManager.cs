using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    public static SceneManager instance;

    public SceneManager(){
        instance = this;
    }

    void Update()
    {
        Client.instance.Update();
    }

    void Start()
    {
        if (ServerNet.instance == null) new ServerNet();
        if (Client.instance == null) new Client();

        // Debug
        //ServerNet.instance.Start("127.0.0.1", 9970);
    }

    public void OnStartClick()
    {
        
        if (Client.IsUse() == false)
        {
            Client.instance.Connect();
        }
            
    }

    public void StartScene()
    {
        Application.LoadLevel("dk-test");
    }
}
