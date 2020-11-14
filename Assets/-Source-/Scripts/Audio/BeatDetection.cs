/// <summary>
/// Fires events on every beat, half beat, and quarter beat
/// </summary>
public class BeatDetection : AudioSyncer
{
    public override void OnUpdate() {
        base.OnUpdate();

        if (m_isBeat) return;
    }

    public override void OnBeat(float val) {
        base.OnBeat(val);

        // Threshold
        if (val >= bias + (bias * .5)) {
        }
        else {
        }
    }
}
