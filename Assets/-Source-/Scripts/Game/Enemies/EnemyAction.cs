using System;
using UnityEngine;

using JetBrains.Annotations;

using Lean.Transition;

namespace Scripts.Game.Enemies
{
    public abstract class EnemyAction : ScriptableObject
    {

        #region Fields

		[Tooltip("Which type of Beat to react on (Full/Half/Quart)")]
        [SerializeField] protected BeatType beatType;

        [Tooltip("React every Nth Full/Half/Quart Beat")]
        [SerializeField] protected int reactEvery = 4;

        //private BeatDetection _beatDetectorCache;
        //private BeatDetection BeatDetector => _beatDetectorCache = (_beatDetectorCache ? _beatDetectorCache : FindObjectOfType<BeatDetection>());

        #endregion

		#region Properties

		[PublicAPI]
		public BaseEnemy Enemy { get; internal set; }
		
        private static TempBeatThing BeatDetector => TempBeatThing.Instance;
		
		#endregion

        #region Methods

		protected enum BeatType
        {
			Full,
			Half,
			Quart
        }

		[PublicAPI]
        public virtual void Initialize()
        {
            switch (beatType)
            {
				case BeatType.Full:
					BeatDetector.OnFullBeat += fullBeatCount =>
					{
						if(fullBeatCount % reactEvery == 0)
						{
							React();
						}
					};
					break;
				
				case BeatType.Half:
					BeatDetector.OnHalfBeat += halfBeatCount =>
					{
						if(halfBeatCount % reactEvery == 0)
						{
							React();
						}
					};
					break;
				
				case BeatType.Quart:
					BeatDetector.OnQuartBeat += quartBeatCount =>
					{
						if(quartBeatCount % reactEvery == 0)
						{
							React();
						}
					};
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
            }
        }

		protected abstract void React();

		#endregion
	}
}
