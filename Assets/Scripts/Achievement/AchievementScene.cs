using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AchievementScene : MonoBehaviour {

	public Achievements achievements;
	public GameObject warfare_win_1;
	public GameObject warfare_win_5;
	public GameObject warfare_win_20;
	public GameObject warfare_win_50;
	public GameObject human_win_1;
	public GameObject human_win_5;
	public GameObject human_win_20;
	public GameObject human_win_50;
	public GameObject concolor;
	public GameObject equiv;
	public GameObject draw;
	public GameObject perfect;

	private AudioSource[] audioSource;
	public AudioClip soundSelect;
	public AudioClip bgm;

	private bool bgmFade = false;

	void Start () {
		achievements.init ();
		warfare_win_1.SetActive (achievements.IsWarfareWin1);
		warfare_win_5.SetActive (achievements.IsWarfareWin5);
		warfare_win_20.SetActive (achievements.IsWarfareWin20);
		warfare_win_50.SetActive (achievements.IsWarfareWin50);
		human_win_1.SetActive (achievements.IsHumanWin1);
		human_win_5.SetActive (achievements.IsHumanWin5);
		human_win_20.SetActive (achievements.IsHumanWin20);
		human_win_50.SetActive (achievements.IsHumanWin50);
		concolor.SetActive (achievements.IsConcolor);
		equiv.SetActive (achievements.IsEquiv);
		draw.SetActive (achievements.IsDraw);
		perfect.SetActive (achievements.IsPerfect);

		audioSource = GetComponents<AudioSource> ();

		this.audioSource[1].clip = bgm;
		this.audioSource[1].Play ();

	}

	void Update() {
		if (bgmFade) {
			this.audioSource [1].volume *= 0.9f;
		}
	}
	void OnMouseDown()
	{

		this.audioSource[0].clip = soundSelect;
		this.audioSource[0].Play ();
		bgmFade = true;
		CameraFade.StartAlphaFade (Color.black, false, 0.5f, 0.2f, () => {
			SceneManager.LoadScene ("Title");
		});
	}

}
