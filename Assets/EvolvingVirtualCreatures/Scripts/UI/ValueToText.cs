using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace mattatz.UI {

	public class ValueToText : MonoBehaviour {

		[SerializeField] Text text;

		public void Display (float value) {
			text.text = value.ToString();
		}

	}

}

