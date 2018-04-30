using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoUnity;
using AssemblyCSharp;
using UnityEngine.UI;

public class GraphRenderer : MonoBehaviour {
    public static GraphRenderer Singleton;

    [Header("UI Status")]
    public Text StatusText;
    public Text ResultText;
    public GameObject ResultPanel;

    [Header("UI Selected")]
    public GameObject SelectedObject;
    public Text SelectedText;
    public Text SelectedTitle;
    public Text SelectedContent;
    private bool Selected = false;

    [Header("Graph Object")]
    public GraphScenePrefabs graphScenePrefabs;
    public GameObject RelationTypePrefab;
    private List<Relationship> rtypeList =new List<Relationship>();

    [Header("Graph Setting")]
    private bool RepulseActive = true;
    [SerializeField]
    public float repulseForceStrength = 0.1f;

    [SerializeField]
    public float linkForceStrength = 6F;

    [SerializeField]
    public float linkIntendedLinkLength = 5F;

    [SerializeField]
    public float globalGravityPhysX = 10f;
    
    [SerializeField]
    public float nodePhysXForceSphereRadius = 50F;

    private float area = 8000;
    public float maxDisplace = 0;
    public float k = 0;

    public Color LineOriginColor;
    public Color LineSelectedColor;

    [Header("Camera")]
    public Camera cam;
    public float Zoom = 50;
    public float MinZoom = 1;
    public float MaxZoom = 100;
    public float ZoomSpeed = 20f;
    public float PanMult = 2f;

    private Graph graph;
    private GraphScene graphScene;
    private List<NeoUnity.Neo4j.GraphData> data;

    [Header("Data")]
    public Dictionary<int, AbstractGraphNode> nodes = new Dictionary<int, AbstractGraphNode>();
    public List<AbstractGraphEdge> rels = new List<AbstractGraphEdge>();
    public Dictionary<string, string> relationship = new Dictionary<string, string>();

    void Awake() {
        Singleton = this;
        graph = new Graph(InitializeGraphBackend());
        graphScene = new GraphScene(graph, graphScenePrefabs);
        ResultPanel.SetActive(false);
        ResultText.gameObject.SetActive(false);
    }

    private void ClearGraph()
    {
        while (GameObject.FindWithTag("Node") != null) DestroyImmediate(GameObject.FindWithTag("Node"));
        while (GameObject.FindWithTag("Edge") != null) DestroyImmediate(GameObject.FindWithTag("Edge"));
        nodes.Clear();
        rels.Clear();
        graph = new Graph(InitializeGraphBackend());
        graphScene = new GraphScene(graph, graphScenePrefabs);
    }

    public void GetNeoData(List<NeoUnity.Neo4j.GraphData> data)
    {
        ClearGraph();
        foreach (NeoUnity.Neo4j.GraphData graphdata in data)
        {
            //loop though all neo4j nodes and make our nodes from it
            foreach (var node in graphdata.graph.nodes)
            {

                if (nodes.ContainsKey(int.Parse(node.id)))
                {
                    //Debug.LogError("Duplicate Node " + int.Parse(node.id) + ", Droping Node");
                }
                else
                {
                    int nid = int.Parse(node.id);
                    string ntype="", ntitle="",ncontent = "";
                    string Role1 = this.GetComponent<ServerConnector>().Entity1_Str;
                    string Role2 = this.GetComponent<ServerConnector>().Entity2_Str;
                    /* check label . judge data entities (movie or perosn)*/
                    if (node.labels.Contains(Role1)){
                        ntype = "M";
                        ntitle = Role1 + "\n" + node.properties.title + "(" + node.properties.released.ToString() + ")";
                        ncontent = node.properties.tagline;
                    }
                    else {
                        ntype = "P";
                        ntitle = Role2 + "\n" + node.properties.name + "(" + node.properties.born.ToString() + ")";
                    }

                    AbstractGraphNode newNode = graph.NewNode(nid, ntype, ntitle, ncontent);
                    nodes.Add(nid, newNode);
                }

            }
            foreach (var rel in graphdata.graph.relationships)
            {
                if (relationship.ContainsValue(rel.startNode + "" + rel.endNode))
                {
                    //Debug.LogError("Duplicate Edge " + int.Parse(rel.id) + ", Droping Edge");
                }
                else {
                    //Debug.Log(rel.startNode + "-" + rel.endNode);
                    AbstractGraphEdge newEdge = graph.NewEdge(nodes[int.Parse(rel.startNode)], nodes[int.Parse(rel.endNode)], rel.type);
                    rels.Add(newEdge);
                    relationship.Add(rel.id,rel.startNode+""+ rel.endNode);
                }
            }

        }

        maxDisplace = (float)(Mathf.Sqrt(area) / 3F);
        k = (float)Mathf.Sqrt(area / (1 + nodes.Count));

        graphScene.DrawGraph();
        UpdateStatus();
    }

    private AbstractGraphBackend InitializeGraphBackend()
	{
		return new SimpleGraphBackend ();
	}

