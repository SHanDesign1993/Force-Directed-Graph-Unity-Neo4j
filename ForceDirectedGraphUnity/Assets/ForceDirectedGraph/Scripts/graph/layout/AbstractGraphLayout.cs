using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public abstract class AbstractGraphLayout {

	private bool running;
	private GraphSceneComponents sceneComponents;

	public AbstractGraphLayout(GraphSceneComponents sceneComponents)
	{
		this.sceneComponents = sceneComponents;
	}

	public void DoInitialLayout()
	{
		sceneComponents.AcceptNode (node => {
			node.SetPosition(new Vector3 (Random.Range (-150, 150), Random.Range (-150, 150), Random.Range (-150, 150)));
		});

		UpdateEdges ();
	}

	protected void UpdateEdges()
	{
		sceneComponents.AcceptEdge (edgeComponent => {
			AbstractGraphEdge edge = edgeComponent.GetGraphEdge();
			AbstractGraphNode startNode = edge.GetStartGraphNode();
			AbstractGraphNode endNode = edge.GetEndGraphNode();
			edgeComponent.UpdateGeometry(
				sceneComponents.GetNodeComponent(startNode.GetId()).GetVisualComponent().transform.position, 
				sceneComponents.GetNodeComponent(endNode.GetId()).GetVisualComponent().transform.position);
		});
	}

	public void DoLayout (float time)
	{
		if (running) {
			return;
		}
		running = true;
		UpdateGraphLayout (sceneComponents, time);
		running = false;
	}

	protected abstract void UpdateGraphLayout (GraphSceneComponents sceneComponents, float time);
}