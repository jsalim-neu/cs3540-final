using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseBehavior : MonoBehaviour
{
    public AudioClip pulseSFX;
    Material mat;
    Color c;
    float currAlpha;
    

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        c = mat.color;
        mat.SetColor("_Color", new Color(c.r, c.g, c.b, 0));
        AudioSource.PlayClipAtPoint(pulseSFX, Camera.main.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        currAlpha = Mathf.Lerp(mat.color.a, 1f, Time.deltaTime * 5);
        mat.SetColor("_Color", new Color(c.r, c.g, c.b, currAlpha));
    }
}
