using System;

namespace AssemblyCSharp
{
	public class FruchtermanReingoldLayout : AbstractGraphLayout
	{
		private const float SPEED_DIVISOR = 1000;
		private const float AREA_MULTIPLICATOR = 10000;

		private float area = 3;
		private double gravity = 1;
		private float speed = 10;

		public FruchtermanReingoldLayout(GraphSceneComponents sceneComponents) : base(sceneComponents)
		{
		}

		protected override void UpdateGraphLayout (GraphSceneComponents sceneComponents, float time)
		{
			float maxDisplace = (float) (Math.Sqrt (AREA_MULTIPLICATOR * area) / 10F);
			float k = (float) Math.Sqrt (AREA_MULTIPLICATOR * area / (1F + sceneComponents.GetNodesCount()));

			sceneComponents.AcceptNode (n1 => {
				sceneComponents.AcceptNode (n2 => {
					if (n1.GetGraphNode().GetId() != n2.GetGraphNode().GetId()) {
						float xDist = n1.GetPosition().x - n2.GetPosition().x;
						float yDist = n1.GetPosition().y - n2.GetPosition().y;
						float zDist = n1.GetPosition().z - n2.GetPosition().z;
						float dist = (float) Math.Sqrt (xDist*xDist + yDist*yDist + zDist*zDist);

						if (dist > 0) {
							float repulsiveF = k*k / dist;

							ForceVectorNodeLayoutData layoutData = GetLayoutData (n1);
							layoutData.dx = xDist / dist * repulsiveF;
							layoutData.dy = yDist / dist * repulsiveF;
							layoutData.dz = zDist / dist * repulsiveF;
						}
					}
				});
			});

			sceneComponents.AcceptEdge (e => {
				NodeComponent nf = sceneComponents.GetNodeComponent(e.GetGraphEdge().GetStartGraphNode().GetId());
				NodeComponent nt = sceneComponents.GetNodeComponent(e.GetGraphEdge().GetEndGraphNode().GetId());	

				float xDist = nf.GetPosition().x - nt.GetPosition().x;
				float yDist = nf.GetPosition().y - nt.GetPosition().y;
				float zDist = nf.GetPosition().z - nt.GetPosition().z;
				float dist = (float) Math.Sqrt (xDist*xDist + yDist*yDist + zDist*zDist);

				float attractiveF = dist * dist / k;

				if (dist > 0) {
					ForceVectorNodeLayoutData sourceLayoutData = GetLayoutData (nf);
					ForceVectorNodeLayoutData targetLayoutData = GetLayoutData (nt);

					sourceLayoutData.dx -= xDist / dist * attractiveF;
                    sourceLayoutData.dy -= yDist / dist * attractiveF;
                    sourceLayoutData.dz -= zDist / dist * attractiveF;
                    
                    targetLayoutData.dx += xDist / dist * attractiveF;
					targetLayoutData.dy += yDist / dist * attractiveF;
					targetLayoutData.dz += zDist / dist * attractiveF;
				}
			});

			sceneComponents.AcceptNode (n => {
				ForceVectorNodeLayoutData layoutData = GetLayoutData (n);
				float d = (float) Math.Sqrt(n.GetPosition().x * n.GetPosition().x + n.GetPosition().y * n.GetPosition().y + n.GetPosition().z * n.GetPosition().z);
                float gf = 0.01F * k * (float) gravity * d;
				layoutData.dx -= gf * n.GetPosition().x / d;
				layoutData.dy -= gf * n.GetPosition().y / d;
				layoutData.dz -= gf * n.GetPosition().z / d;
			});

			// speed
			sceneComponents.AcceptNode (n => {
				ForceVectorNodeLayoutData layoutData = GetLayoutData (n);
				layoutData.dx *= speed / SPEED_DIVISOR;
				layoutData.dy *= speed / SPEED_DIVISOR;
				layoutData.dz *= speed / SPEED_DIVISOR;
			});

			sceneComponents.AcceptNode (n => {
				ForceVectorNodeLayoutData layoutData = GetLayoutData (n);
				float xDist = layoutData.dx;
				float yDist = layoutData.dy;
				float zDist = layoutData.dz;
				float dist = (float) Math.Sqrt(layoutData.dx * layoutData.dx + layoutData.dy * layoutData.dy + layoutData.dz * layoutData.dz);
				if (dist > 0) {
					float limitedDist = Math.Min(maxDisplace * ((float) speed / SPEED_DIVISOR), dist);

					n.SetPosition(new UnityEngine.Vector3(
						n.GetPosition().x + xDist / dist * limitedDist,
						n.GetPosition().y + yDist / dist * limitedDist,
						n.GetPosition().z + zDist / dist * limitedDist
					));
				}
			});

			UpdateEdges ();
		}

		private ForceVectorNodeLayoutData GetLayoutData(NodeComponent nodeComponent)
		{
			return nodeComponent.GetVisualComponent ().GetComponent<ForceVectorNodeLayoutData>();
		}
	}
}

