using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class GraphScene : GraphListener
	{
		private Graph graph;
		private GraphSceneComponents graphSceneComponents;
		private GraphScenePrefabs graphScenePrefabs;
		private AbstractGraphLayout graphLayout;

		public GraphScene (Graph graph, GraphScenePrefabs graphScenePrefabs)
		{
			this.graph = graph;
			this.graphScenePrefabs = graphScenePrefabs;
			this.graphSceneComponents = new GraphSceneComponents ();
			this.graphLayout = new SpringLayout (this.graphSceneComponents);

			this.graph.AddGraphListener (this);
		}

		public void DrawGraph()
		{
			graph.Accept (node => {
				DrawGraphNode(node);		
			});

			graphLayout.DoInitialLayout ();
		}

		public void GraphNodeCreated(AbstractGraphNode graphNode)
		{
			DrawGraphNode (graphNode);	
		}

		private void DrawGraphNode(AbstractGraphNode node)
		{
			if (graphSceneComponents.HasNodeComponent (node)) {
				return;
			}

			NodeComponent nodeComponent = new NodeComponent (node, graphScenePrefabs.InstantiateNode());
			graphSceneComponents.AddNodeComponent (nodeComponent);

			node.Accept (edge => {
				AbstractGraphNode endNode = edge.GetEndGraphNode();
				DrawGraphNode(endNode);
				DrawGraphEdge(edge);
			});
		}

		public void GraphEdgeCreated(AbstractGraphEdge graphEdge)
		{
			DrawGraphEdge (graphEdge);
		}
		
		private void DrawGraphEdge(AbstractGraphEdge edge)
		{
			if (graphSceneComponents.HasEdgeComponent (edge)) {
				return;
			}
			EdgeComponent edgeComponent = new EdgeComponent (edge, graphScenePrefabs.InstantiateEdge ());
			graphSceneComponents.AddEdgeComponent (edgeComponent);

			NodeComponent startNode = graphSceneComponents.GetNodeComponent (edgeComponent.GetGraphEdge().GetStartGraphNode().GetId());
			NodeComponent endNode = graphSceneComponents.GetNodeComponent (edgeComponent.GetGraphEdge().GetEndGraphNode().GetId());
			if (endNode == null) {
				return;
			}

			SpringJoint spring = endNode.GetVisualComponent ().GetComponent<SpringJoint> ();
			spring.connectedBody = startNode.GetVisualComponent ().GetComponent<Rigidbody> ();

			// checking if there is any other edge connecting the same nodes
			long sameNodeConnections = 0;
			graphSceneComponents.AcceptEdge (existingEdgeComponent => {
				AbstractGraphEdge existingEdge = existingEdgeComponent.GetGraphEdge();
				AbstractGraphNode existingStartNode = existingEdge.GetStartGraphNode();
				AbstractGraphNode existingEndNode = existingEdge.GetEndGraphNode();
				if (
					existingStartNode.GetId() == startNode.GetGraphNode().GetId() && existingEndNode.GetId() == endNode.GetGraphNode().GetId() || 
					existingStartNode.GetId() == endNode.GetGraphNode().GetId() && existingEndNode.GetId() == startNode.GetGraphNode().GetId()
				) {
					sameNodeConnections = sameNodeConnections + 1;

				}
			});
			if (sameNodeConnections > 1) {
				edgeComponent.MultiEdge = true;
			}
		}

		public NodeComponent RandomNode()
		{
			return graphSceneComponents.RandomNodeComponent ();
		}

		public void Update(float time)
		{
			graphSceneComponents.AcceptEdge (edgeComponent => {
				AbstractGraphEdge edge = edgeComponent.GetGraphEdge();
				AbstractGraphNode startNode = edge.GetStartGraphNode();
				AbstractGraphNode endNode = edge.GetEndGraphNode();

				NodeComponent startNodeComponent = graphSceneComponents.GetNodeComponent (startNode.GetId());
				NodeComponent endNodeComponent = graphSceneComponents.GetNodeComponent (endNode.GetId());

				// ---- distance function start
				float distance = 1F / startNode.GetDegree();
				distance *= 100;
				distance *= 3;

				SpringJoint springJoint = endNodeComponent.GetVisualComponent().GetComponent<SpringJoint>();
				springJoint.maxDistance = distance;
				springJoint.minDistance = distance;

				SphereCollider collider = endNodeComponent.GetVisualComponent().transform.GetChild(1).GetComponent<SphereCollider>();
				collider.radius = distance * 0.1F;


                endNodeComponent.GetVisualComponent().GetComponent<Rigidbody>().mass = 5;
				// ---- distance function end

				// magic
				Rigidbody body = endNodeComponent.GetVisualComponent().GetComponent<Rigidbody>();
				body.inertiaTensor = new Vector3(0.01F, 0.01F, 0.01F);

				edgeComponent.UpdateGeometry(
					startNodeComponent.GetVisualComponent().transform.position, 
					endNodeComponent.GetVisualComponent().transform.position);
			});




			// graphLayout.DoLayout (time);
		}
	}
}