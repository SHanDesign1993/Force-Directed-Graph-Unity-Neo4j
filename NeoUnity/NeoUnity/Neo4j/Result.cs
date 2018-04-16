using System;
using System.Collections.Generic;

namespace NeoUnity.Neo4j
{
    [Serializable]
    public class Result
    {
        public List<string> columns;
        public List<GraphData> data;
    }
}
