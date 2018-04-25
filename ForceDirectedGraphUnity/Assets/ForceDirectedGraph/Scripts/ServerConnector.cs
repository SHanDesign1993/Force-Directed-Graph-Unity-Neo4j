using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NeoUnity;

public class ServerConnector : MonoBehaviour {

    [Header("Data Setting")]
    public string Entity1_Str;
    public string Entity2_Str;
    public Text QueryText;

    [Header("Server Setting")]
    public int QueryLimit = 300;
    public string Username;
    public string Password;
    public string IP;
    public bool DebugLog = false;

    void Awake()
    {
        NeoUnity.Neo4j.Server.DebugLog = DebugLog;
        NeoUnity.Neo4j.Server.Username = Username;
        NeoUnity.Neo4j.Server.Password = Password;
        NeoUnity.Neo4j.Server.IP = IP;
        NeoUnity.Neo4j.Server.Limit = QueryLimit;
    }

    // Use this for initialization
    void Start () {
        GetAllDataFromServer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /* Get Data From Neo4j Server with Query String */
    public void GetDataFromServer()
    {
        if (QueryText.text == "*")
        {
            NeoUnity.Neo4j.RootObject o = NeoUnity.Neo4j.Server.QueryObject("MATCH (n:"+ Entity1_Str + ") -[r]- (b:" + Entity2_Str + ") return r,n,b");
            if (o.results.Count > 0) 
                GraphRenderer.Singleton.GetNeoData(o.results[0].data);  
        }
        else
        {
            string q = "MATCH (n:" + Entity1_Str + ") -[r] - (b:" + Entity2_Str + ") WHERE (n.title =~'.*" + QueryText.text + "') OR (n.title =~'" + QueryText.text + ".*') OR (b.name =~'.*" + QueryText.text + "') OR (b.name =~'" + QueryText.text + ".*') RETURN n, r, b";
            NeoUnity.Neo4j.RootObject o = NeoUnity.Neo4j.Server.QueryObject(q);
            if (o.results.Count > 0)
                GraphRenderer.Singleton.GetNeoData(o.results[0].data);
        }
    }

    /* Get Data From Neo4j Server */
    public void GetAllDataFromServer()
    {
        NeoUnity.Neo4j.RootObject o = NeoUnity.Neo4j.Server.QueryObject("MATCH (n:" + Entity1_Str + ") -[r]- (b:" + Entity2_Str + ") return r,n,b");
        if (o.results.Count > 0)
            GraphRenderer.Singleton.GetNeoData(o.results[0].data);
    }

}

