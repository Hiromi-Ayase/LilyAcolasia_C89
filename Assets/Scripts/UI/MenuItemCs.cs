using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuItemCs : MonoBehaviour
{
    public AudioClip soundSelect;
    private AudioSource audioSource;

	public GameObject title;
	private bool isFadeVolume = false;

    // Use this for initialization
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

	void Update() {
		if (isFadeVolume) {
			AudioSource src = title.GetComponents<AudioSource> () [1];
			src.volume = src.volume * 0.95f;
		}
	}

	void OnMouseDown()
    {
    
		PlayerPrefs.SetInt("network", 0);
        if (this.name == "WarfareArea")
        {
            this.audioSource.clip = soundSelect;
            this.audioSource.Play();
            CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
            {
                PlayerPrefs.SetInt("level", 4);
                SceneManager.LoadScene("Game");
            });

        }
		else if (this.name == "MimicArea")
		{
			this.audioSource.clip = soundSelect;
			this.audioSource.Play();
			CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
				{
					PlayerPrefs.SetInt("level", 1);
					SceneManager.LoadScene("Game");
				});

		}
		else if (this.name == "AchievementArea")
		{
			this.audioSource.clip = soundSelect;
			this.audioSource.Play();
			isFadeVolume = true;
			CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
				{
					SceneManager.LoadScene("Achievement");
				});

		}
        else if (this.name == "ExitArea")
        {
            this.audioSource.clip = soundSelect;
            this.audioSource.Play();
            CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.2f, () =>
            {
                Application.Quit();
            });

        }
    }
}
