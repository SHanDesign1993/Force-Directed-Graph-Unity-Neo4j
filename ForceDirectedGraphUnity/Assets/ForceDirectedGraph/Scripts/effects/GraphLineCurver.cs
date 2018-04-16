using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class GraphLineCurver : MonoBehaviour
	{
		public float smoothness = 1.0f;

		void Start()
		{

			LineRenderer line = GetComponent<LineRenderer> ();
			if (line == null) {
				return;
			}
			Vector3 startPosition = line.GetPosition (0);
			Vector3 endPosition = line.GetPosition (1);

			float distance = Vector3.Distance (startPosition, endPosition);

			Vector3 middlePosition = Vector3.MoveTowards (startPosition, endPosition, distance / 4);
			//middlePosition.y = middlePosition.y + 20;

			Vector3[] arrayToCurve = new Vector3[3]{startPosition, middlePosition, endPosition};

			/*List<Vector3> points;
			List<Vector3> curvedPoints;
			int pointsLength = 0;
			int curvedLength = 0;

			if(smoothness < 1.0f) smoothness = 1.0f;

			pointsLength = arrayToCurve.Length;

			curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
			curvedPoints = new List<Vector3>(curvedLength);

			float t = 0.0f;
			for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
				t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);

				points = new List<Vector3>(arrayToCurve);

				for(int j = pointsLength-1; j > 0; j--){
					for (int i = 0; i < j; i++){
						points[i] = (1-t)*points[i] + t*points[i+1];
					}
				}

				curvedPoints.Add(points[0]);
			}

			line.numPositions = curvedPoints.Count;
			for (int index = 0; index < curvedPoints.Count; index++) {
				line.SetPosition (index, curvedPoints [index]);
			}*/

			for (int index = 0; index < arrayToCurve.Length; index++) {
				line.SetPosition (index, arrayToCurve [index]);
			}
		}
	}
}

