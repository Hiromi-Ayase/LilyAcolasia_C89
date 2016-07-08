using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class ToggleServer : MonoBehaviour
{

	public void OnToggleServer() {
		InputField address = GameObject.Find ("AddressInputField").GetComponent<InputField> ();
		Text placeHolder = address.placeholder.GetComponent<Text> ();
		Toggle isServer = GameObject.Find ("ToggleServer").GetComponent<Toggle> ();

		if (!isServer.isOn) {
			placeHolder.text = "<Server IP Address>:<Port>";
		} else {
			placeHolder.text = "<Server Port>";
		}
	}

}

