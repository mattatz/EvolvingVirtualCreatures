using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using mattatz.EvolvingVirtualCreatures;

public class MorphologyTest : MonoBehaviour {

	[SerializeField] GameObject prefab;

	void Start () {
		var body = new Node(Vector3.one);

		var leftArm = new Node(new Vector3(0.8f, 0.2f, 0.2f));
		var leftArm2 = new Node(new Vector3(0.6f, 0.1f, 0.1f));
		var leftArm3 = new Node(new Vector3(0.4f, 0.1f, 0.1f));

		body.Connect(leftArm, SideType.Left);
		leftArm.Connect(leftArm2, SideType.Left);
		leftArm2.Connect(leftArm3, SideType.Left);

		// body.Connect(arm, SideType.Right);
		// arm.Connect(arm2, SideType.Right);

		var segment = Build (body);
		segment.transform.position = transform.position;
	}
	
	void Update () {
	}

	public Segment Build (Node root) {
		var parent = Instantiate(prefab).GetComponent<Segment>();
		parent.SetRoot(true);
		parent.transform.localScale = root.Size;
		var cons = root.GetConnections();
		for(int i = 0, n = cons.Length; i < n; i++) {
			var con = cons[i];
			Build (parent, con.Side, con.To);
		}
		return parent;
	}

	void Build (Segment parentSegment, SideType side, Node childNode) {
		var cur = Instantiate(prefab).GetComponent<Segment>();
		cur.transform.localScale = childNode.Size;
		cur.Connect(parentSegment, side);
		var cons = childNode.GetConnections();
		for(int i = 0, n = cons.Length; i < n; i++) {
			var con = cons[i];
			Build (cur, con.Side, con.To);
		}
	}

}
