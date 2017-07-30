using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _BloickController : MonoBehaviour {

    public float speed;
    public float time;
    private IEnumerator coroutine;
    // Use this for initialization
    void Start () {
        float alpha = Random.value * 0.3f + 0.2f;
        //Debug.Log(alpha);
        this.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        coroutine = WaitAndDestroy(1.0f);
        StartCoroutine(coroutine);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos =  this.gameObject.GetComponent<RectTransform>().position;
        pos.x -= speed * Time.deltaTime;
        this.gameObject.GetComponent<RectTransform>().position = pos;
	}

    private IEnumerator WaitAndDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);
    }
}
