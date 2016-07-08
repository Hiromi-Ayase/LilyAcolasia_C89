using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class MenuStart : MonoBehaviour {

    public SpriteRenderer menuStartRenderer;
    public SpriteRenderer menuMainRenderer;
    public AudioClip soundSelect;

    public Digit warfareWin;
    public Digit warfareLost;
    public Digit warfareDraw;
	public Digit mimicWin;
	public Digit mimicLost;
	public Digit mimicDraw;
	public Digit humanWin;
	public Digit humanLost;
	public Digit humanDraw;
    public GameObject menuMain;

    private int frameCount = 0;
    private double duration = 15;
    private AudioSource audioSource;

    /// <summary>
    /// 0:init 1:init->main 2:main 3:main->game
    /// </summary>
    private int state = 0;

	void Start () {
        menuStartRenderer.color = new Color(1f, 1f, 1f, 1f);
        menuMain.SetActive(false);
        this.audioSource = GetComponent<AudioSource>();
    }

    void OnClick()
    {
        Debug.Log(this.name);
    }

    void Update () {

        if (this.state == 0)
        {
            menuStartRenderer.color = new Color(1f, 1f, 1f, (float)Math.Abs(Math.Sin((double)frameCount / duration)));
            this.frameCount++;

            if (Input.GetMouseButtonDown(0))
            {
                menuStartRenderer.color = new Color(0, 0, 0, 1f);
                state = 1;
                this.audioSource.clip = soundSelect;
                this.audioSource.Play();
            }
        }
        else if (this.state == 1)
        {
            menuStartRenderer.color -= new Color(0, 0, 0, 0.1f);
            if (menuStartRenderer.color.a <= 0)
            {
                menuMain.SetActive(true);

                warfareWin.Number = PlayerPrefs.GetInt("warfareWin", 0);
                warfareLost.Number = PlayerPrefs.GetInt("warfareLost", 0);
                warfareDraw.Number = PlayerPrefs.GetInt("warfareDraw", 0);

				mimicWin.Number = PlayerPrefs.GetInt("mimicWin", 0);
				mimicLost.Number = PlayerPrefs.GetInt("mimicLost", 0);
				mimicDraw.Number = PlayerPrefs.GetInt("mimicDraw", 0);

				humanWin.Number = PlayerPrefs.GetInt("humanWin", 0);
				humanLost.Number = PlayerPrefs.GetInt("humanLost", 0);
				humanDraw.Number = PlayerPrefs.GetInt("humanDraw", 0);

                Debug.Log(warfareLost.Number);
                this.state = 2;
            }
        }
    }
}
