using System;
using System.Collections.Generic;

namespace NeoUnity.Neo4j
{
    [Serializable]
    public class RootObject
    {
        public List<Result> results;
        public List<object> errors;
    }
}