    Ray GenerateMouseRay() {
        Vector3 mousePosFar = new Vector3(Input.mousePosition.x,
                                            Input.mousePosition.y, cam.farClipPlane);
        Vector3 mousePosNear = new Vector3(Input.mousePosition.x,
                                            Input.mousePosition.y, cam.nearClipPlane);
        Vector3 mousePosF = cam.ScreenToWorldPoint(mousePosFar);
        Vector3 mousePosN = cam.ScreenToWorldPoint(mousePosNear);
        Ray mr = new Ray(mousePosN,mousePosF-mousePosN);
        return mr;
    }

	void Update ()
	{
        graphScene.Update (1);
 
        // Mouse Input Action.
        if (Input.GetMouseButton(0))
        {
            Ray mouseRay = GenerateMouseRay();
            RaycastHit hit;

            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit))
            {
                if (hit.collider.tag == "Node" && SelectedObject == null)
                {
                    SelectedObject = hit.collider.gameObject;
                    ResetLineColor();
                    Selected = false;
                }
            }
            UpdateTypePos();
        }

        if (SelectedObject)
        {
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            SelectedObject.transform.position = new Vector3(newPos.x, newPos.y, SelectedObject.transform.position.z);

            NeoUnity.Node sn = SelectedObject.GetComponent<NeoUnity.Node>();

            SelectedText.text =  "OBJ: "+sn.gameObject.name;
            SelectedTitle.text = sn.Title;
            SelectedContent.text = sn.Content;

            if (!Selected)
            {
                ChangeLineColor(SelectedObject);
                Selected = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            SelectedText.text = "DRAG A NODE";
            SelectedTitle.text = "";
            SelectedContent.text = "";
            ResetLineColor();
            Selected = false;
            SelectedObject = null;
        }

        if (Input.GetMouseButton(1))
        {
            float x = -Input.GetAxis("Mouse X");
            float y = -Input.GetAxis("Mouse Y");
            cam.transform.position += new Vector3(x, y) * (Zoom / MaxZoom) * PanMult;
        }

        Zoom = Mathf.Clamp(Zoom - (Input.GetAxis("Mouse ScrollWheel") * (Zoom / MaxZoom) * ZoomSpeed), MinZoom, MaxZoom);
        cam.orthographicSize = Zoom;
    }

    /* query result status*/
    void UpdateStatus()
    {
        if (nodes.Count == 0) {
            StatusText.text = "NO DATA FOUND.";
            ResultText.text = StatusText.text;
        }else{
            StatusText.text = nodes.Count.ToString() + " NODES \n" + rels.Count.ToString() + " EDGES";
            ResultText.text = StatusText.text + " \n FOUND.";
        }
        StartCoroutine(ShowResult());
    }

    /*  query result alert*/
    IEnumerator ShowResult() {
        ResultPanel.SetActive(true);
        ResultText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ResultPanel.SetActive(false);
        ResultText.gameObject.SetActive(false);
    }

    /* Change Relationship LineRenderer Color to Yellow. */
    public void ChangeLineColor(GameObject selected)
    {
        var edgeList = GameObject.FindGameObjectsWithTag("Edge");
        foreach (var e in edgeList)
        {
            GameObject relationship = e;
            GameObject startNode = e.GetComponent<Relationship>().Node1.gameObject;
            GameObject endNode = e.GetComponent<Relationship>().Node2.gameObject;

            if (startNode == selected || endNode == selected)
            {
                rtypeList.Add(e.GetComponent<Relationship>());

                var LR = relationship.GetComponent<LineRenderer>();
                var gradient = new Gradient();
                gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(LineSelectedColor, 0.0f), new GradientColorKey(LineSelectedColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });

                LR.colorGradient = gradient;
            }
        }

        SpawnRelationType();
    }

    /* Reset Relationship LineRenderer Color. */
    public void ResetLineColor()
    {
        ClearRelationType();
        var edgeList = GameObject.FindGameObjectsWithTag("Edge");
        foreach (var e in edgeList)
        {
            GameObject relationship = e;

            var LR = relationship.GetComponent<LineRenderer>();
            var gradient = new Gradient();
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(LineOriginColor, 0.0f), new GradientColorKey(LineOriginColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(0.4f, 1.0f) });

            LR.colorGradient = gradient;
        }
    }

    /* Spawn Relation Type Object */
    public void SpawnRelationType()
    {
        if (rtypeList.Count == 0) return;
        foreach (var rt in rtypeList)
        {
            var rType = Instantiate(RelationTypePrefab);
            rType.GetComponentInChildren<TextMesh>().text = rt.RelationshipType;
            rt.RelationshipTypeObj = rType;
        }
    }

    /* Clean Relation Type Object */
    public void ClearRelationType()
    {
        while (GameObject.FindWithTag("RType") != null) DestroyImmediate(GameObject.FindWithTag("RType"));
        rtypeList.Clear();
    }

    /* Update Relation Type Object Position */
    void UpdateTypePos()
    {
        if (rtypeList.Count == 0) return;

        foreach (var rt in rtypeList)
        {
            rt.RelationshipTypeObj.transform.position = (rt.Node1.transform.position + rt.Node2.transform.position) / 2;
        }
    }

}
