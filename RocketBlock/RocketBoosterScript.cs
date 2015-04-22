using UnityEngine;
using System.Collections;

public class RocketBoosterScript : MonoBehaviour, InterpreterInterface, XBuildChild {
    public GameObject rocket;
    public ParticleSystem particles;
    public float BoosterValue;
    double firingTime = -1;

	// Use this for initialization
	void Start () {
        //particles.Pause();
        particles.enableEmission = false;
        //StartCoroutine(LaunchAfterDelay(0));
        //StartCoroutine(StopAfterDelay(2));
	}
	
	// Update is called once per frame
	void Update () {
           
	}

    public GameObject GetParent()
    {
        return rocket;
    }

    public void UpdateChild()
    {
        RootScript root = rocket.GetComponent<RootScript>();
        if (firingTime != -1)
        {
            particles.enableEmission = true;
            UpdateForce(root);
        }
        else
        {
            particles.enableEmission = false;
        }
    }

    public void UpdateForce(RootScript root)
    {
        //Find Angle
        BlockPhysics.AngleOrientation ao = BlockPhysics.CalculateAngle(root.gameObject, this.gameObject);

        //Apply Force
        Vector3 up = rocket.transform.up;
        up.z = 0;
        //up.Normalize();

        //Calculate the Booster Force into the pivot
        //This is F Sin theta
        Vector3 sint = up * BoosterValue;
        sint = Quaternion.AngleAxis((-1 * ao.sign * ao.angle), root.gameObject.transform.forward) * sint;
    
        //Debug.Log("Angle Orientation of " + this.name + " angle " + ao.angle + " sign " + ao.sign + " Result Vector: " + sint);

        root.AddForce(sint);
        //Apply Torque

        Vector3 forward = rocket.transform.forward;
        forward.x = 0;
        forward.y = 0;
        //forward.Normalize();
        float torque = BlockPhysics.CalculateTorque(BoosterValue, rocket, this.gameObject);
        root.AddTorque(forward * torque);
    }

    IEnumerator LaunchAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        EnableEngine();
    }

    IEnumerator StopAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        DisableEngine();
    }

    
    IEnumerator ChangeForceAfterDelay(float time, string force)
    {
        yield return new WaitForSeconds(time);
        BoosterValue = float.Parse(force);
        
        DisableEngine();
        EnableEngine();
    }

    public void EnableEngine()
    {
        if (firingTime != -1) return;
        Debug.Log("Engines enabled. Constant Force on " + rocket.name);

        firingTime = Time.realtimeSinceStartup;
    }


    public void DisableEngine()
    {
        if (firingTime == -1) return;
        firingTime = -1;
    }

    //Methods for xBuild Interpreter
    public void command(float time, string method, string[] args)
    {
        switch(method)
        {
            case "start":
                StartCoroutine(LaunchAfterDelay(time));
                break;
            case "stop":
                StartCoroutine(StopAfterDelay(time));
                break;
        }
    }

    public void set(float time, string vars, string value)
    {
        switch(vars)
        {
            case "force":
                StartCoroutine(ChangeForceAfterDelay(time, value));
                break;
        }
    }
}
