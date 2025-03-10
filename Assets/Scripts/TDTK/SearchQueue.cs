namespace TDTK
{
	internal class SearchQueue
	{
		public NodeTD startNode;

		public NodeTD endNode;

		public NodeTD[] graph;

		public SetPathCallbackTD callBackFunc;

		public SearchQueue(NodeTD n1, NodeTD n2, NodeTD[] g, SetPathCallbackTD func)
		{
			startNode = n1;
			endNode = n2;
			graph = g;
			callBackFunc = func;
		}
	}
}
