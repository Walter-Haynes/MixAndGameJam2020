using UnityEngine;

/// <summary>
/// Parent class for syncing manipulation
/// </summary>
public class AudioSyncer : MonoBehaviour
{

    #region Inspector

    [Header("Audio Information")]
        [SerializeField, Tooltip("What spectrum value is going to trigger a beat")]
        protected float bias;
        [SerializeField, Tooltip("Minimum interval between each beat")]
        protected float timeStep;
        [SerializeField, Tooltip("How long to get to target scale")]
        protected float timeToBeat;
        [SerializeField, Tooltip("How fast do we go back to rest scale")]
        protected float restSmoothTime;

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
    
    protected virtual void Awake() {
        audioSpectrum = AudioSpectrum.Instance;
    }

    /// <summary>
    /// Notifies us when beat has occured
    /// </summary>
    /// <param name="value">Current spectrum value</param>
    public virtual void OnBeat(float value) {
        Debug.Log("beat");
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

        // TODO(sam): set up half and quarter beat detection

        // Went below bias
        if (previousAudioValue > bias && audioValue <= bias) {
            if (timer > timeStep)
                OnBeat(audioValue);
        }

        
        // Went above bias
        if (previousAudioValue <= bias && audioValue > bias) {
            if (timer > timeStep)
                OnBeat(audioValue);
        }

        // Increment Timer
        timer += Time.deltaTime;
    }

    private void Update() {
        OnUpdate();
    }


}
