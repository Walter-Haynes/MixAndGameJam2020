using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Bezier : MonoBehaviour
{
	[SerializeField] private Transform begin, end;
	
	// Update is called once per frame
    void OnDrawGizmos()
    {
		if(begin == null || end == null) return;
		
		for (int i = 0; i < 16; i++)
		{
			Vector3 __start = begin.transform.position;
			Vector3 __stop = end.transform.position;
			
			float __highestPoint = Mathf.Max(__start.y, __stop.y);
			__highestPoint += 0.5f;
			
			Vector2 p1 = Parabola(start: __start, stop: __stop, height: __highestPoint, percentage: i / 16.0f);
			Gizmos.DrawSphere(p1, radius: 0.05f);
		}
	}
	
	public static Vector2 Parabola(in Vector2 start, in Vector2 stop, float height, in float percentage)
	{
		float __Func(float t) => -4 * height * t * t + 4 * height * t;

		Vector2 __midPoint = Vector2.Lerp(start, stop, percentage);

		return new Vector2(__midPoint.x, y: __Func(percentage) + Mathf.Lerp(start.y, stop.y, percentage));
	}
}
