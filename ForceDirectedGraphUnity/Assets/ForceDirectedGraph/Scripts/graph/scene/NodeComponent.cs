using System;
using UnityEngine;
using NeoUnity;

namespace AssemblyCSharp
{
	public class NodeComponent : AbstractSceneComponent
	{
		private AbstractGraphNode graphNode;
        public Node Node;
        public Rigidbody Rb;
        public Collider[] aoundColliders = new Collider[20];

        public NodeComponent (AbstractGraphNode graphNode, GameObject visualComponent) : base(visualComponent)
		{
			this.graphNode = graphNode;
			InitializeNodeComponent ();
		}

		private void InitializeNodeComponent()
		{

			SpriteRenderer sprite = GetVisualComponent ().GetComponent<SpriteRenderer> ();
			sprite.name = "Node_" + graphNode.GetId ();

			TextMesh text = GetVisualComponent ().GetComponentInChildren<TextMesh>();
			text.text = "" + graphNode.GetTitle();

            if (graphNode.GetNType()=="M")
            {
                GetVisualComponent().GetComponentInChildren<SpriteRenderer>().material.color = Color.yellow;
            }
            

            Node = GetVisualComponent().GetComponentInChildren<Node>();
            Node.ID = graphNode.GetNid();
            Node.Title = "ID: "+ Node.ID+ " "+graphNode.GetTitle();
            Node.Content = graphNode.GetContent();
            Node.FVdata = new NeoUnity.ForceVectorData();
            Rb = Node.GetComponent<Rigidbody>();

            LayerMask mask = 1 << LayerMask.NameToLayer("Node");
            Physics.OverlapSphereNonAlloc(Rb.transform.position, GraphRenderer.Singleton.nodePhysXForceSphereRadius, aoundColliders, mask);
            // Physics.OverlapSphere calls gc
            //hitResult = Physics.OverlapSphere(Rb.transform.position, sphRadius, mask);
        }

        public AbstractGraphNode GetGraphNode()
		{
			return graphNode;
		}    

    }
}

