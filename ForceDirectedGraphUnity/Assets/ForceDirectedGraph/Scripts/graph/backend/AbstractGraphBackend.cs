using System;
using NeoUnity;
namespace AssemblyCSharp
{
	public abstract class AbstractGraphBackend
	{
		private GraphBackendListeners listeners = new GraphBackendListeners();
	
		public abstract AbstractGraphNode NewNode(int nid,string ntype, string ntitle,string ncontent);

		public abstract AbstractGraphEdge NewEdge(AbstractGraphNode from, AbstractGraphNode to,string rtype);

		public abstract AbstractGraphNode GetNodeById(long nodeId);

		public abstract long CountAllEdges();

		public abstract long CountAllNodes();

		public void AddListener(GraphBackendListener listener)
		{
			listeners.AddGraphBackendListener (listener);
		}

		public void NotifyBackendNodeCreated(AbstractGraphNode graphNode)
		{
			listeners.NotifyGraphBackendCreated (graphNode);
		}

		public void NotifyBackendEdgeCreated(AbstractGraphEdge graphEdge)
		{
			listeners.NotifyGraphBackendCreated (graphEdge);
		}
	}
}

