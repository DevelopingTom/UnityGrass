using UnityEngine;
using UnityEngine.VFX;
using System;
using System.Collections;

[RequireComponent(typeof(VisualEffect))]
public class Splash : MonoBehaviour
{
    [SerializeField]
    float lifetime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<VisualEffect>().SetFloat("lifetime", lifetime);
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
