using UnityEngine;
using System.Collections;
using System;

public class ColorChoice : MonoBehaviour {

    public delegate void OnClick(string color);
    private static ColorChoice colorChoice;

    private string[] COLOR = { "Y", "G", "B", "R", "W" };
    private OnClick onClick = null;
    private SpriteRenderer spriteRenderer;
	public int stateSpecial = 0;
	private static Vector3 spTargetScale = new Vector3(960, 960, 1);

    public static void Show(OnClick action)
    {
        colorChoice.onClick = action;
        colorChoice.gameObject.SetActive(true);
		colorChoice.transform.localScale = new Vector3(50, 50, 1);
		colorChoice.stateSpecial = 1;

        colorChoice.spriteRenderer.color = new Color(1f, 1f, 1f, 0);
    }

    void Awake()
    {
        colorChoice = this;
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        this.spriteRenderer.color += new Color(0, 0, 0, 0.2f);

		if (stateSpecial != 0) {
			Vector3 scale = this.transform.localScale;
			if (stateSpecial == 1) {
				scale += new Vector3 (100, 0, 0);
				if (scale.x > spTargetScale.x) {
					scale.x = spTargetScale.x;
					stateSpecial = 2;
				}
			} else if (stateSpecial == 2) {
				scale += new Vector3 (0, 100, 0);
				if (scale.y > spTargetScale.y) {
					scale.y = spTargetScale.y;
					stateSpecial = 0;
				}
			} else if (stateSpecial == 3) {
				scale -= new Vector3 (0, 100, 0);
				if (scale.y < 50) {
					scale.y = 50;
					stateSpecial = 4;
				}
			} else if (stateSpecial == 4) {
				scale -= new Vector3 (100, 0, 0);
				if (scale.x < 50) {
					scale.x = 50;
					stateSpecial = 0;
					this.gameObject.SetActive(false);
				}
			}
			this.transform.localScale = scale;
		}
    }

	void OnMouseDown()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector2 currentScreenPoint = new Vector2(x - screenPoint.x, y - screenPoint.y);
        int c = (int)((((Math.Atan2(currentScreenPoint.y + 35, currentScreenPoint.x) / Math.PI + 1) * 180 + 54) % 360) / 72);
        Debug.Log(COLOR[c]);
        onClick(COLOR[c]);
		stateSpecial = 3;
    }
}
