using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public class TransformAngleSensor : Sensor {

		protected Transform transform;

		public TransformAngleSensor (Transform transform) {
			this.transform = transform;
		}

		public override int OutputCount () {
			return 3;
		}

		public override float[] Output () {
			var angles = transform.localRotation.eulerAngles / 360f;
			return new float[] { angles.x, angles.y, angles.z };
		}

	}

}

