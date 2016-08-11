using UnityEngine;
using System.Collections;

public class EfiledId : MonoBehaviour {

	public int SIZE = 960;
	public int localPosFactor = 6;
	public Sprite[] digits;
	public GameObject digitPrefab;
	public int digitSize = 6;
	public string spriteName = "Texture/digit/title_menunum_48x48";

	private GameObject[] digitObjs;
	private SpriteRenderer[] digitRenderers;

	public int Number { get; set; }


	void Start () {
	}

	void Update () {
	}
}
