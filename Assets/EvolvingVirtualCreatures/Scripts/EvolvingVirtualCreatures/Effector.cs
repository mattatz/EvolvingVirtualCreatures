using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public interface Effector {
		int InputCount();
		void Affect(float[] inputs, float dt);
	}

}
