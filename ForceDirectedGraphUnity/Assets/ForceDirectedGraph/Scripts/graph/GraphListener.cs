using System;

namespace AssemblyCSharp
{
	public interface GraphListener
	{
		void GraphNodeCreated(AbstractGraphNode graphNode);

		void GraphEdgeCreated(AbstractGraphEdge graphEdge);
	}
}

