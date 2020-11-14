using System.Collections;
using UnityEngine;

public class AudioSyncScale : AudioSyncer {

	[SerializeField]
	private BEAT_TYPE type;
	[SerializeField]
	public Vector3 beatScale;
	[SerializeField]
	public Vector3 restScale;

	private IEnumerator MoveToScale(Vector3 _target, BEAT_TYPE beatType)
	{
		if (beatType != type) yield break;

		Vector3 _curr = transform.localScale;
		Vector3 _initial = _curr;
		float _timer = 0;

		while (_curr != _target)
		{
			switch(type) {
				case BEAT_TYPE.FULL:
					_curr = Vector3.Lerp(_initial, _target, _timer / timeToBeat);
					break;
				case BEAT_TYPE.HALF:
					_curr = Vector3.Lerp(_initial, _target, _timer / timeToBeatHalf);
					break;
				case BEAT_TYPE.QUARTER:
					_curr = Vector3.Lerp(_initial, _target, _timer / timeToBeatQuarter);
					break;
			}
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

		switch(type) {
			case BEAT_TYPE.FULL:
				transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
				break;
			case BEAT_TYPE.HALF:
				transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTimeHalf * Time.deltaTime);
				break;
			case BEAT_TYPE.QUARTER:
				transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTimeQuarter * Time.deltaTime);
				break;
		}
	}

	public override void OnBeat(float val, BEAT_TYPE beatType)
	{
		base.OnBeat(val, beatType);

		StopCoroutine("MoveToScale");
		StartCoroutine(MoveToScale(beatScale, beatType));
	}
}
