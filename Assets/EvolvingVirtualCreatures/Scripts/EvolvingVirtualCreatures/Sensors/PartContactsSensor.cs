using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public class PartContactsSensor : Sensor {

		protected Part part;

		public PartContactsSensor (Part part) {
			this.part = part;
		}

		public override int OutputCount () {
			return 6;
		}

		public override float[] Output () {
			return this.part.Contacts01;
		}

	}

}

