using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class GraphListeners
	{
		private List<GraphListener> listeners = new List<GraphListener>();

		public void AddGraphListener(GraphListener listener)
		{
			listeners.Add (listener);
		}

		public void NotifyNodeCreated(AbstractGraphNode graphNode)
		{
			listeners.ForEach (listener => {
				listener.GraphNodeCreated(graphNode);
			});
		}
			
		public void NotifyEdgeCreated(AbstractGraphEdge graphEdge)
		{
			listeners.ForEach (listener => {
				listener.GraphEdgeCreated(graphEdge);
			});
		}
	}
}

