using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _CircleController : MonoBehaviour
{
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
        localSize = this.gameObject.GetComponent<RectTransform>().localScale.x;
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
        this.gameObject.GetComponent<RectTransform>().localScale = new Vector3(size, size, 1.0f);
        this.gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.9f - timer / changeTime * 0.6f);

    }
}