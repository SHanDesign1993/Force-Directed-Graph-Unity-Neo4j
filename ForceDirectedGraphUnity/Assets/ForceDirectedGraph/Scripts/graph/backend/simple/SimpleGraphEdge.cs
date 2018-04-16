using System;

namespace AssemblyCSharp
{
	public class SimpleGraphEdge : AbstractGraphEdge
	{

		private long id;
		private AbstractGraphNode startNode;
		private AbstractGraphNode endNode;
        private string rtype;

		public SimpleGraphEdge (long id, AbstractGraphNode startNode, AbstractGraphNode endNode, string rtype)
		{
			this.id = id;
			this.startNode = startNode;
			this.endNode = endNode;
            this.rtype = rtype;
		}

		public override AbstractGraphNode GetStartGraphNode ()
		{
			return startNode;
		}

		public override AbstractGraphNode GetEndGraphNode ()
		{
			return endNode;
		}

		public override long GetId ()
		{
			return id;
		}

        public override string GetRType() {
            return rtype;
        }
	}
}

