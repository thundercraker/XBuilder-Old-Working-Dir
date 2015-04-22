using UnityEngine;
using System.Collections;
//using System.Web.Script.Serialization;

public class XBuildData {

    struct XBuildStruct
    {
        public Vector2 center;
        public int[,] data;
        public string[,] name;
    }

    XBuildStruct XBS;
    /*
	public XBuildData(string rawJson)
    {
        dynamic result = JsonValue.Parse(rawJson);
        XBS.center = result.response.center;
        XBS.data = result.response.data;
        XBS.name = result.response.name;
    }

    public string GetJSON()
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();
        return jss.Serialize(this);
    }
     * */
}
