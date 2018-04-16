using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoUnity.Neo4j
{
    [Serializable]
    public class Graph
    {
        public List<Node> nodes;
        public List<Relationship> relationships;
    }
}
