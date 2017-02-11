using System;

using MonoTouch.CoreFoundation;

using bit.shared.audio;
using bit.shared.ios.audio;

namespace Hello_MultiScreen_iPhone
{
	public class TunerAudioProcessor
	{
		private AudioInputStream _ais;
		private PitchDetector _pd;
        private BackgroundAudioProcessor _bap;
        private PitchDetectorParams _pdp;
        private Action<int,PitchDetectorResult> _fgHandler;

        public TunerAudioProcessor (Action<int,PitchDetectorResult> fgHandler)
		{
			int numPackets = 3200;
            _fgHandler = fgHandler;
            _pdp = new PitchDetectorParams();
            _pd = new PitchDetector(_pdp, processPitchResult);
            _bap = new BackgroundAudioProcessor(_pd);
            _ais = new AudioInputStream(numPackets,_bap);
           
		}

		public void Start()
		{
			_ais.Start();
		}
		
		public void Stop (bool immediate)
		{
			_ais.Stop(immediate);
		}

        public void processPitchResult (int seq, PitchDetectorResult pdr)
        {
            if (_fgHandler != null) {
                DispatchQueue.MainQueue.DispatchAsync (() => {
                    _fgHandler (seq, pdr);});
            }
        }

	}
}

