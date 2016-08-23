using UnityEngine;
using System.Collections;
using System;
using System.Text;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;


public class NetworkStart : MonoBehaviour {
	private const int DEFAULT_PORT = 8856;

	public static PhotonNetworkPlayer user = null;
	public NetworkStartBase net;
	public MenuStart ms;

	private int frameCount = 0;

	void Start() {
	}

	void Update() {
		frameCount++;
		if (frameCount % 300 == 0 && user != null) {
			if (user.IsServer) {
				if (user.Accept ()) {
					DontDestroyOnLoad (user.gameObject);

					CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
						{
							net.menuNet.SetActive (true);
							net.menuCreate2.SetActive (false);
							GameObject.Find("NetworkMenu").SetActive (false);

							PlayerPrefs.SetInt("level", 0);
							PlayerPrefs.SetInt("network", 1);
							SceneManager.LoadScene("Game");
						});
				}
			} else {
				if (user.Connect ()) {
					DontDestroyOnLoad (user.gameObject);

					CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
						{
							net.menuNet.SetActive (true);
							net.menuJoin.SetActive (false);
							GameObject.Find("NetworkMenu").SetActive (false);

							PlayerPrefs.SetInt("network", 2);
							SceneManager.LoadScene("Game");
						});
				}
			}
		}
	}

	void OnMouseDown()
	{
		net.statusText.text = "";
		if (this.name == "menu_item_network_create") {
			net.input.text = "";
			net.menuNet.SetActive (false);
			net.menuCreate2.SetActive (true);
			net.networkUI.SetActive (false);
			net.isClient = false;

			enterServer ();
			ms.seSelect ();
		} else if (this.name == "menu_item_network_join") {
			net.eFieldId.Text = net.input.text;
			net.input.text = "";
			net.isClient = true;
			net.menuNet.SetActive (false);
			net.menuJoin.SetActive (true);
			net.networkUI.SetActive (true);
			ms.seSelect ();
		} else if (this.name == "menu_item_back_join") {
			close ();
			net.menuNet.SetActive (true);
			net.menuJoin.SetActive (false);
			net.networkUI.SetActive (false);
			net.input.text = "";
			ms.seCancel ();
		} else if (this.name == "menu_item_back_create2") {
			close ();
			net.menuNet.SetActive (true);
			net.menuCreate2.SetActive (false);
			net.networkUI.SetActive (false);
			ms.seCancel ();
		} else if (this.name == "menu_item_enter") {
			enterClient ();
			ms.seSelect ();
		} else if (this.name == "menu_item_back") {
			GameObject.Find ("NetworkMenu").SetActive (false);
			net.menuMain.SetActive (true);
			ms.seCancel ();
		}
	}

	private void close() {
		if (user != null) {
			user.Close ();
			user = null;
			Debug.Log ("Connection close.");
		}
	}

	public void enterClient() {
		user = net.userInstance;
		if (net.eFieldIdClient.Text == "" || net.eFieldIdClient.Text == null) {
			net.eFieldIdClient.Text = net.input.text;
		}
		string id = net.eFieldIdClient.Text;
		long seed;

		if (!decode (id, out seed)) {
			net.statusText.text = "Invalid ID!";
		} else {
			net.statusText.text = "";
			user.Rand = seed;
			user.joinRoom (id);
			Debug.Log ("E Filed ID: " + net.eFieldIdClient.Text);
		}
	}

	public void enterServer () {
		user = net.userInstance;
		string name = user.createRoom ();
		net.eFieldId.Text = name;
		Debug.Log ("Server E Filed ID: " + net.eFieldId.Text);
	}

	private static bool decode (string id, out long seed) {
		if (id.Length != 8) {
			seed = 0;
			return false;
		}
		seed = 0;
		foreach (char c in id.ToCharArray()) {
			if (c < 'a' || c > 'z')
				return false;
			seed = (seed * 26) + (int)(c - 'a');
		}
		return true;
	}

}
