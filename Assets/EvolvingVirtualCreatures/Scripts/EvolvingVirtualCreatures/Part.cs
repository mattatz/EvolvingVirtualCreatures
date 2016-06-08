using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	// [RequireComponent (typeof(Rigidbody))]
	public class Part : MonoBehaviour {

		public bool[] Contacts { 
			get { 
				return new bool[] { front.Contact, back.Contact, left.Contact, right.Contact, up.Contact, down.Contact };
			} 
		}

		public float[] Contacts01 {
			get { 
				return new float[] { front.Contact01, back.Contact01, left.Contact01, right.Contact01, up.Contact01, down.Contact01 };
			} 
		}

		[SerializeField] protected Face front, back, left, right, up, down;

	}

}

