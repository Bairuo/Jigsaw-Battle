﻿using System;
using System.Reflection;

public class HandleServerMsg{

    //连接类协议
    public void DisConnect(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.ClosePlayer(conn);
    }
    public void HeatBeat(Conn conn, ProtocolBase protoBase)
    {
        conn.lastTickTime = Sys.GetTimeStamp();
    }
    public void AddRoom(Conn conn, ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string questid = proto.GetString(start, ref start);
        try
        {
            int quest = int.Parse(questid);
            if (ServerNet.instance.AddRoom(conn, quest))
            {
                ServerNet.instance.SendID(conn); //此函数目前加入一个空房间也相当于创建房间
                ServerNet.instance.SendRoomNum(conn);
                ServerNet.instance.HandleP2P(conn);
            }
        }
        catch
        {

        }
    }

    //（针对）战斗类协议

    public void Hit(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    } 
    public void Shoot(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }
    public void PlayerTurn(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }
    public void U(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].UDPBroadcastOthers(protoBase, conn.id);
    }

    //(通用）战斗类协议
    public void EnterBlock(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }
    public void PlayerInit(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
        //ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void AreaInit(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }
    public void BlockGenerate(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }


    public void AddPlayer(Conn conn, ProtocolBase protoBase) //战斗场景初始化玩家
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void AddEnemy(Conn conn, ProtocolBase protoBase) //战斗场景初始化敌人
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void UpdateInfo(Conn conn, ProtocolBase protoBase)
    {
        //服务器在此处可以有作弊校验的功能（先存入房间数据？)，然而这里只负责转发....
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }
    public void UpdateUnitInfo(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].BroadcastOthers(protoBase, conn.id);
    }

    //流程类协议

    public void NextLevel(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void StartScene(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void Prepare(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].prepare++;
    }

    public void Pause(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void CurtainStart(Conn conn, ProtocolBase protoBase)
    {
        if (ServerNet.instance.rooms[conn.roomid].prepare == ServerNet.instance.rooms[conn.roomid].num)
        {
            ServerNet.instance.rooms[conn.roomid].prepare = 0;
            ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
        }

    }
    public void ReStart(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
    public void Return(Conn conn, ProtocolBase protoBase)
    {
        ServerNet.instance.rooms[conn.roomid].Broadcast(protoBase);
    }
}
