using System;
using System.Collections;
using System.Collections.Generic;
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
        public List<Rigidbody> ConnectedRb = new List<Rigidbody>();

        public NodeComponent (AbstractGraphNode graphNode, GameObject visualComponent) : base(visualComponent)
		{
			this.graphNode = graphNode;
			InitializeNodeComponent ();
		}


		private void InitializeNodeComponent()
		{

            Renderer sprite = GetVisualComponent().GetComponent<Renderer>();
            //SpriteRenderer sprite = GetVisualComponent ().GetComponent<SpriteRenderer> ();
            sprite.name = "Node_" + graphNode.GetId ();

			TextMesh text = GetVisualComponent ().GetComponentInChildren<TextMesh>();
			text.text = "" + graphNode.GetTitle();

            if (graphNode.GetNType()=="M")
            {
                sprite.material.color = Color.yellow;
                GetVisualComponent().transform.GetChild(1).GetComponent<Renderer>().material.color = new Color(0.19f,0.8f, 0.19f);
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
            //aoundColliders = Physics.OverlapSphere(Rb.transform.position, GraphRenderer.Singleton.nodePhysXForceSphereRadius, mask);
        }

        public AbstractGraphNode GetGraphNode()
		{
			return graphNode;
		}    

    }
}

