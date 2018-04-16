using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public abstract class AbstractSceneComponent
	{
		private GameObject visualComponent;

		public AbstractSceneComponent (GameObject visualComponent)
		{
			this.visualComponent = visualComponent;
		}

		public GameObject GetVisualComponent()
		{
			return visualComponent;
		}

		public void SetPosition(Vector3 position)
		{
			visualComponent.transform.position = position;
		}

		public Vector3 GetPosition ()
		{
			return visualComponent.transform.position;
		}
	}
}

