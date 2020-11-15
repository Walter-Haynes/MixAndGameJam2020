/// <summary>
/// Fires events on every beat, half beat, and quarter beat
/// </summary>
public class BeatDetection : AudioSyncer
{
    # region Delegates
        public delegate void FullBeat(float val); 
        public event FullBeat OnFullBeat;
        public delegate void HalfBeat(float val); 
        public event HalfBeat OnHalfBeat;
        public delegate void QuarterBeat(float val); 
        public event QuarterBeat OnQuarterBeat;
        
    #endregion Delegates

    public override void OnUpdate() {
        base.OnUpdate();

        if (m_isBeat) return;
    }

    public override void OnBeat(float val, BEAT_TYPE beatType) {
        base.OnBeat(val, beatType);
        switch(beatType) {
            case BEAT_TYPE.FULL:
                OnFullBeat?.Invoke(val);
                break;
            case BEAT_TYPE.HALF:
                OnHalfBeat?.Invoke(val);
                break;
            case BEAT_TYPE.QUARTER:
                OnQuarterBeat?.Invoke(val);
                break;
        }
    }
}
