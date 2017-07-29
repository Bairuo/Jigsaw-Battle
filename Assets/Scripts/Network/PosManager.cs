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

    Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();
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


    void AddPlayer(string id, CharactAttribute info)
    {
        if (id != playerID)
        {
            prefab = Resources.Load(info.prefabname) as GameObject;
            Vector3 pos = new Vector3(info.x, info.y, info.z);
            GameObject player = (GameObject)UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity);
            NetUnitData playerinfo = new NetUnitData(id, player);
            /*
            player.GetComponent<Character>().netid = id;

            player.GetComponent<Character>().health = info.health;
            player.GetComponent<Character>().damage = info.damage;
            player.GetComponent<Character>().speed = info.speed;

            player.GetComponent<Character>().ShootClip = info.clip;
            player.GetComponent<Character>().ShootForce = info.addforce;
            player.GetComponent<Character>().ClipCalmtime = info.calmtime;

            player.GetComponent<Character>().ResourceInit(info.boltname, info.dropname);

            player.GetComponent<Character>().type = info.type;
            player.GetComponent<Character>().value = info.value;
            player.GetComponent<Character>().Probability = info.probability;
            player.GetComponent<Character>().Camp = info.camp;
            */
            lock(players)players.Add(id, player);
        }
    }
    public void AddPlayer(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);

        string prefabname = proto.GetString(start, ref start);
        float x = proto.Getfloat(start, ref start);
        float y = proto.Getfloat(start, ref start);
        float z = proto.Getfloat(start, ref start);

        int health = proto.GetInt(start, ref start);
        int damage = proto.GetInt(start, ref start);
        float speed = proto.Getfloat(start, ref start);

        int clip = proto.GetInt(start, ref start);
        int addforce = proto.GetInt(start, ref start);
        float calmtime = proto.Getfloat(start, ref start);

        string boltname = proto.GetString(start, ref start);
        string dropname = proto.GetString(start, ref start);

        int type = proto.GetInt(start, ref start);
        int value = proto.GetInt(start, ref start);
        float probability = proto.Getfloat(start, ref start);
        int camp = proto.GetInt(start, ref start);

        CharactAttribute info = new CharactAttribute(x, y, z, prefabname, health, damage, speed, clip, addforce, calmtime, boltname, dropname, type, value, probability, camp);

        AddPlayer(id, info);

    }
    void AddEnemy(string id, CharactAttribute info)
    {
        prefab = Resources.Load(info.prefabname) as GameObject;
        Vector3 pos = new Vector3(info.x, info.y, info.z);
        GameObject enemy = (GameObject)UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity);
        lock(enemies)enemies.Add(id, enemy);
        lock (LastReceiveTime) LastReceiveTime.Add(id, 0);

        enemy.SetActive(true);
        /*
        enemy.GetComponent<Character>().health = info.health;
        enemy.GetComponent<Character>().damage = info.damage;
        enemy.GetComponent<Character>().speed = info.speed;

        enemy.GetComponent<Character>().ShootClip = info.clip;
        enemy.GetComponent<Character>().ShootForce = info.addforce;
        enemy.GetComponent<Character>().ClipCalmtime = info.calmtime;

        enemy.GetComponent<Character>().ResourceInit(info.boltname, info.dropname);

        enemy.GetComponent<Character>().type = info.type;
        enemy.GetComponent<Character>().value = info.value;
        enemy.GetComponent<Character>().Probability = info.probability;
        enemy.GetComponent<Character>().Camp = info.camp;
        */
        //客户端在收到AddEnemy命令前一定已经加过房主角色
        //if (players.ContainsKey("0")) enemy.GetComponent<EnemyController>().Target = players["0"];
    }
    public void AddEnemy(ProtocolBase protocol)
    {
        if (playerID != "0")//非房主才需要初始化
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            string id = proto.GetString(start, ref start);

            string prefabname = proto.GetString(start, ref start);
            float x = proto.Getfloat(start, ref start);
            float y = proto.Getfloat(start, ref start);
            float z = proto.Getfloat(start, ref start);

            int health = proto.GetInt(start, ref start);
            int damage = proto.GetInt(start, ref start);
            float speed = proto.Getfloat(start, ref start);

            int clip = proto.GetInt(start, ref start);
            int addforce = proto.GetInt(start, ref start);
            float calmtime = proto.Getfloat(start, ref start);

            string boltname = proto.GetString(start, ref start);
            string dropname = proto.GetString(start, ref start);

            int type = proto.GetInt(start, ref start);
            int value = proto.GetInt(start, ref start);
            float probability = proto.Getfloat(start, ref start);
            int camp = proto.GetInt(start, ref start);

            CharactAttribute info = new CharactAttribute(x, y, z, prefabname, health, damage, speed, clip, addforce, calmtime, boltname, dropname, type, value, probability, camp);

            AddEnemy(id, info);
        }
    }


    public void Close()
    {
        if (playerID != "0") Client.instance.DelListener("UpdateUnitInfo", UpdateUnitInfo);
        players.Clear();

        enemies.Clear();

        lastSendTime = float.MinValue;
        isInit = false;
    }
    public void Init(string id) 
    {
        if (isInit) return;
        isInit = true;
        /*
        if (GameController.instance.Player != null)
        {
            player = GameController.instance.Player;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }


        player.GetComponent<Character>().netid = id;
        */
        playerID = id;
        players.Add(playerID, player);

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
        if (playerID == "0")
        {
            /*
            lock (GameController.instance.enemies)
            foreach (var item in GameController.instance.enemies)
            {
                if (item.Value.GetComponent<Character>().ISDeath) continue;
                ProtocolBytes unitproto = NetUnitData.GetUnitData("UpdateUnitInfo", item.Key.ToString(), item.Value.transform.position);
                Client.instance.Send(unitproto);
            }
            lock (GameController.instance.enemies2)
            foreach (var item in GameController.instance.enemies2)
            {
                if (item.Value.GetComponent<Character>().ISDeath) continue;
                ProtocolBytes unitproto = NetUnitData.GetUnitData("UpdateUnitInfo", item.Key.ToString(), item.Value.transform.position);
                Client.instance.Send(unitproto);
            }
             * */
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

        UpdateUnitInfo(id, pos);
        
    }
    public void UpdateUnitInfo(string id, Vector3 pos)
    {
        if (enemies.ContainsKey(id)) 
        {
            enemies[id].GetComponent<Transform>().position = pos;
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
            if (playerID == "0" || !enemies.ContainsKey(id)) return;
            temp = enemies[id];
        }

    }

    string id_last = "-1";
    int forward_last = -1;
    bool stand_last = false;
    float x_last = 0, y_last = 0, z_last = 0;

  
    public void Update()
    {
        //0.085,0.075,0.1,0.15,0.19,0.2
        if (Time.time - lastSendTime > 0.2f)
        {
            SendPos();
            lastSendTime = Time.time;
        }

    }
    
}
