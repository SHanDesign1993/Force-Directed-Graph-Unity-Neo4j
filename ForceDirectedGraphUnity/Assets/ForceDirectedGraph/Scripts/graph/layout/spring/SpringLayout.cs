using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public class SpringLayout : AbstractGraphLayout
	{
		private float stretch = 0.7F;
		private int repulsion_range_sq = 100 * 100;
		private float force_multiplier = 1/3;

		public SpringLayout (GraphSceneComponents graphSceneComponents) : base(graphSceneComponents)
		{
		}

		override protected void UpdateGraphLayout (GraphSceneComponents sceneComponents, float time)
		{
			sceneComponents.AcceptNode (nodeComponent => {
				SpringLayoutData layoutData = GetLayoutData (nodeComponent);
				layoutData.xDelta /= 4;
				layoutData.yDelta /= 4;
				layoutData.zDelta /= 4;

				layoutData.x = 0;
				layoutData.y = 0;
				layoutData.z = 0;

				layoutData.xRepulsion = 0;
				layoutData.yRepulsion = 0;
				layoutData.zRepulsion = 0;
			});

			RelaxEdges (sceneComponents);
			CalculateRepulsion (sceneComponents);
			MoveNodes (sceneComponents);

			UpdateEdges ();
		}

		private void RelaxEdges(GraphSceneComponents sceneComponents)
		{
			sceneComponents.AcceptEdge (edgeComponent => {
				NodeComponent v1 = sceneComponents.GetNodeComponent (edgeComponent.GetGraphEdge().GetStartGraphNode().GetId());
				NodeComponent v2 = sceneComponents.GetNodeComponent (edgeComponent.GetGraphEdge().GetEndGraphNode().GetId());

				float vx = v1.GetPosition().x - v2.GetPosition().x;
				float vy = v1.GetPosition().y - v2.GetPosition().y;
				float vz = v1.GetPosition().z - v2.GetPosition().z;

				float desiredLen = 5;
				float len = (float) Mathf.Sqrt (vx*vx + vy*vy + vz*vz);
				if (len == 0) {
					len = 1F;
				}	

				double f = force_multiplier * (desiredLen - len) / len;
				f = f * Mathf.Pow (stretch, 2);

				double dx = f * vx;
				double dy = f * vy;
				double dz = f * vz;

				SpringLayoutData v1d = GetLayoutData (v1);
				SpringLayoutData v2d = GetLayoutData (v2);

				v1d.x += dx;
				v1d.y += dy;
				v1d.z += dz;

				v2d.x -= dx;
				v2d.y -= dy;
				v2d.z -= dz;
			});
		}

		private void CalculateRepulsion(GraphSceneComponents sceneComponents)
		{
			sceneComponents.AcceptNode (v => {
				SpringLayoutData layoutData = GetLayoutData (v);

				double dx = 0;
				double dy = 0;
				double dz = 0;

				sceneComponents.AcceptNode (v2 => {
					float vx = v.GetPosition().x - v2.GetPosition().x;
					float vy = v.GetPosition().y - v2.GetPosition().y;
					float vz = v.GetPosition().z - v2.GetPosition().z;

					double distanceSq = Mathf.Sqrt(Vector3.Distance(v.GetPosition(), v2.GetPosition()));
					if (distanceSq == 0) {
						System.Random random = new System.Random();
						dx += random.NextDouble();
						dy += random.NextDouble();
						dz += random.NextDouble();
					} else if (distanceSq < repulsion_range_sq) {
						float factor = 0.1F;
						dx += factor * vx / distanceSq;
						dy += factor * vy / distanceSq;
						dz += factor * vz / distanceSq;
					}
				});

				double dlen = dx*dx + dy*dy + dz*dz;
				if (dlen > 0) {
					dlen = System.Math.Sqrt(dlen) / 20;
					layoutData.xRepulsion += dx / dlen;
					layoutData.yRepulsion += dy / dlen;
					layoutData.zRepulsion += dz / dlen;
				}
			});
		}

		private void MoveNodes(GraphSceneComponents sceneComponents)
		{
			sceneComponents.AcceptNode (v => {
				SpringLayoutData vd = GetLayoutData (v);
				vd.xDelta += vd.xRepulsion + vd.x;
				vd.yDelta += vd.yRepulsion + vd.y;
				vd.zDelta += vd.zRepulsion + vd.z;

				v.SetPosition (new Vector3 ((float)vd.xDelta, (float)vd.yDelta, (float)vd.zDelta));
			});
		}

		private SpringLayoutData GetLayoutData(NodeComponent nodeComponent)
		{
			return nodeComponent.GetVisualComponent ().GetComponent<SpringLayoutData>();
		}
	}
}

