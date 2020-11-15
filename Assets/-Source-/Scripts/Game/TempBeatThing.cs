using System;

using UnityEngine;

using JetBrains.Annotations;

using Scripts.Utilities;

public class TempBeatThing : SingletonMonoBehaviour<TempBeatThing>
{
    [SerializeField] private float bpm = 115;

    [PublicAPI]
    public float BeatsPerMinute => bpm;
    [PublicAPI]
    public float BeatsPerSecond => (bpm / 60.0f);
    [PublicAPI]
    public float TimeBetweenFullBeats => (1.0f / BeatsPerSecond); //in seconds.
    [PublicAPI]
    public float TimeBetweenHalfBeats => (TimeBetweenFullBeats / 2f);
    [PublicAPI]
    public float TimeBetweenQuartBeats => (TimeBetweenHalfBeats / 2f);

    private float _fullBeatTimer, _halfBeatTimer, _quartBeatTimer;
    private int _fullBeats, _halfBeats, _quartBeats;

    [PublicAPI]
    public float PerBeatToPerSecond => TimeBetweenFullBeats;

    [PublicAPI]
    public event Action<int> OnFullBeat;
    [PublicAPI]
    public event Action<int> OnHalfBeat;
    [PublicAPI]
    public event Action<int> OnQuartBeat;

    private void Update()
    {
        _fullBeatTimer  += Time.deltaTime;
        _halfBeatTimer  += Time.deltaTime;
        _quartBeatTimer += Time.deltaTime;
 
        if(_fullBeatTimer  >= TimeBetweenFullBeats)
        {
            _fullBeats++;
            OnFullBeat?.Invoke(_fullBeats);
            _fullBeatTimer -= TimeBetweenFullBeats;
        }
        if(_halfBeatTimer  >= TimeBetweenHalfBeats)
        {
            _halfBeats++;
            OnHalfBeat?.Invoke(_halfBeats);
            _halfBeatTimer -= TimeBetweenHalfBeats;    
        }
        if(_quartBeatTimer >= TimeBetweenQuartBeats)
        {
            _quartBeats++;
            OnQuartBeat?.Invoke(_quartBeats);
            _quartBeatTimer -= TimeBetweenQuartBeats;
        }
    }
}
