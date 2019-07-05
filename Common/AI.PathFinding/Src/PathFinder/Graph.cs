using System.Collections.Generic;

namespace Lockstep.AI.PathFinding {
	public interface Graph<N> {

		/**和当前节点相连的连接关系
		 * Returns the connections outgoing from the given node.
		 * 
		 * @param fromNode
		 *            the node whose outgoing connections will be returned
		 * @return the array of connections outgoing from the given node.
		 */
		List<Connection<N>> GetConnections(N fromNode);
	}
}