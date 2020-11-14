using UnityEngine;

/// <summary>
/// Parent class for syncing manipulation
/// </summary>
public class AudioSyncer : MonoBehaviour
{

    #region Inspector

    [Header("Audio Information")]
        [Header("Full beats")]
            [SerializeField, Tooltip("What spectrum value is going to trigger a beat")]
            protected float bias = 25;
            [SerializeField, Tooltip("Minimum interval between each beat")]
            protected float timeStep = 0.15f;
            [SerializeField, Tooltip("How long to get to target scale")]
            protected float timeToBeat = 0.05f;
            [SerializeField, Tooltip("How fast do we go back to rest scale")]
            protected float restSmoothTime = 2f;
        [Header("Half beats")]
            [SerializeField, Tooltip("What spectrum value is going to trigger a half beat")]
            protected float biasHalf = 25 / 2f;
            [SerializeField, Tooltip("Minimum interval between each half beat")]
            protected float timeStepHalf = 0.15f / 2f;
            [SerializeField, Tooltip("How long to get to target scale")]
            protected float timeToBeatHalf = 0.05f /2f;
            [SerializeField, Tooltip("How fast do we go back to rest scale")]
            protected float restSmoothTimeHalf = 2f / 2f;
        [Header("Quarter beats")]
            [SerializeField, Tooltip("What spectrum value is going to trigger a quarter beat")]
            protected float biasQuarter = 25f / 4f;
            [SerializeField, Tooltip("Minimum interval between each quarter beat")]
            protected float timeStepQuarter = 0.15f / 4f;
            [SerializeField, Tooltip("How long to get to target scale")]
            protected float timeToBeatQuarter = 0.05f / 4f;
            [SerializeField, Tooltip("How fast do we go back to rest scale")]
            protected float restSmoothTimeQuarter = 2f / 4f;

    #endregion Inspector

    #region Temp

        // Previous value went above or below bias
        private float previousAudioValue;
        // Current value
        private float audioValue;
        // Keep track of time step interval
        private float timer;
        // Currently a beat?
        protected bool m_isBeat;
        // Reference to audio spectrum
        AudioSpectrum audioSpectrum;

    #endregion Temp

    public enum BEAT_TYPE {
        FULL,
        HALF,
        QUARTER
    }
    
    protected virtual void Start() {
        audioSpectrum = AudioSpectrum.Instance;
    }

    /// <summary>
    /// Notifies us when beat has occured
    /// </summary>
    /// <param name="value">Current spectrum value</param>
    public virtual void OnBeat(float value, BEAT_TYPE beatType) {
        Debug.Log("beat " + beatType);
        timer = 0;
        m_isBeat = true;
    }

    /// <summary>
    /// Virtual to allow subclasses to override Update
    /// </summary>
    public virtual void OnUpdate() {
        // Assign previous audio value and get current one
        previousAudioValue = audioValue;
        audioValue = audioSpectrum.spectrumValue;

        ////// Full Beat
        // Went below bias
        if (previousAudioValue > bias && audioValue <= bias) {
            if (timer > timeStep)
                OnBeat(audioValue, BEAT_TYPE.FULL);
        }
        // Went above bias
        if (previousAudioValue <= bias && audioValue > bias) {
            if (timer > timeStep)
                OnBeat(audioValue, BEAT_TYPE.FULL);
        }
        ////// Half Beat
        if (previousAudioValue > biasHalf && audioValue <= biasHalf) {
            if (timer > timeStepHalf)
                OnBeat(audioValue, BEAT_TYPE.HALF);
        }
        if (previousAudioValue <= biasHalf && audioValue > biasHalf) {
            if (timer > timeStepHalf)
                OnBeat(audioValue, BEAT_TYPE.HALF);
        }
        ////// Quarter Beat
        if (previousAudioValue > biasQuarter && audioValue <= biasQuarter) {
            if (timer > timeStepQuarter)
                OnBeat(audioValue, BEAT_TYPE.QUARTER);
        }
        if (previousAudioValue <= biasQuarter && audioValue > biasQuarter) {
            if (timer > timeStepQuarter)
                OnBeat(audioValue, BEAT_TYPE.QUARTER);
        }
        // Increment Timer
        timer += Time.deltaTime;
    }

    private void Update() {
        OnUpdate();
    }


}
