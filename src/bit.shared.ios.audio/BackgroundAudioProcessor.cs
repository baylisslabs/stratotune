using System;
using System.Collections;
using System.Threading;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;

using bit.shared.numerics;
using bit.shared.audio;
using bit.shared.logging;

namespace bit.shared.ios.audio
{
	public class BackgroundAudioProcessor : IAudioDataProcessor
	{
        private static Logger _log = LogManager.GetLogger("BackgroundAudioProcessor");
		private IAudioDataProcessor _processor;
        private DispatchQueue _dispatchQueue;
        volatile private int _lastMsgSeq;
        private int _maxQueueLength;

		public BackgroundAudioProcessor (IAudioDataProcessor processor = null, int maxQueueLength = 1)
		{
            _maxQueueLength = maxQueueLength;
			_processor = processor;
            _dispatchQueue = new DispatchQueue("bit.shared.ios.audio.BackgroundAudioProcessor");
		}
		
		public void Process32BitMonoLinearPCM (int[] pcmData, double sampleRate)
		{
			if (pcmData.Length == 0) {
				return;
			}

            var nextSeq = Interlocked.Increment(ref _lastMsgSeq);
            var pcmDataCopy = new double[pcmData.Length];
            pcmDataCopy = pcmData.to_double();
            _dispatchQueue.DispatchAsync(()=>{backgroundProcess(nextSeq,pcmDataCopy,sampleRate);});			
		}
		
		public void Process32BitMonoLinearPCM (double[] pcmData, double sampleRate)
		{
			throw new NotImplementedException();
		}
		
		private void backgroundProcess (int msgSeq, double[] pcmData, double sampleRate)
        {		
            try {
                int queuedBehind = _lastMsgSeq - msgSeq;
                if (queuedBehind < _maxQueueLength) {
                    if (_processor != null) {
                        _processor.Process32BitMonoLinearPCM (pcmData, sampleRate);
                    }	
                }
            } catch (Exception ex) {
                _log.Error("Error processing audio input",ex);
#if DEBUG
                throw;
#endif  
            }
		}
	}
}

