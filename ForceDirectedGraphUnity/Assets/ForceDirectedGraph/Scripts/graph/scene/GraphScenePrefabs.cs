using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class GraphScenePrefabs : MonoBehaviour
	{
		public GameObject NodePrefab;
		public GameObject EdgePrefab;

		public GameObject InstantiateNode()
		{
			GameObject node = Instantiate (NodePrefab);
			node.transform.SetParent (transform);
			return node;
		}

		public GameObject InstantiateEdge()
		{
			GameObject edge = Instantiate (EdgePrefab);
			edge.transform.SetParent (transform);
			return edge;
		}
	}
}

