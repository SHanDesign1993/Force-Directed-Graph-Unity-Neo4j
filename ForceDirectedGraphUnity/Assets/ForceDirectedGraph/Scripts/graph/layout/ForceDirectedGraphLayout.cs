using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class ForceDirectedGraphLayout {

	private bool running;
    private bool isStatic = true;
    private float halfWidth = 0;
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
            edgeComponent.sourceRb.mass++;
            edgeComponent.targetRb.mass++;

            
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
            n.Rb.mass--;
            if (n.Rb.mass > MostEdgesNodeCount) {
                MostEdgesNodeCount = n.Rb.mass;
            }
        }
     }

    private Vector3[] PointsOnSphere(int n,Vector3 center)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;
            var pt = new Vector3(x + center.x, y + center.y, z + center.z);
            upts.Add(pt);
            Debug.Log(pt);
        }
        Vector3[] pts = upts.ToArray();
        return pts;
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
            //doGravity(n.Rb);
            //doRepulse(n.Rb, n.aoundColliders, sphRadiusSqr);
        }

        foreach (var e in sceneComponents.edgeComponents) {
            //doAttraction(e.sourceRb, e.targetRb, intendedLinkLengthSqr);
        }


        /*
        Octree tree = new Octree(2.1f * halfWidth);
        foreach (var n in sceneComponents.nodeComponents)
        {
            tree.Add(n.Rb.gameObject);
        }
        foreach (var n in sceneComponents.nodeComponents)
        {
            // Apply repulsion between nodes. 
            tree.Accelerate(n.Rb.gameObject);
            
            // Apply origin attraction of nodes. 
            Vector3 originDisplacementUnit = -n.Rb.gameObject.transform.position;
            float originDistance = n.Rb.gameObject.transform.position.magnitude;

            float attractionCofficient = 2e4f;
            if (originDistance < GraphRenderer.Singleton.linkIntendedLinkLength)
                attractionCofficient *= originDistance / GraphRenderer.Singleton.linkIntendedLinkLength;

            n.Rb.AddForce(originDisplacementUnit * attractionCofficient / (originDistance + 7000));
            Debug.Log(n.Node.ID + ":" + originDisplacementUnit * attractionCofficient / (originDistance + 7000));
            // Apply edge spring forces. 
            foreach (var r in n.ConnectedRb)
            {
                // Gets a vector that points from the player's position to the target's.
                var heading = r.position - n.Rb.position;
                var distance = heading.magnitude;
                var direction = heading / distance; // This is now the normalized direction.

                float idealLength = GraphRenderer.Singleton.linkIntendedLinkLength + n.Rb.GetComponent<SphereCollider>().radius + r.GetComponent<SphereCollider>().radius;

                n.Rb.AddForce(direction * (distance - idealLength) / n.Rb.mass);
                
            }
        }
        */


    }

}