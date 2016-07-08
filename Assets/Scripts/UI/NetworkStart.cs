using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;


public class NetworkStart : MonoBehaviour {
	public static LilyAcolasia.NetworkUser user = null;
	private int frameCount = 0;

	void Start() {
		user = null;
	}

	void Update() {
		frameCount++;
		if (frameCount % 100 == 0 && user != null) {
			if (user.IsServer) {
				if (user.Accept ()) {
					CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
						{
							PlayerPrefs.SetInt("level", 0);
							PlayerPrefs.SetInt("network", 1);
							SceneManager.LoadScene("Game");
						});
				}
			} else {
				if (user.Connect ()) {
					CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
						{
							PlayerPrefs.SetInt("network", 2);
							SceneManager.LoadScene("Game");
						});
				}
			}
		}
	}

	public void OnButtonDown () {
		InputField address = GameObject.Find ("AddressInputField").GetComponent<InputField> ();
		Toggle isServer = GameObject.Find ("ToggleServer").GetComponent<Toggle> ();
		Text buttonText = GameObject.Find ("ButtonText").GetComponent<Text> ();
		Text status = GameObject.Find ("StatusText").GetComponent<Text> ();
		if (user == null) {

			if (isServer.isOn) {
				Regex r = new Regex (@"^(\d+)$");
				Match m = r.Match (address.text);
				if (!m.Success) {
					status.text = "Invalid port...";
					return;
				}
				int port = Int32.Parse(m.Groups[1].Value);
				int rand = new System.Random().Next();
				status.text = "Waiting Client...";

				user = new LilyAcolasia.NetworkUser (port, "ServerPlayer", rand);
			} else {
				Regex r = new Regex (@"^([\d\.]+):(\d+)$");
				Match m = r.Match (address.text);
				if (!m.Success) {
					status.text = "Invalid host or port...";
					return;
				}
				int port = Int32.Parse (m.Groups[2].Value);
				string host = m.Groups[1].Value;

				user = new LilyAcolasia.NetworkUser (host, port, "ClientPlayer");
				status.text = "Connecting to server...";
			}

			buttonText.text = "Cancel";
			address.readOnly = true;
			isServer.interactable = false;

		} else {
			buttonText.text = "OK";
			user.Close ();
			user = null;
			address.readOnly = false;
			isServer.interactable = true;
			status.text = "Disconnected.";
		}
	}
}
