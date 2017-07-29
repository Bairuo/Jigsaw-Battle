using System;
using UnityEngine;

public class NetUnitData{
    //对于玩家来说id是玩家在房间内的id
    //对于其它物体来说id是创建端的InstanceID
    string id;

    public NetUnitData(string _id, GameObject obj)
    {
        id = _id;
        float x = obj.gameObject.transform.position.x;
        float y = obj.gameObject.transform.position.y;
        float z = obj.gameObject.transform.position.z;

        if (obj.tag != "Player")
        {
            if (Client.instance.playerid != "0")
            {
                Rigidbody2D r = obj.GetComponent<Rigidbody2D>();
                r.constraints = RigidbodyConstraints2D.FreezeAll;
            }

        }
        else
        {
            if (id != Client.instance.playerid)
            {
                Rigidbody2D r = obj.GetComponent<Rigidbody2D>();
                r.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

    }

    public static ProtocolBytes GetUnitData(string protoName, string id, Vector3 pos)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString(protoName);
        proto.AddString(id);

        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);

        return proto;
    }

}
