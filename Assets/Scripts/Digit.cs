using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Digit : MonoBehaviour {

    public int SIZE = 960;
	public int MARGIN = 1920;
    public int localPosFactor = 6;
    public GameObject digitPrefab;
	public int digitSize = 2;
    public string spriteName = "Texture/digit/number_64x64";

	private Sprite[] digits;
	private List<GameObject> digitObjList = new List<GameObject>();
	private List<SpriteRenderer> digitRdrList = new List<SpriteRenderer>();

    public long Number { get; set; }
    private long beforeNum = -1;


    void Start () {
        this.digits = Resources.LoadAll<Sprite>(spriteName);
			
		for (int i = 0; i < digitSize; i++) {
			GameObject digit = Instantiate (digitPrefab);
			SpriteRenderer renderer = digit.GetComponent<SpriteRenderer>();

			digit.transform.parent = this.transform;
			digit.transform.localScale = new Vector3(SIZE, SIZE, 0);
			digit.transform.localPosition = new Vector3 ((i - (-1.0f + digitSize) / 2) * MARGIN / localPosFactor, 0, 0);
			renderer.sortingOrder = 3;

			this.digitObjList.Add (digit);
			this.digitRdrList.Add (renderer);
		}
    }
	
	void Update () {
        if (this.Number != this.beforeNum)
        {
			this.beforeNum = this.Number;
			long now = this.Number;
			for (int i = digitSize - 1; i >= 0; i--) {
				this.digitRdrList [i].sprite = digits [now % 10];
				now /= 10;
			}
        }	    
	}
}
