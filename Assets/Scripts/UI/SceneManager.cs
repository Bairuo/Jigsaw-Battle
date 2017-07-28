using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    public void OnStartClick()
    {
        Debug.Log("Click!");
        Application.LoadLevel("dk-test");
    }
}
