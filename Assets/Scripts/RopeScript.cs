using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{

    public Vector2 destiny;
    public float speed = 0.1f;
    public float distance = 0.1f;
    public GameObject nodePrefab;
    public GameObject player;
    public GameObject lastNode;
    bool done = false;

    public LineRenderer lr;

    int vertexCount = 2;
    public List<GameObject> Nodes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lastNode = transform.gameObject;
        Nodes.Add(transform.gameObject);
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position,destiny,speed);

        if ((Vector2)transform.position != destiny)
        {
            if (Vector2.Distance(player.transform.position,lastNode.transform.position) > distance)
            {
                CreateNode();
            }
        }
        else if(!done)
        {
            done = true;
            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        }

        RenderLine();
    }

    void RenderLine()
    {
        lr.positionCount = vertexCount;
        int i;
        for (i = 0; i < Nodes.Count; i++)
        {
            lr.SetPosition(i, Nodes[i].transform.position);
        }
        lr.SetPosition(i, player.transform.position);
    }

    void CreateNode()
    {
        Vector2 pos2Create = player.transform.position - lastNode.transform.position;               //Get Vector to next Rope section
        pos2Create.Normalize();                                                                     //" -> Look direction
        pos2Create *= distance;                                                                     //Calculate Distance until next spawn
        pos2Create += (Vector2)lastNode.transform.position;                                         //Get next Postition

        GameObject go = (GameObject)Instantiate(nodePrefab,pos2Create,Quaternion.identity);         //Spawn next RopeSection

        go.transform.SetParent(transform);                                                          //add Section to last Parent

        lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();       //connect Hinge to last Rigidbody

        lastNode = go;                                                                              //get ready for next Node
        Nodes.Add(lastNode);                                                                        //Add Count for Line Renderer
        vertexCount++;
    }
}