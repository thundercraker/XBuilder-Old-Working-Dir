using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RootScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ForceList = new List<Vector3>();
        TorqueList = new List<Vector3>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateChildren();

        //now begin updating self
        if (ForceList.Count > 0)
        {
            gameObject.constantForce.relativeForce = CompoundForce();// *Time.deltaTime;
            //Debug.Log("Compound Force " + CompoundForce());

        }
        else
        {
            //Debug.Log("Clamping Force");
            CompositeForceDamp();
            CompositeVelocityDamp();
        }
        if (TorqueList.Count > 0)
        {
            gameObject.constantForce.relativeTorque = CompoundTorque();// *Time.deltaTime;
            //Debug.Log("Compound Torque " + CompoundTorque());
        }
        else
        {
            //Debug.Log("Clamping Torque");
            CompositeTorqueDamp();
            CompositeAngularVelocityDamp();
        }

        ForceList.Clear();
        TorqueList.Clear();
	}

    void Update()
    {
        Vector3 pos = gameObject.transform.position;
        pos.z = 0;
        gameObject.transform.position = pos;

        //Look at updates
        if (LookAtTransform!=null)
        {
            var rotation = Quaternion.LookRotation(LookAtTransform.position - gameObject.transform.position);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rotation, Time.deltaTime * DampeningTorque);
        }
    }

    /* Manage the manual execution of updates on the children
     * this will allow the root object to have its update at the end
     * when children have passed along various updates
     */
    List<XBuildChild> updateList;
    List<Vector3> ForceList;
    List<Vector3> TorqueList;
    public float DampeningForce = 20f;
    public float DampeningTorque = 20f;
    public float DampeningAccel = 3f;
    public Transform LookAtTransform = null;

    public void AddChildToUpdate(XBuildChild child)
    {
        if(updateList == null)
            updateList = new List<XBuildChild>();
        updateList.Add(child);
    }

    public void UpdateChildren()
    {
        foreach(XBuildChild child in updateList)
        {
            child.UpdateChild();
        }
    }

    //Allow children to add a force or torque vector to be queued
    //and calculated for this frame
    public void AddForce(Vector3 force)
    {
        ForceList.Add(force);
    }

    public void AddTorque(Vector3 torque)
    {
        TorqueList.Add(torque);
    }

    public Vector3 CompoundForce()
    {
        Vector3 compound = new Vector3(0, 0, 0);
        foreach(Vector3 force in ForceList)
        {
            compound = compound + force;
        }
        compound.z = 0;
        return compound;
    }

    public Vector3 CompoundTorque()
    {
        Vector3 compound = new Vector3(0, 0, 0);
        foreach(Vector3 torque in TorqueList)
        {
            compound = compound + torque;
        }
        compound.x = 0;
        compound.y = 0;
        return compound;
    }

    public void CompositeForceDamp()
    {
        Vector3 cforce = gameObject.constantForce.relativeForce;
        float dampSignX = Mathf.Sign(cforce.x);
        float dampSignY = Mathf.Sign(cforce.y);
        float dampx = (Mathf.Abs(cforce.x) > Mathf.Abs(DampeningForce * Time.deltaTime)) ? cforce.x - (dampSignX * DampeningForce * Time.deltaTime) : 0;
        float dampy = (Mathf.Abs(cforce.y) > Mathf.Abs(DampeningForce * Time.deltaTime)) ? cforce.y - (dampSignY * DampeningForce * Time.deltaTime) : 0;
        gameObject.constantForce.relativeForce = new Vector3(dampx, dampy, 0);
    }

    public void CompositeTorqueDamp()
    {
        Vector3 ctorque = gameObject.constantForce.relativeTorque;
        float dampSignZ = Mathf.Sign(ctorque.z);
        float dampz = (Mathf.Abs(ctorque.z) > Mathf.Abs(DampeningForce * Time.deltaTime)) ? ctorque.z - (dampSignZ * DampeningForce * Time.deltaTime) : 0;
        gameObject.constantForce.relativeTorque = new Vector3(0, 0, dampz);

    }

    public void CompositeVelocityDamp()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        Vector3 velocity = rb.velocity;
        float dampSignX = Mathf.Sign(velocity.x);
        float dampSignY = Mathf.Sign(velocity.y);
        float dampx = (Mathf.Abs(velocity.x) > Mathf.Abs(DampeningAccel * Time.deltaTime)) ? velocity.x - (dampSignX * DampeningAccel * Time.deltaTime) : 0;
        float dampy = (Mathf.Abs(velocity.y) > Mathf.Abs(DampeningAccel * Time.deltaTime)) ? velocity.y - (dampSignY * DampeningAccel * Time.deltaTime) : 0;

        //Debug.Log("Velocity Damping " + dampx + " " + dampy);
        rb.velocity = new Vector3(dampx, dampy, 0);
    }

    public void CompositeAngularVelocityDamp()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        Vector3 velocity = rb.angularVelocity;
        float dampSignZ = Mathf.Sign(velocity.z);
        float dampz = (Mathf.Abs(velocity.z) > Mathf.Abs(DampeningAccel * Time.deltaTime)) ? velocity.z - (dampSignZ * DampeningAccel * Time.deltaTime) : 0;

        //Debug.Log("Angular Velocity Damping " + dampz);
        rb.angularVelocity = new Vector3(0, 0, dampz);
    }
}
