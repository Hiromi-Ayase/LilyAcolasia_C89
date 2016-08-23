using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon;


public class PhotonNetworkPlayer : Photon.MonoBehaviour
{
	private Queue<string> queue = new Queue<string> ();
	private PhotonView view;
	private bool isServer = false;
	private long seed = 0;

	public NetworkStartBase net;

	void Awake() {
	}

	void Start() {
		if (!PhotonNetwork.connected) {
			PhotonNetwork.ConnectUsingSettings ("0.1");
		}
		this.view = GetComponent<PhotonView> ();
	}


	void OnJoinedLobby(){
		Debug.Log ("Lobby join");
	}

	void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
		Debug.Log (codeAndMsg);
		net.statusText.text = "Join Room Failed.";
	}

	void OnPhotonCreateRoomFailed(object[] codeAndMsg) {
		Debug.Log (codeAndMsg);
		net.statusText.text = "Create Room Failed.";
	}

	public void createRoom(string name) {
		this.isServer = true;
		PhotonNetwork.CreateRoom (name);
	}

	public string createRoom() {
		joinLobby ();
		string name = "";
		seed = 0;
		this.isServer = true;
		for (int i = 0; i < 8; i++) {
			int x = UnityEngine.Random.Range (0, 26);
			seed = (seed * 26) + x;
			name += (char)('a' + x);
		}
		PhotonNetwork.CreateRoom(name);
		return name;
	}

	public void joinRoom(string name) {
		joinLobby ();
		this.isServer = false;
		PhotonNetwork.JoinRoom (name);
	}

	private void joinLobby() {
		if (!PhotonNetwork.insideLobby) {
			PhotonNetwork.JoinLobby ();
		}
	}

	public void send(string args) {
		view.RPC ("sendMessage", PhotonTargets.Others, args);
	}

	public string message() {
		lock (queue) {
			if (!IsConnected) {
				return null;
			} else if (queue.Count == 0) {
				return "";
			} else {
				return queue.Dequeue ();
			}
		}
	}

	public bool Accept() {
		return IsConnected;
	}

	public bool Connect() {
		return IsConnected;
	}

	public void Close() {
		if (PhotonNetwork.inRoom) {
			PhotonNetwork.LeaveRoom ();
		}
		if (PhotonNetwork.insideLobby) {
			PhotonNetwork.LeaveLobby ();
		}
	}

	public bool IsConnected {
		get { return PhotonNetwork.room != null && PhotonNetwork.otherPlayers.Length == 1; }
	}

	public bool IsServer {
		get { return isServer; }
	}

	public long Rand {
		get { return seed; }
		set { this.seed = value; }
	}

	[PunRPC]
	public void sendMessage(string msg) {
		lock (queue) {
			Debug.Log ("Message : " + msg);
			queue.Enqueue (msg);
		}
	}
}

