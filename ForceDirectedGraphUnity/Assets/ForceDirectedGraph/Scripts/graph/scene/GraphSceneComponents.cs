using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public delegate void GraphSceneEdgeVisitor(EdgeComponent edgeComponent);
	public delegate void GraphSceneNodeVisitor(NodeComponent edgeComponent);

	public class GraphSceneComponents
	{
		public List<NodeComponent> nodeComponents = new List<NodeComponent>();
        public List<EdgeComponent> edgeComponents = new List<EdgeComponent>();

		public GraphSceneComponents ()
		{
		}

		public void AddNodeComponent(NodeComponent nodeComponent)
		{
			nodeComponents.Add (nodeComponent);
		}

		public void AddEdgeComponent(EdgeComponent edgeComponent)
		{
			AbstractGraphEdge edge = edgeComponent.GetGraphEdge ();
			NodeComponent edgeStartNode = GetNodeComponent (edge.GetStartGraphNode().GetId());
			if (edgeStartNode == null) {
				Debug.Log ("Unable to find start node component.");
				return;
			}
			NodeComponent edgeEndNode = GetNodeComponent (edge.GetEndGraphNode().GetId());
			if (edgeEndNode == null) {
				Debug.Log ("Unable to find end node component.");
				return;
			}
			edgeComponents.Add(edgeComponent);
			edgeComponent.UpdateGeometry (edgeStartNode.GetVisualComponent().transform.position, edgeEndNode.GetVisualComponent().transform.position);
		}

		public bool HasNodeComponent(AbstractGraphNode graphNode)
		{
			return nodeComponents.Exists (nodeComponent => {
				return nodeComponent.GetGraphNode().GetId() == graphNode.GetId();
			});
		}

		public bool HasEdgeComponent(AbstractGraphEdge graphEdge)
		{
			return edgeComponents.Exists (edgeComponent => {
				return edgeComponent.GetGraphEdge().GetId() == graphEdge.GetId();
			});
		}

		public NodeComponent GetNodeComponent(long nodeId)
		{
			return nodeComponents.Find (node => {
				return node.GetGraphNode().GetId() == nodeId;
			});
		}

		public void AcceptEdge(GraphSceneEdgeVisitor edgeVisitor)
		{
			edgeComponents.ForEach (edge => {
				edgeVisitor(edge);
			});
		}

		public void AcceptNode(GraphSceneNodeVisitor nodeVisitor)
		{
			nodeComponents.ForEach (node => {
				nodeVisitor(node);
			});
		}

		public NodeComponent RandomNodeComponent ()
		{
			return nodeComponents [Random.Range(0, nodeComponents.Count)];
		}

		public long GetNodesCount()
		{
			return nodeComponents.Count;
		}
	}
}

