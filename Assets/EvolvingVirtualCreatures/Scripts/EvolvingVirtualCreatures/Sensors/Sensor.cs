using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {


	public abstract class Sensor {
		abstract public int OutputCount();
		abstract public float[] Output();
	}

}

