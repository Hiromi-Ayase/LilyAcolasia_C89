using UnityEngine;
using System.Collections;

public class MenuItemNetwork : MonoBehaviour {

	public GameObject menu;
	public GameObject networkMenu;
	public MenuStart ms;

	void Start () {
	}
	void OnMouseDown() {
		ms.seSelect ();
		this.networkMenu.SetActive(true);
		this.menu.SetActive (false);
	}
}
