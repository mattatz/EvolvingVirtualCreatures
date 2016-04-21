using UnityEngine;
using Random = UnityEngine.Random;

using System.Collections;

namespace mattatz.Utils {

	public class Gaussian {

		/*
		 * standard normal distribution
		 * https://en.wikipedia.org/wiki/Normal_distribution
		 */
		public static float Std (float mu = 0f, float sigma = 1f) {
			var u1 = Random.value;
			var u2 = Random.value;
			var rand_std_normal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
			return mu + sigma * rand_std_normal;
		}

	}

}

