using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public class AnchorAngleSensor : Sensor {

		protected Segment segment;

		public AnchorAngleSensor (Segment segment) {
			this.segment = segment;
		}

		public override int OutputCount () {
			return 3;
		}

		public override float[] Output () {
			Vector3 angles;
			if(segment.Root) {
				angles = segment.transform.localRotation.eulerAngles / 360f;
			} else {
				angles = segment.Anchor.localRotation.eulerAngles / 360f;
			}
			return new float[] { angles.x, angles.y, angles.z };
		}

	}

}

