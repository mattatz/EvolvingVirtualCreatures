using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public class DirectionSensor : Sensor {

		protected Transform transform;
		protected Vector3 target;

		public DirectionSensor (Transform tr, Vector3 p) {
			transform = tr;
			target = p;
		}

		public override int OutputCount () {
			return 3;
		}

		public override float[] Output () {
			var dir = (target - transform.position).normalized; 
			return new float[] { dir.x, dir.y, dir.z };
		}

	}

}
