using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


public class PosManager
{
    public GameObject prefab;
    //PosManager只维护玩家列表，其余由GameController维护
    //注意玩家ID和其它人物ID生成方式是不同的
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> blocks = new Dictionary<string, GameObject>();

    Dictionary<string, NetUnitData> playersinfo = new Dictionary<string, NetUnitData>();
    Dictionary<string, NetUnitData> blocksinfo = new Dictionary<string, NetUnitData>();

    Dictionary<string, float> LastReceiveTime = new Dictionary<string, float>();

    GameObject player;

    string playerID = "";
    public bool isInit = false;
    public float lastSendTime = float.MinValue;

    public static PosManager instance;


    public PosManager()
    {
        instance = this;
    }

    public void PlayerRegister(GameObject player)
    {
        string netID = player.GetComponent<PlayerController>().netID;
        NetUnitData playerinfo = new NetUnitData(netID, player);

        //Debug.Log("Register: " + netID);
        lock(players)players.Add(netID, player);
        lock(playersinfo)playersinfo.Add(netID, playerinfo);
    }
    public void BlockRegister(GameObject block)
    {
        string netID = block.GetComponent<Block>().net_id;
        NetUnitData blockinfo = new NetUnitData(netID, block);

        //Debug.Log(netID + " " + blockinfo.fpos);
        lock(blocks)blocks.Add(netID, block);
        lock(blocksinfo)blocksinfo.Add(netID, blockinfo);
    }

    public void Close()
    {
        if (playerID != "0") Client.instance.DelListener("UpdateUnitInfo", UpdateUnitInfo);
        players.Clear();

        blocks.Clear();

        lastSendTime = float.MinValue;
        isInit = false;
    }
    public void Init(string id) 
    {
        if (isInit) return;
        isInit = true;

        playerID = id;
        Client.instance.AddListener("UpdateUnitInfo", UpdateUnitInfo);
    }
    public void StartFight() //发送初始化信息 
    {
        //发送自己的信息
        CharactAttribute info = new CharactAttribute(player);
        ProtocolBytes proto = info.GetInfoProto("AddPlayer");
        Client.instance.Send(proto);

        //房主或服务器
        /*
        if (playerID == "0") 
        {
            foreach (var item in GameController.instance.enemies)
            {
                item.Value.GetComponent<EnemyController>().Target = player;
                info = new CharactAttribute(item.Value);
                Client.instance.Send(info.GetInfoProto("AddEnemy"));
            }
            foreach (var item in GameController.instance.enemies2)
            {
                item.Value.GetComponent<EnemyController>().Target = player;
                info = new CharactAttribute(item.Value);
                Client.instance.Send(info.GetInfoProto("AddEnemy"));
            }
        }
        else
        {
            GameController.instance.ClearEnemy();
            Client.instance.AddListener("UpdateUnitInfo", UpdateUnitInfo);
        }
         * */

    }

    public void SendPos()
    {
        if (players.ContainsKey(playerID)) {
            if (players[playerID].active == true)
            {
                ProtocolBytes unitproto = NetUnitData.GetUnitData("UpdateUnitInfo", playerID, players[playerID].transform.position);

                Client.instance.Send(unitproto);
            }

        }

        foreach (var item in blocks)
        {
            if (item.Value.GetComponentInChildren<TransformController>() != null)
            {
                string netID = item.Value.GetComponentInChildren<TransformController>().controllerID;
                if (netID != playerID) continue;

                Block block = item.Value.GetComponent<Block>();
                ProtocolBytes unitproto = NetUnitData.GetUnitData("UpdateUnitInfo", block.net_id, item.Value.transform.position);

                Client.instance.Send(unitproto);
            }
        }
    }

    public void UpdateUnitInfo(ProtocolBase protocol)
    {        

        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        float z = proto.Getfloat(start, ref start);
        Vector3 pos = new Vector3(x, y, z);

        /*
        if (LastReceiveTime.ContainsKey(id))
        {
            if (LastReceiveTime[id] != 0)
            {
                if (Time.time - LastReceiveTime[id] > 0.26f)
                {
                    return;
                }
                LastReceiveTime[id] = Time.time;
            }
            else
            {
                LastReceiveTime[id] = Time.time;
            }
        }
        */

        UpdateUnitInfo(id, pos);
        
    }
    public void UpdateUnitInfo(string id, Vector3 pos)
    {
        //Debug.Log(id);
        if (blocksinfo.ContainsKey(id))
        {
            blocksinfo[id].Update(pos);
            //Debug.Log(blocksinfo[id].fpos);
        }
        if (playersinfo.ContainsKey(id))
        {
            playersinfo[id].Update(pos);
            //Debug.Log(playersinfo[id].fpos);
        }
    }

    public void Hit(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int isPlayer = proto.GetInt(start, ref start);
        int damage = proto.GetInt(start, ref start);

        GameObject temp;
        if (isPlayer == 1)
        {
            if (id == playerID || !players.ContainsKey(id)) return;
            temp = players[id];
        }
        else
        {
            if (playerID == "0" || !blocks.ContainsKey(id)) return;
            temp = blocks[id];
        }

    }

    // UDP speed up
    string id_last = "-1";
    int forward_last = -1;
    bool stand_last = false;
    float x_last = 0, y_last = 0, z_last = 0;

  
    public void Update()
    {
        //0.085,0.075,0.1,0.15,0.19,0.2
        if (Time.time - lastSendTime > 0.085f)
        {
            SendPos();
            lastSendTime = Time.time;
        }

        //Debug.Log("??");
        //Debug.Log(players.Count);

        foreach (var item in players)
        {
            //Debug.Log(item.Key);
            if (item.Value.active == false) continue;
            GameObject player = item.Value;
            string id = item.Key;
            Vector3 fpos = playersinfo[id].fpos;
            Vector3 pos = player.transform.position;

            
            if (player.GetComponent<PlayerController>().netID == playerID) continue;
            //Debug.Log(id + " " + fpos);
            lock(player)player.transform.position = Vector3.Lerp(pos, fpos, playersinfo[id].delta);
        }

        foreach (var item in blocks)
        {
            if (item.Value.GetComponentInChildren<TransformController>() != null)
            {
                string netID = item.Value.GetComponentInChildren<TransformController>().controllerID;
                if (netID == playerID) continue;
                
                //Debug.Log("Update" + item.Key + " " + blocksinfo[item.Key].fpos);

                GameObject block = item.Value;
                string id = item.Key;
                Vector3 fpos = blocksinfo[id].fpos;
                Vector3 pos = block.transform.position;

                lock (block) block.transform.position = Vector3.Lerp(pos, fpos, blocksinfo[id].delta);
            }
        }
    }
    
}
