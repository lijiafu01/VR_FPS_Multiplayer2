using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanoidAnimals
{
	public class rotatePlatform12 : MonoBehaviour
	{
		public float rotateSpeed;
		// Update is called once per frame
		void Update()
		{
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
		}
	}
}