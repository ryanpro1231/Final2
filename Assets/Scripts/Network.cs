using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour {
    static SocketIOComponent socket;
    public GameObject playerPrefab;
    public Spawner spawner;
    

    

	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("talkback", OnTalkBack);
        socket.On("spawn", OnSpawn);
        socket.On("move", OnMove);
        socket.On("disconnected", OnDisconnected);
        socket.On("register", OnRegister);
        socket.On("updatePosition", OnUpdatePosition);
        socket.On("requestPosition", OnRequestPosition);

    }

    private void OnRequestPosition(SocketIOEvent obj)
    {
        socket.Emit("updatePosition", PosToJson(spawner.localPlayer.transform.position, spawner.localPlayer.transform.localRotation.eulerAngles.z));
    }

    private void OnUpdatePosition(SocketIOEvent obj)
    {
        Debug.Log("Updating positions " + obj.data);

        //var v = float.Parse(obj.data["v"].ToString().Replace("\"", ""));
        //var h = float.Parse(obj.data["h"].ToString().Replace("\"", ""));
        var position = MakePosfromJson(obj);
        var rotation = obj.data["rotz"].n;
        var player = spawner.FindPlayer(obj.data["id"].ToString());

        

        player.transform.position = position;
        player.transform.eulerAngles = new Vector3(0, 0, rotation);
   
      
    }

    private void OnRegister(SocketIOEvent obj)
    {
        Debug.Log("Regisetered Player " + obj.data);
        spawner.AddPlayer(obj.data["id"].ToString(), spawner.localPlayer);
       
   
    }

    private void OnDisconnected(SocketIOEvent obj)
    {
        Debug.Log("Player disconnected " + obj.data);

        var id = obj.data["id"].ToString();

        spawner.RemovePlayer(id);
    }

    private void OnMove(SocketIOEvent obj)
    {
        //Debug.Log("Player Moving" + obj.data);
        var id = obj.data["id"].ToString();
        Debug.Log(id);

        var v = float.Parse(obj.data["v"].ToString().Replace("\"", ""));
        var h = float.Parse(obj.data["h"].ToString().Replace("\"", ""));

        var player = spawner.FindPlayer(id);

        var playerMover = player.GetComponent<PlayerMovementNetwork>();
        playerMover.v = v;
        playerMover.h = h;



    }

    private void OnSpawn(SocketIOEvent obj)
    {
        Debug.Log("Player Spawned" + obj.data);
        var player = spawner.SpawnPlayer(obj.data["id"].ToString());

        //spawn exisiting players at location
    }

    private void OnTalkBack(SocketIOEvent obj)
    {
        Debug.Log("The Server says Hello Back");
    }

    private void OnConnected(SocketIOEvent obj)
    {
        Debug.Log("Connected to Server");
        socket.Emit("sayhello");
        socket.Emit("senddata");
    }

    static public void Move(float currentPosV, float currentPosH)
    {
        //Debug.Log("Send Position to Server" + VectorToJson(currentPos));
        socket.Emit("move", new JSONObject(VectorToJson(currentPosV, currentPosH)));
    }

    public static string VectorToJson(float dirV, float dirH)
    {
        return string.Format(@"{{""v"":""{0}"",""h"":""{1}""}}", dirV, dirH);
    }
    public static JSONObject PosToJson(Vector3 pos, float rotz)
    {
        JSONObject jPos = new JSONObject(JSONObject.Type.OBJECT);
        jPos.AddField("x", pos.x);
        jPos.AddField("y", pos.y);
        jPos.AddField("z", pos.z);
        jPos.AddField("rotz",rotz);
        return jPos;
    }
    public static Vector3 MakePosfromJson(SocketIOEvent e)
    {

        return new Vector3(e.data["x"].n, e.data["y"].n, e.data["z"].n);
    }

}
