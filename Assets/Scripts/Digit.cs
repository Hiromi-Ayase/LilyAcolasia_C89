using UnityEngine;
using System.Collections;

public class Digit : MonoBehaviour {

    public int SIZE = 960;
    public int localPosFactor = 6;
    public Sprite[] digits;
    public GameObject digitPrefab;
    public bool three = false;
    public string spriteName = "Texture/digit/number_64x64";

    private GameObject digit1;
    private GameObject digit2;
    private GameObject digit3;
    private SpriteRenderer digitRenderer1;
    private SpriteRenderer digitRenderer2;
    private SpriteRenderer digitRenderer3;

    public int Number { get; set; }
    private int beforeNum = -1;


    void Start () {
        this.digits = Resources.LoadAll<Sprite>(spriteName);
        this.digit1 = Instantiate(digitPrefab);
        this.digit2 = Instantiate(digitPrefab);
        this.digit3 = Instantiate(digitPrefab);

        this.digit1.transform.parent = this.transform;
        this.digit2.transform.parent = this.transform;
        this.digit3.transform.parent = this.transform;

        this.digitRenderer1 = this.digit1.GetComponent<SpriteRenderer>();
        this.digitRenderer2 = this.digit2.GetComponent<SpriteRenderer>();
        this.digitRenderer3 = this.digit3.GetComponent<SpriteRenderer>();


        this.digitRenderer1.sortingOrder = 3;
        this.digitRenderer2.sortingOrder = 3;
        this.digitRenderer3.sortingOrder = 3;

        if (!three)
        {
            this.digit1.transform.localScale = new Vector3(SIZE, SIZE, 0);
            this.digit1.transform.localPosition = new Vector3(-SIZE / localPosFactor, 0, 0);
            this.digit2.transform.localScale = new Vector3(SIZE, SIZE, 0);
            this.digit2.transform.localPosition = new Vector3(SIZE / localPosFactor, 0, 0);
            this.digit3.SetActive(false);
        }
        else
        {
            this.digit1.transform.localScale = new Vector3(SIZE, SIZE, 0);
            this.digit1.transform.localPosition = new Vector3(-SIZE / localPosFactor, 0, 0);
            this.digit2.transform.localScale = new Vector3(SIZE, SIZE, 0);
            this.digit2.transform.localPosition = new Vector3(0, 0, 0);
            this.digit3.transform.localScale = new Vector3(SIZE, SIZE, 0);
            this.digit3.transform.localPosition = new Vector3(SIZE / localPosFactor, 0, 0);
        }
    }
	
	void Update () {
        if (this.Number != this.beforeNum)
        {
            if (this.three)
            {
                this.beforeNum = this.Number;
                this.digitRenderer1.sprite = digits[this.Number / 100];
                this.digitRenderer2.sprite = digits[(this.Number % 100) / 10];
                this.digitRenderer3.sprite = digits[this.Number % 10];
            }
            else
            {
                this.beforeNum = this.Number;
                this.digitRenderer1.sprite = digits[this.Number / 10];
                this.digitRenderer2.sprite = digits[this.Number % 10];
            }
        }	    
	}
}
