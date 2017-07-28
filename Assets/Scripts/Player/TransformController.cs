using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformController : MonoBehaviour {
    GameObject controller;
    public float speed = 0.5f;
    public float redius = 5f; //重新出现区域的半径
    struct Position
    {
        public float x;
        public float y;
        public void SetPostion(float _x, float _y)
        {
            float root = Mathf.Sqrt(_x * _x + _y * _y);
            x = _x / root;
            y = _y / root;
        }
    }
    [SerializeField] private JoyStick joystick;

	// Use this for initialization
	void Start () {
        controller = this.gameObject.transform.parent.gameObject;
        joystick = GameObject.Find("JoyStick").GetComponent<JoyStick>();
    }

    // Update is called once per frame
    void Update() {
        Rigidbody2D rb2 = controller.GetComponent<Rigidbody2D>();
        //if (Input.GetKey("up"))
        //{
        //    Vector2 velocity = rb2.velocity + new Vector2(0, 1);
        //    velocity.Normalize();
        //    rb2.velocity = velocity * speed;
        //}
        //if (Input.GetKey("down"))
        //{
        //    Vector2 velocity = rb2.velocity + new Vector2(0, -1);
        //    velocity.Normalize();
        //    rb2.velocity = velocity * speed;
        //}
        //if (Input.GetKey("right"))
        //{
        //    Vector2 velocity = rb2.velocity + new Vector2(1, 0);
        //    velocity.Normalize();
        //    rb2.velocity = velocity * speed;
        //}
        //if (Input.GetKey("left"))
        //{
        //    Vector2 velocity = rb2.velocity + new Vector2(-1, 0);
        //    velocity.Normalize();
        //    rb2.velocity = velocity * speed;
        //}
        Vector2 vec = joystick.rectT_Joy.anchoredPosition / 10;
        rb2.velocity = vec;
        if(controller.tag == "Block")
        {
            if(Input.GetKey("e"))
            {
                if(controller.gameObject.GetComponent<Block>().Leave())
                {
                    PlayerCreater();
                    rb2.velocity = new Vector2(0, 0);
                }
            }
        }
    }

    Position PositionFinder()
    {
        Position[] poses = new Position[8];
        poses[0].SetPostion(1, 1);
        poses[1].SetPostion(1, 0);
        poses[2].SetPostion(1, -1);
        poses[3].SetPostion(0, 1);
        poses[4].SetPostion(0, -1);
        poses[5].SetPostion(-1, 1);
        poses[6].SetPostion(-1, 0);
        poses[7].SetPostion(-1, -1);
        Position[] poses_r = new Position[8];
        int count = 0;
        foreach(Position pos in poses)
        {
            RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, new Vector2(pos.x, pos.y), 0, 0, redius);
            if(!hit)
            {
                poses_r[count] = pos;
                count++;
            }
        }
        return poses_r[Random.Range(0, count)];
    }


    public void TransformChanger(GameObject obj)
    {
        this.gameObject.transform.SetParent(obj.transform);
        controller = this.gameObject.transform.parent.gameObject;
    }

    public void PlayerCreater()
    {
        Position pos = PositionFinder();
        Transform parent = GameObject.Find("ObjectController").gameObject.transform;
        foreach(Transform child in parent)
        {   
            if(child.gameObject.tag == "Player")
            {
                Vector2 _pos = controller.transform.position;
                TransformChanger(child.gameObject);
                child.SetParent(GameObject.Find("_ObjectController").gameObject.transform);
                child.position = new Vector3(pos.x * redius + _pos.x, pos.y * redius + _pos.y, 0);
                child.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                child.gameObject.SetActive(true);
                break;
            }
        }
    }
}
