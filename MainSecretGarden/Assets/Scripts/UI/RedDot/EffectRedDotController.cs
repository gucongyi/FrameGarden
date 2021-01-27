using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRedDotController : MonoBehaviour
{
    public float speed = 60f;
    float z = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        z += speed * Time.deltaTime;
        z = z % 360f;
        transform.localEulerAngles = new Vector3(0f,0f, z);
    }
}
