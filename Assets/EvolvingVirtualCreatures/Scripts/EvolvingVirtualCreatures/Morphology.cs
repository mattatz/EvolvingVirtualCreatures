using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace mattatz.EvolvingVirtualCreatures {

	public class Node {

		public Vector3 Size { get { return size; } }

		Vector3 size;
		List<Connection> connections;

		public Node (Vector3 size) {
			this.size = size;
			this.connections = new List<Connection>();
		}

		public void Connect (Node to, SideType side) {
			connections.Add (new Connection(to, side));
		}

		public Connection[] GetConnections () {
			// var refToMe = connections.Find (co => co.To == this);
			return connections.ToArray();
		}

	}

	public class Connection {

		public Node To { get { return node; } }
		public SideType Side { get { return side; } }

		Node node;
		SideType side;

		public Connection (Node node, SideType side) {
			this.node = node;
			this.side = side;
		}

	}

	public class Morphology {
	}

}

