using UnityEngine;
using System.Collections;

public class MenuExit : MonoBehaviour
{

    private Material material;
    private bool fading = false;

    void Start()
    {
        this.material = this.GetComponent<Renderer>().material;
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        if (!fading)
        {
            fading = true;
            CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0.5f, () =>
            {
                Application.Quit();
            });
        }
    }

    void OnMouseEnter()
    {
        this.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.5f));
    }

    void OnMouseExit()
    {
        this.material.SetTextureOffset("_MainTex", new Vector2(0.5f, 0.5f));
    }

}
