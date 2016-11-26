using UnityEngine;
using System.Collections;
using System;

public class move : MonoBehaviour {

    public float speed;
    
    void FixedUpdate()
    {
        
        float distanceH = speed * Time.fixedDeltaTime * Input.GetAxis("Horizontal");
        float distanceV = (float)Math.Sqrt(4*(float)speed) * Time.fixedDeltaTime * Input.GetAxis("Vertical");
        transform.Translate(Vector3.right * distanceH);
        transform.Translate(Vector3.up * distanceV);
    }
}
