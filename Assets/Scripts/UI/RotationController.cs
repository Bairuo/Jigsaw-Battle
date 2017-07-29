using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationController : MonoBehaviour {

    public float speed;
    private float w, z;
	// Use this for initialization
	void Start () {
        w = this.gameObject.GetComponent<RectTransform>().rotation.w;
        z = this.gameObject.GetComponent<RectTransform>().rotation.z;
    }
	
	// Update is called once per frame
	void Update () {
        z += Time.deltaTime * speed / 180;
        if (z >= 1)
            z = -1;
        this.gameObject.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, z, w);
	}
}
