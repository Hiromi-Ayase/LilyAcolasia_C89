using System;
using UnityEngine;
using UnityEngine.UI;

public class AddressInputField : MonoBehaviour
{

	public NetworkStartBase nsb;


	void Start ()
	{
	}

	public void OnChange (string text) {
		if (nsb.isClient) {
			if (nsb.eFieldIdClient.Text != null && nsb.eFieldIdClient.Text != "") {
				nsb.eFieldIdClient.Text = "";
				Debug.Log (text);
			}
		}
	}

	public void OnEndEdit (string text) {
		if (nsb.isClient) {
			nsb.input.text = "";
			nsb.eFieldIdClient.Text = text.Substring (0, Math.Min (text.Length, 8));
			Debug.Log (text);
		}
	}
}

