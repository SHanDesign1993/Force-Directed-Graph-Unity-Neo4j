using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public class FruchtermanReingoldLayout 
	{
		
		private float speed = 10;
        private GraphSceneComponents sceneComponents;

        private float iterator = 0;
        private float MaxIterations = 1000;  
 

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
            if (iterator >= MaxIterations)
            {
                if (GraphRenderer.Singleton.SelectedObject)
                    iterator = 0;
                
            } else {
                iterator += Time.deltaTime * 200f;
                ApplyForce();
            }

		}

        private void ApplyForce() {

            foreach (var n1 in sceneComponents.nodeComponents)
            {
                foreach (var n2 in sceneComponents.nodeComponents)
                {
                    if (n1.Node.ID != n2.Node.ID)
                    {
                        float xDist = n1.Node.transform.position.x - n2.Node.transform.position.x;
                        float yDist = n1.Node.transform.position.y - n2.Node.transform.position.y;
                        float zDist = n1.Node.transform.position.z - n2.Node.transform.position.z;
                        float dist = (float)Mathf.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist);

                        if (dist > 0)
                        {
                            float repulsiveF = GraphRenderer.Singleton.k * GraphRenderer.Singleton.k / dist;
                            n1.Rb.AddForce(new Vector3(xDist / dist * repulsiveF, yDist / dist * repulsiveF, zDist / dist * repulsiveF) * speed);
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

                float attractiveF = dist * dist / GraphRenderer.Singleton.k;

                if (dist > 0)
                {
                    nf.AddForce(new Vector3(-xDist / dist * attractiveF, -yDist / dist * attractiveF, -zDist / dist * attractiveF) * speed);
                    nt.AddForce(new Vector3(xDist / dist * attractiveF, yDist / dist * attractiveF, zDist / dist * attractiveF) * speed);
                }
            }

        }
	}
}

