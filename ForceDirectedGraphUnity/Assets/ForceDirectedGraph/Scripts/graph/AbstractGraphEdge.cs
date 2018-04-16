using System;

namespace AssemblyCSharp
{
	public abstract class AbstractGraphEdge : AbstractGraphElement
	{
		public abstract AbstractGraphNode GetStartGraphNode();

		public abstract AbstractGraphNode GetEndGraphNode();

        public abstract string GetRType();

    }
}

