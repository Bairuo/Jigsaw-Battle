using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleController : MonoBehaviour
{

    public float angle;
    public float changetime;
    public Image circle;
    private float timer;
    private float state;
    // Use this for initialization
    void Start()
    {
        timer = 0;
        state = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer += state * Time.deltaTime;
        if (timer >= changetime)
            state = -1;
        if (timer <= -changetime)
            state = 1;
        float w = circle.rectTransform.rotation.w;
        circle.rectTransform.rotation = new Quaternion(0, 0, timer / changetime * angle / 180, w);
    }
}