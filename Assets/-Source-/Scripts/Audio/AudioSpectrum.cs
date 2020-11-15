// Tutorial: https://www.youtube.com/watch?v=PzVbaaxgPco&list=PLbgL4_5N-yCuTVfV-YLM0vtto7mrTOK8O&index=5&t=0s
using UnityEngine;

/// <summary>
/// Get audio spectrum data from Unity's AudioListener
/// </summary>
public class AudioSpectrum : Singleton<AudioSpectrum>
{

    public float spectrumValue
    {
        get;
        private set;
    }

    // Serve music beats
    private float[] audioSpectrum;

    /// <summary>
    /// Initialize audio spectrum 
    /// </summary>
    private void Start()
    {
        audioSpectrum = new float[128]; // Must be power of 2
    }

    /// <summary>
    /// Update spectrum array by getting data from audio listener
    /// </summary>
    private void Update()
    {
        // https://answers.unity.com/questions/1623827/how-to-choose-fft-window-type.html
        // FFTWindow: Spectrum analysis windowing types.
        // Use this to reduce leakage of signals across frequency bands.
        // aka: fancy maths
        AudioListener.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);

        // TODO(sam): better audio detection?
        if (audioSpectrum != null && audioSpectrum.Length > 0) {
            spectrumValue = audioSpectrum[0] * 100; // 100 is an arbitrary value, to denormalize
        }
    }
}
