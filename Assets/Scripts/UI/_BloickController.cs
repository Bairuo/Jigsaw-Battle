using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _BloickController : MonoBehaviour {

    public float speed;
    private IEnumerator coroutine;
    // Use this for initialization
    void Start () {
        coroutine = WaitAndDestroy(1.0f);
        StartCoroutine(coroutine);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator WaitAndDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);
    }
}
