using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour {

    public GameObject Subject;
    public GameObject Rocket;
    public ParticleSystem particles;
    public float BoosterValue;

	// Use this for initialization
    void Start()
    {
        particles.enableEmission = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Fire()
    {
    }
}
