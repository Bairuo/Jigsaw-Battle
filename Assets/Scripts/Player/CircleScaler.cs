using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScaler : MonoBehaviour {
    public float maxSize;
    public float changeTime;
    private float timer;
    private float localSize;
    private float localMaxSize;
    private bool flag;
    // Use this for initialization
    void Start()
    {
        timer = 0;
        flag = true;
        localSize = this.gameObject.transform.localScale.x;
        localMaxSize = localSize * maxSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            flag = true;
        }
        if (timer >= changeTime)
        {
            flag = false;
        }
        if (flag)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
        float size = (timer / changeTime) * (localMaxSize - localSize) + localSize;
        this.gameObject.transform.localScale = new Vector3(size, size, 1);
    }
}
