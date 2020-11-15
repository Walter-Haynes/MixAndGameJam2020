using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[Tooltip("Speed in PER BEAT values")]
	[SerializeField] private float speed = 4;

	private void Update()
	{
		float __speedPerSecond = speed * TempBeatThing.Instance.PerBeatToPerSecond;
		float __speedPerFrame = __speedPerSecond * Time.deltaTime;
		
		Debug.Log($"\n Move Speed Per Beat = {speed} \n Move Speed Per Second = {__speedPerSecond}");
		
		Vector3 __translation = new Vector3(0, y: __speedPerFrame,0);

		transform.Translate(__translation);
	}
}
