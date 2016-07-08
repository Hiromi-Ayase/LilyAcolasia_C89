using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DragUI : MonoBehaviour
{
    private const int ONCLICK_ORDER = 1000;
    private const int ONMOVE_ORDER = 400;
    private const int FIELD_SHFIT = 350;
    private static Vector3 Z_SHIFT = new Vector3(0, 0, -0.1f);
    public delegate bool OperationHandler(string from, string to, string card);
    private static Dictionary<char, string> COLOR2FILE = new Dictionary<char,string>() {
                                            {'Y', "black"},
                                            {'W', "white"},
                                            {'G', "green"},
                                            {'B', "blue"},
                                            {'R', "red"},
    };
    public Sprite rearTexture;
    public GameObject sameBonusPrefab;
    public GameObject equivBonusPrefab;
    public GameObject concolorBonusPrefab;

    private static Dictionary<string, Vector3> POSITION_SHIFT = new Dictionary<string, Vector3>()
    {
        {"NorthHand", new Vector3(1800, 0, 0)},
        {"SouthHand", new Vector3(1800, 0, 0)},
        {"NorthField0", new Vector3(0, FIELD_SHFIT, 0)},
        {"NorthField1", new Vector3(0, FIELD_SHFIT, 0)},
        {"NorthField2", new Vector3(0, FIELD_SHFIT, 0)},
        {"NorthField3", new Vector3(0, FIELD_SHFIT, 0)},
        {"NorthField4", new Vector3(0, FIELD_SHFIT, 0)},
        {"SouthField0", new Vector3(0, -FIELD_SHFIT, 0)},
        {"SouthField1", new Vector3(0, -FIELD_SHFIT, 0)},
        {"SouthField2", new Vector3(0, -FIELD_SHFIT, 0)},
        {"SouthField3", new Vector3(0, -FIELD_SHFIT, 0)},
        {"SouthField4", new Vector3(0, -FIELD_SHFIT, 0)},
        {"Trash", new Vector3(0, 1f, 0)},
        {"Talon", new Vector3(0, 1f, 0)},
    };


    private bool isEasing = false;
    private string id;
    private GameMaster master;
    private Vector3 target;
    private Sprite texture;
    private SpriteRenderer spriteRenderer;
    private int preOrder = 0;
    private int targetOrder = 0;
	private GameObject talon;

    private GameObject sameBonus;

    private Vector3 screenPoint;
    private Vector3 offset;
    private OperationHandler handler;

    public string Id { get { return this.id; } }
    public bool IsEasing { get { return this.isEasing; } }
    public bool SameBonus
    {
        set
        {
            if (sameBonus != null)
                this.sameBonus.SetActive(value);
        }
    }

    void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        int num = this.id[1] - '0';
        string color = COLOR2FILE[this.id[0]];
        this.texture = Resources.Load<Sprite>("Texture/card/" + color + "_" + num);
        this.sameBonus = Instantiate(sameBonusPrefab);
        this.sameBonus.transform.parent = this.transform;
        this.sameBonus.SetActive(false);
        this.sameBonus.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        this.sameBonus.transform.localPosition = new Vector3(0, 0.84f);
        
        setTexture();
		refreshTalon();
    }



    void OnMouseDown()
    {
        this.screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        this.offset = this.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(x, y, this.screenPoint.z));
        if (!isEasing)
        {
            this.target = this.transform.position;
        }
        else
        {
            isEasing = false;
        }
        this.preOrder = this.spriteRenderer.sortingOrder;
        this.spriteRenderer.sortingOrder = ONCLICK_ORDER;
    }

    void OnMouseDrag()
    {
        if (this.offset != Vector3.zero)
        {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;

            Vector3 currentScreenPoint = new Vector3(x, y, this.screenPoint.z);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
            this.transform.position = currentPosition;
        }
    }

    void OnMouseUp()
    {
        if (this.offset != Vector3.zero)
        {
            this.offset = Vector3.zero;
            GameObject parent = SearchParent();
            if (parent != null)
            {
                if (this.handler(this.transform.parent.name, parent.name, this.id))
                {
                    Move(parent.transform);
                }
                else
                {
                    MoveBack();
                }
            }
            else
            {
                MoveBack();
            }
        }
    }


    private GameObject SearchParent()
    {
        GameObject scene = GameObject.Find("GameScene");
        Vector3 pos = this.transform.position - scene.transform.position;
        foreach (GameObject parent in this.master.CardParents)
        {
            Vector3 target = parent.transform.position;
            Vector3 scale = parent.transform.localScale;
            Vector3 t1 = target - scale * 5;
            Vector3 t2 = target + scale * 5;
            if (t1.x <= pos.x && pos.x <= t2.x && t1.y <= pos.y && pos.y <= t2.y)
            {
                return parent;
            }
        }
        return null;
    }
    
    void Update()
    {
        if (isEasing)
        {
            Vector3 diff = target - transform.position;
            Vector3 v3 = diff * 0.4f;
            transform.position += v3;
            if (diff.magnitude < 0.01f)
            {
                isEasing = false;
                transform.position = target;
                if (targetOrder > 0)
                {
                    this.spriteRenderer.sortingOrder = this.targetOrder;
                    this.targetOrder = 0;
                }
            }

        }
    }


    public void Init(string name, OperationHandler handler, GameMaster master)
    {
        this.id = name;
        this.handler = handler;
        this.master = master;
    }

    public void MoveBack()
    {
        this.isEasing = true;
        this.spriteRenderer.sortingOrder = this.preOrder;
    }

    public void Move()
    {
        Transform parent = this.transform.parent;
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        int index = -1;
        foreach (Transform child in children)
        {
            if (child != parent && child.name.StartsWith("Card"))
            {
                index++;
            }
            if (child == this.transform)
            {
                break;
            }
        }

        this.isEasing = true;
        if (POSITION_SHIFT.ContainsKey(parent.name))
        {
            Vector3 shift = POSITION_SHIFT[parent.name];
            float s = ((float)index - ((float)children.Length - 2) / 2);
            if (parent.name.Contains("Field"))
            {
                s = index;
            }
            this.target = parent.transform.position + shift * s + Z_SHIFT * index;
        }
        else
        {
            this.target = parent.transform.position;
        }
        if (this.targetOrder == 0)
        {
            this.spriteRenderer.sortingOrder = index + 200;
        }
        else
        {
            this.targetOrder = index + 200;
            this.spriteRenderer.sortingOrder = ONMOVE_ORDER;
        }
    }

    public void Move(Transform target)
    {
        Transform before = this.transform.parent;
        this.transform.parent = target;

		this.targetOrder = 1;
        setTexture();
		refreshTalon();

		foreach (Transform child in target.GetComponentsInChildren<Transform>())
        {
            if (child != target && child.name.StartsWith("Card"))
            {
                child.gameObject.GetComponent<DragUI>().SendMessage("Move");
            }
        }

        foreach (Transform child in before.GetComponentsInChildren<Transform>())
        {
            if (child != before && child.name.StartsWith("Card"))
            {
                child.gameObject.GetComponent<DragUI>().SendMessage("Move");
            }
        }
    }

	public void refreshTalon() {
		if (this.talon == null) {
			this.talon = GameObject.Find ("Talon");
		}
		List<SpriteRenderer> list = new List<SpriteRenderer> ();
		foreach (Transform child in this.talon.GetComponentsInChildren<Transform>()) {
			if (child != talon && child.name.StartsWith ("Card")) {
				list.Add (child.gameObject.GetComponent<SpriteRenderer> ());
			}
		}
		list.Sort ((a, b) => b.sortingOrder - a.sortingOrder);
		for (int i = 0; i < list.Count; i++) {
			if (i < 1) {
				list [i].sprite = rearTexture;
			} else {
				list [i].sprite = null;
			}
		}
	}

	private void setTexture()
    {
        string name = this.transform.parent.name;
		if (name == "SouthHand" || name == "Trash" || name.Contains ("Field")) {
			this.spriteRenderer.sprite = texture;
		}
        else
        {
            this.spriteRenderer.sprite = rearTexture;
        }

        if (name.Contains("NorthField"))
        {
            this.transform.rotation = new Quaternion(0, 0, 180, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}