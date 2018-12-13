using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour {
    [SerializeField]
    private Road road;

    [SerializeField]
    private Chara chara;

    [SerializeField]
    private Particle particle;

    [SerializeField]
    private GodRay godray;

    [SerializeField]
    private Cloud cloud;

    [SerializeField]
    private CameraController cameraCtl;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (road != null)      road.ForceUpdate();
        if (chara != null)     chara.ForceUpdate();
        if (particle != null)  particle.ForceUpdate();
        if (godray != null)    godray.ForceUpdate();
        if (cloud != null)     cloud.ForceUpdate();
        if (cameraCtl != null) cameraCtl.ForceUpdate();
    }
}
