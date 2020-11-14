/// <summary>
/// Fires events on every beat, half beat, and quarter beat
/// </summary>
public class BeatDetection : AudioSyncer
{
    # region Delegates
        public delegate void FullBeat(float val); 
        private FullBeat fullBeatDelegate;
        public delegate void HalfBeat(float val); 
        private HalfBeat halfBeatDelegate;
        public delegate void QuarterBeat(float val); 
        private QuarterBeat quarterBeatDelegate;
    #endregion Delegates

    public override void OnUpdate() {
        base.OnUpdate();

        if (m_isBeat) return;
    }

    public override void OnBeat(float val, BEAT_TYPE beatType) {
        base.OnBeat(val, beatType);
        switch(beatType) {
            case BEAT_TYPE.FULL:
                if (fullBeatDelegate != null) fullBeatDelegate(val);
                break;
            case BEAT_TYPE.HALF:
                if (fullBeatDelegate != null) halfBeatDelegate(val);
                break;
            case BEAT_TYPE.QUARTER:
                if (fullBeatDelegate != null) quarterBeatDelegate(val);
                break;
        }
    }
}
