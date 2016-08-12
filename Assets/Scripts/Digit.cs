using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Digit : MonoBehaviour {
	private const int EMPTY_CHAR = 65;

	public int SIZE = 960;
	public int MARGIN = 1920;
    public int localPosFactor = 6;
    public GameObject digitPrefab;
	public int digitSize = 2;
    public string spriteName = "Texture/digit/number_64x64";
	public bool isAlpha = false;

	private Sprite[] digits;
	private List<GameObject> digitObjList = new List<GameObject>();
	private List<SpriteRenderer> digitRdrList = new List<SpriteRenderer>();

	public long Number { get; set; }
	public string Text { get; set; }
    private long beforeNum = -1;
	private string beforeText = null;

	private int[] map = new int[256];

    void Start () {
        this.digits = Resources.LoadAll<Sprite>(spriteName);
		setAlphaMap ();
			
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

	private void setAlphaMap() {
		for (int i = 0; i < 256; i++) {
			map [i] = EMPTY_CHAR;
		}
		for (int i = 0; i < 64; i++) {
			if (i < 26) {
				map ['A' + i] = i;
			} else if (i < 52) {
				map ['a' + i - 26] = i;
			} else if (i < 62) {
				map ['0' + i - 52] = i;
			}
		}
		map ['+'] = 62;
		map ['/'] = 63;
		map ['='] = 64;
	}
	
	void Update () {
		if (!isAlpha) {
			if (this.Number != this.beforeNum) {
				this.beforeNum = this.Number;
				long now = this.Number;
				for (int i = digitSize - 1; i >= 0; i--) {
					this.digitRdrList [i].sprite = digits [now % 10];
					now /= 10;
				}
			}	    
		} else {
			if (this.Text != this.beforeText) {
				this.beforeText = this.Text;

				for (int i = 0; i < digitSize; i ++) {
					int idx = i < this.Text.Length ? map [this.Text [i]] : EMPTY_CHAR;
					this.digitRdrList [i].sprite = digits [idx];
				}
			}	    
		}
	}
}
