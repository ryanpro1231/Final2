using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using System;

public class NetworkInput : MonoBehaviour {
    public static SocketIOComponent socket;
    string userName;
    public InputField nameInput;


	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("connected",OnConnect);
	}

    private void OnConnect(SocketIOEvent obj)
    {
        Debug.Log("We are farmers");
    }

    // Update is called once per frame
    public void GrabForumData () {
        userName = nameInput.text;
        Debug.Log(userName);
        JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
        data.AddField("name", userName);
	}
}
