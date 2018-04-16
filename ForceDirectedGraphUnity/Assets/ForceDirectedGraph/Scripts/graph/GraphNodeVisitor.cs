using System;

namespace AssemblyCSharp
{
	public interface GraphNodeVisitor
	{
		void VisitNode(AbstractGraphNode graphNode);
	}
}

