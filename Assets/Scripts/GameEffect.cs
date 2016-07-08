using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameEffect : MonoBehaviour {

    public Sprite skill1Active;
    public Sprite skill3Active;
    public Sprite skill5Active;
    public Sprite skill7Active;
    public Sprite skill9ActiveEnemy;
    public Sprite skill9ActiveYour;
    public Sprite skill1CutIn;
    public Sprite skill3CutIn;
    public Sprite skill5CutIn;
    public Sprite skill7CutIn;
    public Sprite skill9CutIn;
    public Sprite endLose;
    public Sprite endWin;
    public Sprite endDraw;
    public Sprite gameEndLose;
    public Sprite gameEndWin;
    public Sprite gameEndDraw;

    public GameObject cutIn;
    public GameObject special;
    public GameObject special9;
    public GameObject roundEnd;
    public GameObject gameEnd;

    public SpriteRenderer cutInRenderer;
    public SpriteRenderer specialRenderer;
    public SpriteRenderer special9Renderer;
    public SpriteRenderer roundEndRenderer;
    public SpriteRenderer gameEndRenderer;

	private List<SpriteRenderer> fields;
    private static GameEffect instance;
    private bool isCutIn = false;
	private bool isRoundEnd = false;
    private bool isGameEnd = false;
    private int frameCounter = 0;
	private int blinkCounter = 0;

	private static int stateSpecial = 0;
	private static GameObject sp;
	private static Vector3 spTargetScale = new Vector3(960, 960, 1);

    private const int CUTIN_SPEED1 = 3;
    private const int CUTIN_SPEED2 = 100;
    private const int CUTIN_LEFT = -1000;
    private const int ROUND_END1 = 4000;
    private const int ROUND_END2 = 2000;
	private const int BLINK = 30;

    public static bool IsCutIn { get { return instance.isCutIn; } }
    public static bool IsRoundEnd { get { return instance.isRoundEnd; } }
    public static bool IsGameEnd { get { return instance.isGameEnd; } }

    private float debugFactor = 1;

    void Start () {
        instance = this;
		fields = new List<SpriteRenderer> ();
		foreach (Transform child in GameObject.Find("FieldWrapper").GetComponentInChildren<Transform>()) {
			fields.Add (child.GetComponent<SpriteRenderer>());
		}
	}
	
	void Update () {
        if (this.isCutIn)
        {
            float rate = (float)Math.Abs(this.cutIn.transform.position.x) / Math.Abs(CUTIN_LEFT);
            float vx = (rate * CUTIN_SPEED2 + CUTIN_SPEED1) * debugFactor;
            float alpha = 1 - rate;
        
            instance.cutInRenderer.color = new Color(1f, 1f, 1f, alpha);
            instance.cutIn.transform.localPosition += new Vector3(vx, 0, 0);

            if (this.cutIn.transform.localPosition.x > -CUTIN_LEFT)
            {
                this.isCutIn = false;
                instance.cutInRenderer.sprite = null;
            }
        }

        if (this.isRoundEnd) {
            this.frameCounter ++;
            this.roundEnd.transform.localScale *= 0.9f;
            this.roundEndRenderer.color -= new Color(0, 0, 0, 0.01f * debugFactor);
            this.roundEnd.transform.localPosition = new Vector3(0, (float)Math.Sin((double)this.frameCounter / 4) * ROUND_END2, 0);
            if (this.roundEndRenderer.color.a <= 0)
            {
                this.isRoundEnd = false;
            }
        }

        if (this.isGameEnd)
        {
            this.gameEndRenderer.color += new Color(0, 0, 0, 0.01f * debugFactor);
            if (this.gameEndRenderer.color.a >= 1)
            {
                this.isGameEnd = false;
            }
        }

		if (stateSpecial != 0) {
			Vector3 scale = sp.transform.localScale;
			if (stateSpecial == 1) {
				scale += new Vector3 (100, 0, 0);
				if (scale.x > spTargetScale.x) {
					scale.x = spTargetScale.x;
					stateSpecial = 2;
				}
			} else {
				scale += new Vector3 (0, 100, 0);
				if (scale.y > spTargetScale.y) {
					scale.y = spTargetScale.y;
					stateSpecial = 0;
				}
			}
			sp.transform.localScale = scale;
		}

		if (fields != null && !this.isCutIn && !this.isRoundEnd && !this.isGameEnd) {
			this.blinkCounter++;
			float alpha = (float)Math.Sin((double)this.blinkCounter / BLINK) / 4 + 0.75f;
			foreach (SpriteRenderer sr in fields) {
				var color = sr.color;
				color.a = alpha;
				sr.color = color;
			}
		}
	}

    public static void GameEnd(int winner)
    {
        instance.isGameEnd = true;
        instance.gameEndRenderer.color = new Color(1f, 1f, 1f, 0f);
        if (winner == 0)
        {
            instance.gameEndRenderer.sprite = instance.gameEndLose;
            Debug.Log(instance.endLose);
        }
        else if (winner == 1)
        {
            instance.gameEndRenderer.sprite = instance.gameEndWin;
        }
		else if (winner == 2)
		{
			instance.gameEndRenderer.sprite = instance.gameEndDraw;
		}
		else
		{
			instance.gameEndRenderer.sprite = instance.gameEndDraw;
		}
    }

    /// <summary>
    /// 0:AI 1:Player 2:Even
    /// </summary>
    public static void RoundEnd(int winner)
    {
        return;
		/*
        instance.frameCounter = 0;
        instance.isRoundEnd = true;
        instance.roundEnd.transform.localScale = new Vector3(ROUND_END1, ROUND_END1, 0);
        instance.roundEndRenderer.color = new Color(1f, 1f, 1f, 1f);
        instance.roundEnd.transform.localPosition = new Vector3();

        if (winner == 0)
        {
            instance.roundEndRenderer.sprite = instance.endLose;
        }
        else if (winner == 1)
        {
            instance.roundEndRenderer.sprite = instance.endWin;
        }
        else
        {
            instance.roundEndRenderer.sprite = instance.endDraw;
        }
        */
    }

    public static void CutIn(int n)
    {
        Sprite sprite = null;
        if (n == 1)
            sprite = instance.skill1CutIn;
        else if (n == 3)
            sprite = instance.skill3CutIn;
        else if (n == 5)
            sprite = instance.skill5CutIn;
        else if (n == 7)
            sprite = instance.skill7CutIn;
        else if (n == 9)
            sprite = instance.skill9CutIn;

        instance.cutInRenderer.sprite = sprite;
        instance.cutInRenderer.color = new Color(1f, 1f, 1f, 0);
        instance.cutIn.transform.localPosition = new Vector3(CUTIN_LEFT, 0, 0);
        instance.isCutIn = true;
    }

    public static void Special(int n, int turn)
    {
        Sprite sprite = null;
        if (n == 1)
            sprite = instance.skill1Active;
        else if (n == 3)
            sprite = instance.skill3Active;
        else if (n == 5)
            sprite = instance.skill5Active;
        else if (n == 7)
            sprite = instance.skill7Active;

		if (turn == 1)
        {
            instance.specialRenderer.transform.rotation = new Quaternion(0, 0, 0, 0);
            instance.special.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            instance.specialRenderer.transform.rotation = new Quaternion(0, 0, 180f, 0);
            instance.special.transform.localPosition = new Vector3(-1250, 1250, 0);
        }

		if (n % 2 == 1) {
			setSpecial (instance.special);
		}
		instance.specialRenderer.sprite = sprite;
    }

    public static void Special9(int turn)
    {
		if (turn == -1) {
			instance.special9Renderer.sprite = null;
		}
        else if (turn == 1)
        {
			if (instance.special9Renderer.sprite != instance.skill9ActiveYour) {
				setSpecial (instance.special9);
				instance.special9Renderer.sprite = instance.skill9ActiveYour;
			}
        }
        else
        {
			if (instance.special9Renderer.sprite != instance.skill9ActiveEnemy) {
				setSpecial (instance.special9);
				instance.special9Renderer.sprite = instance.skill9ActiveEnemy;
			}
        }
    }


	private static void setSpecial(GameObject instance) {
		sp = instance;
		sp.transform.localScale = new Vector3(50, 50, 1);
		stateSpecial = 1;
	}
}


