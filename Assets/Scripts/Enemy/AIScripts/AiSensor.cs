using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

//[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
public class AiSensor : MonoBehaviour
{
    public float distance = 10;
    public float angle = 30;
    public float height = 1f;
    public int scanFrequency = 30;

    public Color meshColor = Color.red;
    //CHECK LAYERS TO COLLIDE
    public LayerMask layers; // for scanning
    //CHECK LAYER TO LINECAST IF A WALL IS NOT OBSTRUCTING
    public LayerMask occlusionLayers;


    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;
    public SphereCollider sphereCollider;
    //To get the hostile layer
    AiAgent agent;
    string HOSTILE_LAYER;
    // Start is called before the first frame update
    void Start()
    {
        ResetPotentialTargets();
        agent = GetComponent<AiAgent>();
        HOSTILE_LAYER = agent.AiConfig.HOSTILE_LAYER_NAME;
        scanInterval = 1f / scanFrequency;
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = distance;
    }

    // Update is called once per frame
    void Update()
    {
        //scanTimer -= Time.deltaTime;
        //if(scanTimer < 0)
        //{
        //    scanTimer += scanInterval;
        //    Scan();
        //}
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position,distance,colliders,layers,QueryTriggerInteraction.Collide);

        for(int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
            }
        }
    }

    public struct GameObjects_PreventGC
    {
        public GameObject obj;
        public bool consider;
    }

    [SerializeField]
    public GameObjects_PreventGC[] Objects_Array = new GameObjects_PreventGC[15];

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter called. " + other.gameObject);
        for(int i = 0;i < 15; i++)
        {
            if (Objects_Array[i].obj == null || Objects_Array[i].consider == false)
            {
                //Debug.Log("ObjectAdded.  " + other.name);
                Objects_Array[i].obj = other.gameObject as GameObject;
                Objects_Array[i].consider = true;
                break;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit called." + other.gameObject);
        for (int i = 0; i < 15; i++)
        {
            if (Objects_Array[i].obj != null && Objects_Array[i].obj == other.gameObject)
            {
                //Debug.Log("Remove target out of sight : " + other.name);
                Objects_Array[i].consider = false;
            }
        }
    }

    public bool IsInSight(GameObject obj) // need a revamp
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        //if(direction.y < 0 )//|| direction.y >height)
        //{
        //    return false;
        //}

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if(deltaAngle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;

        if (Physics.Linecast(origin, dest, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    public int Filter(GameObject[] buffer) // filters all the objects to match the certain layer and populate the buffer array 
    {
        RemoveDisabled();
        int count = 0;
        for(int i = 0;i < 15; i++)
        {
            if (Objects_Array[i].consider == true)
            {
                buffer[count++] = Objects_Array[i].obj;
                if(buffer.Length == count)
                {
                    break;
                }
            }
        }
        return count;
    }

    public void RemoveDisabled()
    {
        for(int i = 0; i < 15; i++)
        {
            if (Objects_Array[i].consider == true)
            {
                if (Objects_Array[i].obj != null)
                {
                    if (!Objects_Array[i].obj.gameObject.activeInHierarchy)
                    {
                        //Debug.Log("Remove Disabled called");
                        Objects_Array[i].consider = false;
                    }
                    //else if ((Objects_Array[i].obj.transform.position - transform.position).magnitude > distance) // CHANGESS
                    //{
                    //    Debug.Log("Remove Disabled called");
                    //    Objects_Array[i].consider = false;
                    //    Objects.Remove(Objects_Array[i].obj);
                    //}
                    //else
                    //{
                    //    //Debug.Log("Object Still active not removed");
                    //}
                }
                else
                {
                    //Debug.Log("Object was null, cannot be removed");
                }
            }
        }
    }

    public void RemovePlayerDisabled(GameObject PlayerHITBOX)
    {
        for(int i = 0; i < 15; i++)
        {
            if (Objects_Array[i].obj != null && Objects_Array[i].obj == PlayerHITBOX.gameObject)
            {
                if (Objects_Array[i].consider == true)
                {
                    Objects_Array[i].consider = false;
                    break;
                }
            }
        }
    }
    public void ResetPotentialTargets()
    {
        for(int i = 0; i < 15; i++)
        {
            Objects_Array[i].obj = null;
            Objects_Array[i].consider = false;
        }
    }

    public void OnDisable()
    {
        //Debug.Log("AiSensor OnDisabled called");
        ResetPotentialTargets();
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 10;
        int numTriangles = (segments*4) + 2 + 2;
        int numVertices = numTriangles*3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        //left side

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for(int i = 0; i < segments; i++)
        { 
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;
            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        

        for(int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh,transform.position,transform.rotation);
        }

        //Gizmos.DrawWireSphere(transform.position, distance);
        for(int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(colliders[i].transform.position,0.2f);
        }

        Gizmos.color = Color.green;
        //foreach(var obj in Objects)
        //{
        //    Gizmos.DrawSphere(obj.transform.position, 0.2f);
        //}
    }

    
}
