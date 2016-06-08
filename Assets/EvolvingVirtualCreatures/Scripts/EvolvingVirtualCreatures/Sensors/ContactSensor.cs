using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public class ContactSensor : Sensor {

		protected Part part;

		public ContactSensor (Part part) {
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

