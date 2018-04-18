using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class ForceDirectedGraphLayout {

	private bool running;
    private bool isStatic = true;
    private GraphSceneComponents sceneComponents;

    public ForceDirectedGraphLayout(GraphSceneComponents sceneComponents)
	{
		this.sceneComponents = sceneComponents;
	}

	public void DoInitialLayout()
	{
		sceneComponents.AcceptNode (node => {
			node.SetPosition(new Vector3 (Random.Range (-100, 100), Random.Range (-100, 100), Random.Range (-100, 100)));
		});

		UpdateEdges ();

        //hitResult = new Collider[sceneComponents.nodeComponents.Count/5];
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

    private void doAttraction(Rigidbody sourceRb, Rigidbody targetRb , float intendedLinkLengthSqr)
    {
        Vector3 forceDirection = sourceRb.transform.position - targetRb.transform.position;
        float distSqr = forceDirection.sqrMagnitude;

        if (distSqr > intendedLinkLengthSqr)
        {
            //Debug.Log("(Link.FixedUpdate) distSqr: " + distSqr + "/ intendedLinkLengthSqr: " + intendedLinkLengthSqr + " = distSqrNorm: " + distSqrNorm);
            float distSqrNorm = distSqr / intendedLinkLengthSqr;
            float forceStrength = GraphRenderer.Singleton.linkForceStrength;
            Vector3 targetRbImpulse = forceDirection.normalized * forceStrength * distSqrNorm;
            Vector3 sourceRbImpulse = forceDirection.normalized * -1 * forceStrength * distSqrNorm;

            //Debug.Log("(Link.FixedUpdate) targetRb: " + targetRb + ". forceDirection.normalized: " + forceDirection.normalized + ". distSqrNorm: " + distSqrNorm + ". Applying Impulse: " + targetRbImpulse);
            ((Rigidbody)targetRb as Rigidbody).AddForce(targetRbImpulse);
            //Debug.Log("(Link.FixedUpdate) targetRb: " + sourceRb + ". forceDirection.normalized: " + forceDirection.normalized + "  * -1 * distSqrNorm: " + distSqrNorm + ". Applying Impulse: " + sourceRbImpulse);
            ((Rigidbody)sourceRb as Rigidbody).AddForce(sourceRbImpulse);
        }
    }

    private void doGravity(Rigidbody Rb)
    {
        // Apply global gravity pulling node towards center of universe
        Vector3 dirToCenter = -Rb.transform.position;
        Vector3 impulse = dirToCenter.normalized * Rb.mass * GraphRenderer.Singleton.globalGravityPhysX;
        Rb.AddForce(impulse);

    }

    private void doRepulse(Rigidbody Rb,Collider[] hitResult, float sphRadiusSqr)
    {
        // test which node in within forceSphere.
        // only apply force to nodes within forceSphere, with Falloff towards the boundary of the Sphere and no force if outside Sphere.
        foreach (Collider hitCollider in hitResult)
        {
            if (hitCollider == null) break;
            Rigidbody hitRb = hitCollider.attachedRigidbody;

            if (hitRb != null && hitRb != Rb)
            {
                Vector3 direction = hitCollider.transform.position - Rb.transform.position;
                float distSqr = direction.sqrMagnitude;

                // Normalize the distance from forceSphere Center to node into 0..1
                float impulseExpoFalloffByDist = Mathf.Clamp(1 - (distSqr / sphRadiusSqr), 0, 1);

                // apply normalized distance
                hitRb.AddForce(direction.normalized * GraphRenderer.Singleton.repulseForceStrength * impulseExpoFalloffByDist);
            }
        }
    }

    public void DoLayout (float time)
	{
        if (isStatic) {
            GraphRenderer.Singleton.globalGravityPhysX = 0;
        }
		if (running) {
			return;
		}
        running = true;
		UpdateLayout (sceneComponents, time);
		running = false;
	}

    private void UpdateLayout(GraphSceneComponents sceneComponents, float time) {
       
        float sphRadius = GraphRenderer.Singleton.nodePhysXForceSphereRadius;
        float sphRadiusSqr = sphRadius * sphRadius;
        float intendedLinkLengthSqr = GraphRenderer.Singleton.linkIntendedLinkLength * GraphRenderer.Singleton.linkIntendedLinkLength;

        foreach (var n in sceneComponents.nodeComponents) {          
            doGravity(n.Rb);
            doRepulse(n.Rb, n.aoundColliders, sphRadiusSqr);
        }

        foreach (var e in sceneComponents.edgeComponents) {
            doAttraction(e.sourceRb, e.targetRb, intendedLinkLengthSqr);
        }
    }

}