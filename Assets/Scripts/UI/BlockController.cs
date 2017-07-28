using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlockController : MonoBehaviour {

    public float time;
    public float redius;
    public GameObject block;
    public GameObject parent;
    public GameObject blockController;
    private float timer;
	// Use this for initialization
	void Start(){
        timer = time;
 
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = time;
            BlockCreater();
        }
	}
    
    public void BlockCreater()
    {
        GameObject gbj =  Object.Instantiate(block, parent.gameObject.transform);
        Vector2 position = Random.insideUnitCircle;
        gbj.gameObject.GetComponent<RectTransform>().position = new Vector3(this.gameObject.GetComponent<RectTransform>().position.x + position.x * redius, this.gameObject.GetComponent<RectTransform>().position.y + position.y * redius, 0);
    }
}
