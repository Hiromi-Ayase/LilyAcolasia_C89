using UnityEngine;
using System.Collections;

public class MenuItemNetwork : MonoBehaviour {

	public AudioClip soundSelect;
	private AudioSource audioSource;

	public GameObject menu;
	public GameObject networkMenu;

	void Start () {
		this.audioSource = GetComponent<AudioSource>();
	}
	void OnMouseDown() {
		this.audioSource.clip = soundSelect;
		this.audioSource.Play();
		this.networkMenu.SetActive(true);
		this.menu.SetActive (false);
	}
}
