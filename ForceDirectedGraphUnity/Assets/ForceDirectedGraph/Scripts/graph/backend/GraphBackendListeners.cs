using System;
using System.Collections.Generic;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public class GraphBackendListeners
	{

		private List<GraphBackendListener> listeners = new List<GraphBackendListener>(); 

		public void AddGraphBackendListener(GraphBackendListener listener) 
		{
			listeners.Add (listener);
		}

		public void NotifyGraphBackendCreated(AbstractGraphNode graphNode)
		{
			listeners.ForEach (listener => {
				listener.GraphBackendNodeCreated(graphNode);
			});
		}

		public void NotifyGraphBackendCreated(AbstractGraphEdge graphEdge)
		{
			listeners.ForEach (listener => {
				listener.GraphBackendEdgeCreated(graphEdge);
			});
		}
	}
}

