using System;

namespace NeoUnity.Neo4j
{
    [Serializable]
    public class Relationship
    {
        public string id;
        public string type;
        public string startNode;
        public string endNode;
        public RelationshipProperties properties;
    }
}
