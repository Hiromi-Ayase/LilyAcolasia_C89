using UnityEngine;
using System.Collections;
using System;

public class ColorChoice : MonoBehaviour {

    public delegate void OnClick(string color);
    private static ColorChoice colorChoice;

    private string[] COLOR = { "Y", "G", "B", "R", "W" };
    private OnClick onClick = null;
    private SpriteRenderer spriteRenderer;

    public static void Show(OnClick action)
    {
        colorChoice.onClick = action;
        colorChoice.gameObject.SetActive(true);
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
        this.gameObject.SetActive(false);
    }
}
