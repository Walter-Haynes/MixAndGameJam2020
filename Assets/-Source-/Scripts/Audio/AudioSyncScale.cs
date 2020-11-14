// Script from https://github.com/coderDarren/RenaissanceCoders_UnityScripting/blob/master/UnityScripting/Assets/Tutorials/MusicSyncVisualizer/AudioSyncScale.cs
// Used for testing
using System.Collections;
using UnityEngine;

public class AudioSyncScale : AudioSyncer {

	private IEnumerator MoveToScale(Vector3 _target)
	{
		Vector3 _curr = transform.localScale;
		Vector3 _initial = _curr;
		float _timer = 0;

		while (_curr != _target)
		{
			_curr = Vector3.Lerp(_initial, _target, _timer / timeToBeat);
			_timer += Time.deltaTime;

			transform.localScale = _curr;

			yield return null;
		}

		m_isBeat = false;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		if (m_isBeat) return;

		transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
	}

	public override void OnBeat(float val)
	{
		base.OnBeat(val);

		StopCoroutine("MoveToScale");
		StartCoroutine("MoveToScale", beatScale);
	}

	public Vector3 beatScale;
	public Vector3 restScale;
}