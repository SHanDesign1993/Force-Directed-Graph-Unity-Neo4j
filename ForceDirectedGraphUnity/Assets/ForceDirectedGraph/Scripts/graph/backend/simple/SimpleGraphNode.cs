using System;

namespace AssemblyCSharp
{
	public class SimpleGraphNode : AbstractGraphNode
	{

		private SimpleGraphBackend graphBackend;
		private long id;
        private int nid;
        private string ntype;
        private string ntitle;
        private string ncontent;
        public long EdgeCount = 0;

        public SimpleGraphNode (SimpleGraphBackend simpleGraphBackend, long id , int nid , string ntype ,string ntitle ,string ncontent)
		{
			this.graphBackend = simpleGraphBackend;
			this.id = id;

            this.nid = nid;
            this.ntype = ntype;
            this.ntitle = ntitle;
            this.ncontent = ncontent;
        }

		public override void Accept (GraphEdgeVisitor graphEdgeVisitor)
		{
			graphBackend.FindEdges (id).ForEach (edge => {
				graphEdgeVisitor(edge);
			});
		}

		public override long GetDegree ()
		{
			return graphBackend.FindEdges (id).Count;
		}

		public override bool IsAdjacent (AbstractGraphNode graphNode)
		{
			return graphBackend.FindEdges (id, graphNode.GetId ()).Count > 0;
		}

		public override long GetId ()
		{
			return id;
		}

        public override int GetNid()
        {
            return nid;
        }

        public override string GetTitle()
        {
            return ntitle;
        }

        public override string GetContent() {
            return ncontent;
        }

        public override string GetNType()
        {
            return ntype;
        }
    }
}

