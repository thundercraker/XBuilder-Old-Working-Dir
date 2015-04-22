using UnityEngine;
using System.Collections.Generic;

public class BlockObjectScript : MonoBehaviour
{
    public static string XBUILD_BLOCK_TAG = "xbuildblock";
    public static string XBUILD_ROOT_TAG = "xbuildroot";

    enum BlockType
    {
        RootTest = 0,
        NormalRed = 1,
        NormalBlue = 2,
        RocketBooster = 3,
        MissileT1 = 4,
        Radar = 5
    }

    public GameObject interpreterObject;
    public GameObject mainCamera;
    public float X, Y, Z;
    public GameObject emptyPrefab;
    public GameObject prefabNormalRed;
    public GameObject prefabNormalBlue;
    public GameObject prefabRocketBooster;
    public GameObject prefabMissileType1;
    public GameObject prefabRadar;

    //Dictionary<Vector2, string> names;
    string[,] names;

    // Use this for initialization
    void Start()
    {
        //Make the success Screen invivisble
        //successOverlay.active = false;

        //CreateBlock((int)BlockType.NormalRed, 2, 2, 0);

        //Test Data
        //int[,] data = new int[,] { { 2, 1, 1, 1, 2}, { 1, 0, 2, 0, 1}, { 1, 2, 2, 2, 1 },
        //{ 1, 0, 2, 0, 1}, { 2, 2, 3, 2, 2}};
        int[,] data = new int[,] { 
            { 3, 1, 0, 0, 0, 0, 0},
            { 0, 2, 1, 0, 0, 0, 0}, 
            { 1, 1, 1, 4, 0, 0, 0}, 
            { 2, 2, 2, 2, 5, 0, 0},
            { 1, 1, 1, 4, 0, 0, 0}, 
            { 0, 2, 1, 0, 0, 0, 0},
            { 3, 1, 0, 0, 0, 0, 0}
        };
        //names = new Dictionary<Vector2, string>();
        //names.Add(new Vector2(2, 0), "rocket1");
        names = new string[7, 7];
        names[0, 0] = "rocket1";
        names[6, 0] = "rocket2";
        names[2, 3] = "missiletype11";
        names[4, 3] = "missiletype12";
        names[3, 4] = "radar1";

        Vector2 center = new Vector2(3, 2);
        string XBuildName = "XShip";

        Vector3 location = new Vector3(X, Y, Z);
        GameObject root = CreateXBuild(XBuildName, location, center, data);
        Rigidbody RBroot = root.GetComponent<Rigidbody>();
        RBroot.mass = root.transform.childCount;
        //root XBuild must be attaced to camera
        
        CameraScript cs = root.AddComponent<CameraScript>();
        cs.mainCamera = mainCamera;

        //start the interpreter script
        Interpreter iscript = interpreterObject.GetComponent<Interpreter>();
        iscript.Root = root.GetComponent<RootScript>();

        iscript.Run();
    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject CreateBlock(int type, string name, Vector2 location, GameObject root)
    //create a block at certain coordinates
    //z coordinate is normally locked as the
    //custom block building is restricted to a plane
    {
        GameObject newBlock;
        switch (type)
        {
            case ((int)BlockType.NormalRed):
                newBlock = Instantiate(prefabNormalRed) as GameObject;
                break;
            case ((int)BlockType.NormalBlue):
                newBlock = Instantiate(prefabNormalBlue) as GameObject;
                break;
            case ((int)BlockType.RocketBooster):
                newBlock = Instantiate(prefabRocketBooster) as GameObject;
                if (name != null)
                {
                    newBlock.name = name;
                    Debug.Log("[CreateBlock Debug] Named RocketBooster block (" + name + ") @ " + location);
                }
                else
                {
                    Debug.Log("[CreateBlock Error] Named RocketBooster block does not have a name @ " + location);
                }
                if (root != null)
                {
                    RocketBoosterScript rbs = newBlock.GetComponent<RocketBoosterScript>();
                    rbs.rocket = root;

                    //rocket blocks are XBuildChild implementers and need to be
                    //added to the root blocks updateList
                    RootScript rs = root.GetComponent<RootScript>();
                    
                    rs.AddChildToUpdate(rbs);
                }
                break;
            case ((int)BlockType.MissileT1):
                newBlock = Instantiate(prefabMissileType1) as GameObject;
                if (name != null)
                {
                    newBlock.name = name;
                    Debug.Log("[CreateBlock Debug] Named MissileTurretType1 block (" + name + ") @ " + location);
                }
                else
                {
                    Debug.Log("[CreateBlock Error] Named MissileTurretType1 block does not have a name @ " + location);
                }
                break;
            case ((int)BlockType.Radar):
                newBlock = Instantiate(prefabRadar) as GameObject;
                if (name != null)
                {
                    newBlock.name = name;
                    Debug.Log("[CreateBlock Debug] Named Radar block (" + name + ") @ " + location);
                }
                else
                {
                    Debug.Log("[CreateBlock Error] Named Radar block does not have a name @ " + location);
                }
                break;
            
            default:
                newBlock = null;
                break;
        }
        foreach (Transform child in newBlock.transform)
        {
            child.gameObject.name = newBlock.name + " :: " + child.gameObject.name;
        }
        //give the newblock a tag
        newBlock.tag = XBUILD_BLOCK_TAG;
        return newBlock;
    }

    /* Data format for any xBuild is 
     * a) the block closest to the center
     * b) a matrix of integers, eg:
     * [xBuild matrixes are always ODD*ODD]
     * 1,1
     * 0,1,0
     * 1,1,1
     * 0,1,0
     * Will give a plus sign of Normal Red Blocks
     */
    class Node
    {
        public GameObject gameObject;
        public Vector2 relative;
        public Vector2 absolute;

        public Node(GameObject gameObject, Vector2 relative, Vector2 absolute)
        {
            this.gameObject = gameObject;
            this.relative = relative;
            this.absolute = absolute;
        }

        public Node EmptyChild(Vector2 add)
        {
            Node child = new Node(null, this.relative, this.absolute);
            child.relative.x += add.x;
            child.relative.y += add.y;
            child.absolute.x += add.x;
            child.absolute.y += add.y;

            return child;
        }
    }

    //Define transforms of Up, Down, Left and Right
    //We will need this to scan in these directions
    Vector2[] scan = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), 
        new Vector2(0, -1), new Vector2(0, 1) };

    GameObject CreateXBuild(string XBuildName, Vector3 location, Vector2 center, int[,] data)
    //create a composite xBuilder
    //start at the first detected node than scan in
    //all 4 directions
    {
        int bounds = data.GetLength(0);
        //parent object to put evertyhing under
        //GameObject rootObject = Instantiate(emptyPrefab, location, Quaternion.identity) as GameObject;
        //rootObject.name = "XBuild Root";

        //the first node
        Vector2 firstLocation = new Vector2((int)center.x, (int)center.y);
        string firstName = names[(int)center.x, (int)center.y];
        //names.TryGetValue(location, out firstName);
        int type = data[(int)center.x, (int)center.y];
        GameObject firstObject = CreateBlock(type, firstName, firstLocation, null);
        firstObject.AddComponent<RootScript>();
        //make this a rigidbody
        //DestroyImmediate(firstObject.GetComponent("BoxCollider"));
        firstObject.AddComponent<Rigidbody>();
        Rigidbody rb = firstObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        firstObject.AddComponent<ConstantForce>();

        //firstObject.transform.parent = rootObject.transform;
        firstObject.transform.localPosition = location;//new Vector3(0,0,0);
        firstObject.name = XBuildName;
        firstObject.tag = XBUILD_ROOT_TAG;
        data[(int)center.x, (int)center.y] = 0;


        Node first = new Node(firstObject, new Vector2(0, 0), center);
        Queue<Node> nodes = new Queue<Node>();
        nodes.Enqueue(first);

        int count = 0;

        while (nodes.Count != 0 && count < 100)
        {
            Node parent = nodes.Dequeue();
            //Debug.Log("Parent Relative Location " + parent.relative + " Absolute " + parent.absolute);
            //4 directional scan
            //up
            for (int i = 0; i < scan.Length; i++)
            {
                Node child = parent.EmptyChild(scan[i]);
                if (checkBounds(child, bounds))
                {
                    //Debug.Log("Bounds " + bounds + "Child Absolute " + child.absolute);
                    int childType = data[(int)child.absolute.x, (int)child.absolute.y];
                    if (childType != 0)
                    {
                        //check if block has a name
                        Vector2 childLocation = new Vector2((int)child.absolute.x, (int)child.absolute.y);
                        string childName = names[(int)child.absolute.x, (int)child.absolute.y];

                        GameObject childObject = CreateBlock(childType, childName, childLocation, firstObject);

                        childObject.transform.parent = firstObject.transform;
                        childObject.transform.localPosition = child.relative;
                        if (childName == null)
                            childObject.name = firstObject.name + "_" + count + "_" + i;

                        child.gameObject = childObject;
                        nodes.Enqueue(child);
                        //Debug.Log("Created child @ relative " + child.relative);
                        //clear from the matrix
                        data[(int)child.absolute.x, (int)child.absolute.y] = 0;
                    }
                    else
                    {
                        //Debug.Log("Empty Tile @ absolute " + child.absolute);
                    }
                }
            }
            count++;
        }

        return firstObject;
    }

    private bool checkBounds(Node node, int bound)
    {
        return (node.absolute.x < 0 || node.absolute.x >= bound
            || node.absolute.y < 0 || node.absolute.y >= bound) ? false : true;
    }
}