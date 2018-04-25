using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public class FruchtermanReingoldLayout 
	{
		private float area = 8000;
		private float speed = 3;
        private GraphSceneComponents sceneComponents;

        public FruchtermanReingoldLayout(GraphSceneComponents sceneComponents)
        {
            this.sceneComponents = sceneComponents;
        }

        public void DoInitialLayout()
        {
            sceneComponents.AcceptNode(node => {
                node.SetPosition(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)));
            });

            UpdateEdges();
        }

        protected void UpdateEdges()
        {
            sceneComponents.AcceptEdge(edgeComponent => {

                AbstractGraphEdge edge = edgeComponent.GetGraphEdge();
                AbstractGraphNode startNode = edge.GetStartGraphNode();
                AbstractGraphNode endNode = edge.GetEndGraphNode();

                sceneComponents.GetNodeComponent(startNode.GetId()).ConnectedRb.Add(sceneComponents.GetNodeComponent(endNode.GetId()).Rb);
                sceneComponents.GetNodeComponent(endNode.GetId()).ConnectedRb.Add(sceneComponents.GetNodeComponent(startNode.GetId()).Rb);

                edgeComponent.UpdateGeometry(
                    sceneComponents.GetNodeComponent(startNode.GetId()).GetVisualComponent().transform.position,
                    sceneComponents.GetNodeComponent(endNode.GetId()).GetVisualComponent().transform.position);
            });

            float MostEdgesNodeCount = 0;
            foreach (var n in sceneComponents.nodeComponents)
            {
                if (n.ConnectedRb.Count > MostEdgesNodeCount)
                {
                    MostEdgesNodeCount = n.ConnectedRb.Count;
                }
            }
            foreach (var n in sceneComponents.nodeComponents)
            {
                if (n.ConnectedRb.Count == MostEdgesNodeCount)
                {
                    n.SetPosition(Vector3.zero);
                }
            }

        }

        public void DoLayout(float time)
		{
            float maxDisplace = (float)(Mathf.Sqrt(area) / 3F);
            float k = (float)Mathf.Sqrt(area / (1 + sceneComponents.nodeComponents.Count));

            foreach (var n1 in sceneComponents.nodeComponents)
            {
                foreach (var n2 in sceneComponents.nodeComponents)
                {
                    if (n1.GetGraphNode().GetId() != n2.GetGraphNode().GetId())
                    {
                        float xDist = n1.GetPosition().x - n2.GetPosition().x;
                        float yDist = n1.GetPosition().y - n2.GetPosition().y;
                        float zDist = n1.GetPosition().z - n2.GetPosition().z;
                        float dist = (float)Mathf.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist);

                        if (dist > 0)
                        {
                            float repulsiveF = k * k / dist;
                            n1.Rb.AddForce(new Vector3(xDist / dist * repulsiveF, yDist / dist * repulsiveF, zDist / dist * repulsiveF)*speed);
                        }
                    }
                }
            }

            foreach (var e in sceneComponents.edgeComponents)
            {
                Rigidbody nf = e.sourceRb;
                Rigidbody nt = e.targetRb;

                float xDist = nf.transform.position.x - nt.transform.position.x;
                float yDist = nf.transform.position.y - nt.transform.position.y;
                float zDist = nf.transform.position.z - nt.transform.position.z;
                float dist = (float)Mathf.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist);

                float attractiveF = dist * dist / k;

                if (dist > 0)
                {

                    nf.AddForce(new Vector3(-xDist / dist * attractiveF, -yDist / dist * attractiveF, -zDist / dist * attractiveF) *speed);
                    nt.AddForce(new Vector3(xDist / dist * attractiveF, yDist / dist * attractiveF, zDist / dist * attractiveF) * speed);
                }
            }
		}
	}
}

