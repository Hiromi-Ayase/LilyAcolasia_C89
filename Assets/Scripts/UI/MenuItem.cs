using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuItem : MonoBehaviour
{
    public AudioClip soundSelect;
    private AudioSource audioSource;


    // Use this for initialization
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
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
