﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class NetworkStartBase : MonoBehaviour
{

	public GameObject networkUI;
	public GameObject menuCreate;
	public GameObject menuCreate2;
	public GameObject menuJoin;
	public GameObject menuNet;
	public GameObject menuMain;
	public Text statusText;
	public InputField input;
	public Digit eFieldId;
	public Digit eFieldIdClient;
	public bool isClient = false;
	public AddressInputField addressInputField;

	void Start ()
	{
		networkUI.SetActive (false);
		menuCreate.SetActive (false);
		menuCreate2.SetActive (false);
		menuJoin.SetActive (false);
		menuNet.SetActive (true);
	}
}

