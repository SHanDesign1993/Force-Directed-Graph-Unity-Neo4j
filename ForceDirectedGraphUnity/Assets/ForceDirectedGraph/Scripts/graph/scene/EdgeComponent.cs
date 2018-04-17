using System;
using UnityEngine;
using NeoUnity;

namespace AssemblyCSharp
{
	public class EdgeComponent : AbstractSceneComponent
	{
		public bool MultiEdge = false;
		private AbstractGraphEdge graphEdge;
        public Relationship relationship;

        public Rigidbody sourceRb;
        public Rigidbody targetRb;

        public EdgeComponent (AbstractGraphEdge graphEdge, GameObject visualComponent) : base(visualComponent)
		{
			this.graphEdge = graphEdge;
			InitializeEdgeComponent ();
		}

		private void InitializeEdgeComponent()
		{
			LineRenderer line = GetVisualComponent ().GetComponent<LineRenderer> ();
			line.name = "Edge_" + graphEdge.GetId ();

			float angle = UnityEngine.Random.Range (0, 360);
		
			float xRotation = Mathf.Cos (Mathf.Deg2Rad * angle) * 100;
			float yRotation = Mathf.Sin (Mathf.Deg2Rad * angle) * 100;
			float zRotation = Mathf.Cos (Mathf.Deg2Rad * angle) * 100;
			GetVisualComponent().transform.Rotate(new Vector3 (xRotation, yRotation, zRotation));

            relationship = GetVisualComponent().GetComponent<Relationship>();
            relationship.LR = line;
            relationship.Node1 = GameObject.Find("Node_" + graphEdge.GetStartGraphNode().GetId().ToString()).GetComponent<Node>();
            relationship.Node2 = GameObject.Find("Node_" + graphEdge.GetEndGraphNode().GetId().ToString()).GetComponent<Node>();
            relationship.RelationshipType = graphEdge.GetRType();
            sourceRb = relationship.Node1.GetComponent<Rigidbody>();
            targetRb = relationship.Node2.GetComponent<Rigidbody>();
        }

        public AbstractGraphEdge GetGraphEdge()
		{
			return graphEdge;
		}

		public void UpdateGeometry(Vector3 start, Vector3 end)
		{
			LineRenderer line = GetVisualComponent ().GetComponent<LineRenderer> ();
			line.SetPosition (0, start);
			line.SetPosition (1, end);
		}

	}
}

