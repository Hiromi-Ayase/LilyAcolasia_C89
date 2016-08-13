using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
	public const int WARFARE_1 = 1;
	public const int WARFARE_5 = 2;
	public const int WARFARE_20 = 3;
	public const int WARFARE_50 = 4;
	public const int HUMAN_1 = 5;
	public const int HUMAN_5 = 6;
	public const int HUMAN_20 = 7;
	public const int HUMAN_50 = 8;
	public const int CONCOLOR = 9;
	public const int EQUIV = 10;
	public const int DRAW = 11;
	public const int PERFECT = 11;


	private const string KEY_WARFARE_WIN = "warfareWin";
	private const string KEY_WARFARE_LOST = "warfareLost";
	private const string KEY_WARFARE_DRAW = "warfareDraw";
	private const string KEY_MIMIC_WIN = "mimicWin";
	private const string KEY_MIMIC_LOST = "mimicLost";
	private const string KEY_MIMIC_DRAW = "mimicDraw";
	private const string KEY_HUMAN_WIN = "humanWin";
	private const string KEY_HUMAN_LOST = "humanLost";
	private const string KEY_HUMAN_DRAW = "humanDraw";
	private const string KEY_CONCOLOR = "concolor";
	private const string KEY_EQUIV = "equiv";
	private const string KEY_DRAW = "draw";
	private const string KEY_PERFECT = "perfect";

	private const int INTERVAL = 300;

	private int warfareWin = 0;
	private int humanWin = 0;
	private int mimicWin = 0;
	private int warfareLost = 0;
	private int humanLost = 0;
	private int mimicLost = 0;
	private int warfareDraw = 0;
	private int humanDraw = 0;
	private int mimicDraw = 0;

	private int concolor = 0;
	private int equiv = 0;
	private int perfect = 0;

	private Queue<int> events = new Queue<int>();

	public Queue<int> Events {
		get { return this.events; }
	}

	public bool IsHumanWin1 {
		get { return humanWin >= 1; }
	}
	public bool IsHumanWin5 {
		get { return humanWin >= 5; }
	}
	public bool IsHumanWin20 {
		get { return humanWin >= 20; }
	}
	public bool IsHumanWin50 {
		get { return humanWin >= 50; }
	}
	public bool IsWarfareWin1 {
		get { return warfareWin >= 1; }
	}
	public bool IsWarfareWin5 {
		get { return warfareWin >= 5; }
	}
	public bool IsWarfareWin20 {
		get { return warfareWin >= 20; }
	}
	public bool IsWarfareWin50 {
		get { return warfareWin >= 50; }
	}
	public bool IsConcolor {
		get { return concolor >= 1; }
	}
	public bool IsEquiv {
		get { return equiv >= 1; }
	}
	public bool IsDraw {
		get { return mimicDraw >= 1 || humanDraw >= 1 || warfareDraw >= 1; }
	}
	public bool IsPerfect {
		get { return perfect >= 1; }
	}

	public void WarfareWin() {
		warfareWin++;
		PlayerPrefs.SetInt (KEY_WARFARE_WIN, warfareWin);
		PlayerPrefs.Save ();

		if (warfareWin == 1) {
			events.Enqueue (WARFARE_1);
		} else if (warfareWin == 5) {
			events.Enqueue (WARFARE_5);
		} else if (warfareWin == 20) {
			events.Enqueue (WARFARE_20);
		} else if (warfareWin == 50) {
			events.Enqueue (WARFARE_50);
		}
	}

	public void WarfareLost() {
		warfareLost++;
		PlayerPrefs.SetInt (KEY_WARFARE_LOST, warfareLost);
		PlayerPrefs.Save ();
	}

	public void WarfareDraw() {
		warfareDraw++;
		PlayerPrefs.SetInt (KEY_WARFARE_DRAW, warfareDraw);
		PlayerPrefs.Save ();
	}


	public void HumanWin () {
		humanWin++;
		PlayerPrefs.SetInt (KEY_HUMAN_WIN, humanWin);
		PlayerPrefs.Save ();

		if (humanWin == 1) {
			events.Enqueue (HUMAN_1);
		} else if (humanWin == 5) {
			events.Enqueue (HUMAN_5);
		} else if (humanWin == 20) {
			events.Enqueue (HUMAN_20);
		} else if (humanWin == 50) {
			events.Enqueue (HUMAN_50);
		}
	}

	public void HumanLost() {
		humanLost++;
		PlayerPrefs.SetInt (KEY_HUMAN_LOST, humanLost);
		PlayerPrefs.Save ();
	}

	public void HumanDraw() {
		humanDraw++;
		PlayerPrefs.SetInt (KEY_HUMAN_DRAW, humanDraw);
		PlayerPrefs.Save ();
	}

	public void MimicWin () {
		mimicWin++;
		PlayerPrefs.SetInt (KEY_MIMIC_WIN, mimicWin);
		PlayerPrefs.Save ();
	}

	public void MimicLost() {
		mimicLost++;
		PlayerPrefs.SetInt (KEY_MIMIC_LOST, mimicLost);
		PlayerPrefs.Save ();
	}

	public void MimicDraw() {
		mimicDraw++;
		PlayerPrefs.SetInt (KEY_MIMIC_DRAW, mimicDraw);
		PlayerPrefs.Save ();
	}

	public void Concolor () {
		concolor++;
		PlayerPrefs.SetInt (KEY_CONCOLOR, concolor);
		PlayerPrefs.Save ();

		if (concolor == 1) {
			events.Enqueue (CONCOLOR);
		}
	}

	public void Equiv () {
		equiv++;
		PlayerPrefs.SetInt (KEY_EQUIV, concolor);
		PlayerPrefs.Save ();

		if (equiv == 1) {
			events.Enqueue (EQUIV);
		}
	}

	public void Perfect () {
		perfect++;
		PlayerPrefs.SetInt (KEY_PERFECT, perfect);
		PlayerPrefs.Save ();

		if (perfect == 1) {
			events.Enqueue (PERFECT);
		}
	}

	public void init() {
		warfareWin = PlayerPrefs.GetInt (KEY_WARFARE_WIN, 0);
		humanWin = PlayerPrefs.GetInt (KEY_HUMAN_WIN, 0);
		mimicWin = PlayerPrefs.GetInt (KEY_MIMIC_WIN, 0);
		warfareLost = PlayerPrefs.GetInt (KEY_WARFARE_LOST, 0);
		humanLost = PlayerPrefs.GetInt (KEY_HUMAN_LOST, 0);
		mimicLost = PlayerPrefs.GetInt (KEY_MIMIC_LOST, 0);
		warfareDraw = PlayerPrefs.GetInt (KEY_WARFARE_DRAW, 0);
		humanDraw = PlayerPrefs.GetInt (KEY_HUMAN_DRAW, 0);
		mimicDraw = PlayerPrefs.GetInt (KEY_MIMIC_DRAW, 0);

		concolor = PlayerPrefs.GetInt (KEY_CONCOLOR);
		equiv = PlayerPrefs.GetInt (KEY_EQUIV);
		perfect = PlayerPrefs.GetInt (KEY_PERFECT);
	}

	void Start ()
	{
		init ();
	}
}

