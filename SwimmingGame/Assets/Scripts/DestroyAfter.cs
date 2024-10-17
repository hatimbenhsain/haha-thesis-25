using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time=1f;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer=0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(timer>=time){
            Destroy(gameObject);
        }
    }
}
