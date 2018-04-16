using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Graph : GraphBackendListener
	{
		public delegate void NodeVisitor (AbstractGraphNode node);

		private AbstractGraphBackend graphBackend;
		private List<AbstractGraphNode> loadedNodes = new List<AbstractGraphNode> ();
		private GraphListeners listeners = new GraphListeners();

		public Graph (AbstractGraphBackend graphBackend)
		{
			this.graphBackend = graphBackend;
			this.graphBackend.AddListener (this);
		}


		public AbstractGraphNode NewNode(int nid,string ntype,string ntitle,string ncontent) 
		{
			return graphBackend.NewNode (nid,ntype,ntitle,ncontent);
		}

		public AbstractGraphEdge NewEdge(AbstractGraphNode from, AbstractGraphNode to ,string rtype)
		{
			return graphBackend.NewEdge (from, to, rtype);
		}

		public void AddGraphListener(GraphListener graphListener)
		{
			listeners.AddGraphListener(graphListener);
		}

		public void GraphBackendNodeCreated(AbstractGraphNode graphNode)
		{
			loadedNodes.Add (graphNode);
			listeners.NotifyNodeCreated (graphNode);
		}

		public void GraphBackendEdgeCreated(AbstractGraphEdge graphEdge)
		{
			listeners.NotifyEdgeCreated (graphEdge);
		}

		public void Accept(NodeVisitor visitor)
		{
			loadedNodes.ForEach (loadedNode => {
				visitor(loadedNode);
			});
		}
	}
}

