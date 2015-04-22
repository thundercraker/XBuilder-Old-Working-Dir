using UnityEngine;
using System.Collections;

public class RocketRunSCript : MonoBehaviour, InterpreterInterface
{
    public GameObject rocket;
    public ParticleSystem particles;
    public float BoosterValue;
    double firingTime = -1;

    // Use this for initialization
    void Start()
    {
        //particles.Pause();
        particles.enableEmission = false;
        StartCoroutine(LaunchAfterDelay(1));
        //StartCoroutine(StopAfterDelay(2));
    }

    // Update is called once per frame
    void Update()
    {
        if (firingTime != -1)
        {
            particles.enableEmission = true;
        }
        else
        {
            particles.enableEmission = false;
        }
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
        Debug.Log("Engines enabled");
        rocket.constantForce.relativeForce = Vector3.up * BoosterValue;
        firingTime = Time.realtimeSinceStartup;
    }


    public void DisableEngine()
    {
        if (firingTime == -1) return;
        rocket.constantForce.relativeForce = Vector3.zero;
        firingTime = -1;
    }

    //Methods for xBuild Interpreter
    public void command(float time, string method, string[] args)
    {
        switch (method)
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
        switch (vars)
        {
            case "force":
                StartCoroutine(ChangeForceAfterDelay(time, value));
                break;
        }
    }
}
