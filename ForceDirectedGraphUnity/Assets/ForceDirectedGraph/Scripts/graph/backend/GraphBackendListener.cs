using System;

namespace AssemblyCSharp
{
	public interface GraphBackendListener
	{
		void GraphBackendNodeCreated(AbstractGraphNode graphNode);

		void GraphBackendEdgeCreated(AbstractGraphEdge graphEdge);
	}
}

