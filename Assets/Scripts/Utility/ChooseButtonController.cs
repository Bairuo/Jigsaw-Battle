using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseButtonController : MonoBehaviour {

    public GameObject panel1;
    public GameObject Panel2;


	// Use this for initialization
	void Start () {
        panel1.gameObject.SetActive(true);
        Panel2.gameObject.SetActive(false);
	}

    public void OnBattleTeamButtonClick()
    {
        panel1.gameObject.SetActive(false);
        Panel2.gameObject.SetActive(true);
    }

    public void OnReturnClick()
    {
        panel1.gameObject.SetActive(true);
        Panel2.gameObject.SetActive(false);
    }
}
