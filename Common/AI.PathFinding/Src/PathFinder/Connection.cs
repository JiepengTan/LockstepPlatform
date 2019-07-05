using Lockstep.Math;

namespace Lockstep.AI.PathFinding {
	public interface Connection<N> {

		/** Returns the non-negative cost of this connection */
		LFloat GetCost();

		/** Returns the node that this connection came from */
		N GetFromNode();

		/** Returns the node that this connection leads to */
		N GetToNode();
	}
}