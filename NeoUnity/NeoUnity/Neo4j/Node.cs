using System;
using System.Collections.Generic;

namespace NeoUnity.Neo4j
{
    [Serializable]
    public class Node 
    {
        public string id;
        public List<string> labels;
        public NodeProperties properties;
    }
    

}
