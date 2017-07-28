using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        this.gameObject.transform.position = new Vector2(0f, 0f);
    }
    
    void Update()
    {
        this.gameObject.transform.position = this.gameObject.transform.position + new Vector3(-.1f, -.1f);
    }
}
