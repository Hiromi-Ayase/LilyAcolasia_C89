using UnityEngine;
using System.Collections;

public class MenuItemNetwork : MonoBehaviour {

	public AudioClip soundSelect;
	private AudioSource audioSource;

	private GameObject menu;
	private GameObject canvas;

	void Start () {
		this.audioSource = GetComponent<AudioSource>();
		this.menu = GameObject.Find ("MenuMain");
		this.canvas = GameObject.Find ("Canvas");
		this.canvas.SetActive (false);
	}
	void OnMouseDown() {
		this.audioSource.clip = soundSelect;
		this.audioSource.Play();
		this.canvas.SetActive(true);
		this.menu.SetActive (false);
	}
}
