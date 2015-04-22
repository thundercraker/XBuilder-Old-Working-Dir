using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RadarBlockScript : MonoBehaviour
{
    public float radius;
    public string[] objects;
    
    public void Ping()
    {
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius);
        objects = (from col in colliders
                   where IsValid(col.gameObject)  
                   orderby Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position)
                   select col.gameObject.name).ToArray();

        //objects = objects.Where(item => IsValid(item)).ToArray();
        Debug.Log("Radar Ping [" + gameObject.name + "] Range " + radius + " Colliders " + colliders.Length + " Closest " + objects[0]);
        //objects = tobj.ToArray();
    }

    public bool IsValid(GameObject item)
    {
        return !(item.name.Contains("XBuild") 
            || item.tag.Equals(BlockObjectScript.XBUILD_BLOCK_TAG, System.StringComparison.OrdinalIgnoreCase));
    }
}
