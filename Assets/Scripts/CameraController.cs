using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float nc = 2.3f;

        float t = Time.time * 2.5f;
        float nx = Mathf.PerlinNoise(t, t + 0.0f) * 0.03f;
        float ny = Mathf.PerlinNoise(t + 10.0f, t + 15.0f) * nc;
        float nz = Mathf.PerlinNoise(t + 25.0f, t + 20.0f) * nc * 0.5f;
        Vector3 noise = new Vector3(nx, ny, nz);

        Quaternion noiseRot = Quaternion.Euler(noise.x, noise.y, noise.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, noiseRot, Time.deltaTime * 5.0f);
    }
}
