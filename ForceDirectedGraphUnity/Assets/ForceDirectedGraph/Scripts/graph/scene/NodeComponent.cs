using System;
using UnityEngine;
using NeoUnity;

namespace AssemblyCSharp
{
	public class NodeComponent : AbstractSceneComponent
	{
		private AbstractGraphNode graphNode;

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

            if (graphNode.GetType()=="M")
            {
                GetVisualComponent().GetComponentInChildren<SpriteRenderer>().material.color = Color.yellow;
            }

            Node n = GetVisualComponent().GetComponentInChildren<Node>();
            n.ID = graphNode.GetNid();
            n.Title = "ID: "+n.ID+ " "+graphNode.GetTitle();
            n.Content = graphNode.GetContent();
        }

		public AbstractGraphNode GetGraphNode()
		{
			return graphNode;
		}

	}
}

