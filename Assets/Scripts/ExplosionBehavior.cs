using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    public float lifeTime = 0.2f;
    public AudioClip explodeSFX;

    Material mat;

    public float maxRadius = 1;



    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(explodeSFX, Camera.main.transform.position);
        Destroy(gameObject, lifeTime);
        mat = GetComponent<Renderer>().material;
        transform.localScale = Vector3.one;
    }

    public void Initialize(float rad)
    {
        maxRadius = rad;
    }

    // Update is called once per frame
    void Update()
    {
        //lerp alpha value to make explosion more transparent over time
        Color c = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(mat.color.a, 0f, Time.deltaTime * (1/lifeTime)));
        mat.SetColor("_Color", c);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxRadius * 2, Time.deltaTime * 5);
    }
}
