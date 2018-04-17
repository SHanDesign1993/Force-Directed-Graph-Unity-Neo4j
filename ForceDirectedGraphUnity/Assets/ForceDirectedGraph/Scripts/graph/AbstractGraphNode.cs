using System;

namespace AssemblyCSharp
{
	public delegate void GraphEdgeVisitor(AbstractGraphEdge edge);

	public abstract class AbstractGraphNode : AbstractGraphElement
	{
		public abstract void Accept(GraphEdgeVisitor graphEdgeVisitor);

		public abstract long GetDegree();

		public abstract bool IsAdjacent(AbstractGraphNode graphNode);

        public abstract string GetContent();

        public abstract string GetTitle();

        public abstract string GetNType();

        public abstract int GetNid();
        
	}
}

