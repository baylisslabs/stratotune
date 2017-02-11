using System;

using Jayrock.JsonRpc;

using bit.shared.audio;
using bit.shared.testutil;

namespace bit.shared.audio.FuncTests
{
	
	class TestService : JsonRpcService
	{
        class AudioProcessorAdaptor : IAudioDataProcessor
        {
            public delegate void Process32BitMonoLinearPCMDelegate (double[] pcmData, double sampleRate);
            public Process32BitMonoLinearPCMDelegate Func { get; set; }
                
            #region IAudioDataProcessor implementation
            void IAudioDataProcessor.Process32BitMonoLinearPCM (int[] pcmData, double sampleRate)
            {
                throw new NotImplementedException ();
            }

            void IAudioDataProcessor.Process32BitMonoLinearPCM (double[] pcmData, double sampleRate)
            {
                this.Func(pcmData,sampleRate);
            }
            #endregion
        }
        
		[JsonRpcMethod]
		public PitchDetectorResult PitchDetector_Process32BitMonoLinearPCM (double[] pcmData, double sampleRate)
		{
            var pdp = new PitchDetectorParams()
            {
                _squelch_open = -1,
                _squelch_close = -1               
            };
            PitchDetectorResult pdr = null;
			var pd = new PitchDetector(pdp,(seq,r)=>{pdr=r;});
			pd.Process32BitMonoLinearPCM(pcmData,sampleRate);
			return pdr;			
		}	
        
        [JsonRpcMethod]
        public double[] EnvelopeModulator_Generate (double gain, int nSamples, double sampleRate, string waveForm, double f0)
        {
            var apa = new AudioProcessorAdaptor();
            var em = new EnvelopeModulator(apa);
            double[] result = null;
            em.Gain = gain;
            switch(waveForm) {
                case "SineWave": em.Envelope = new EnvelopeFuncExtensions.SineWave { Freq=f0, Theta0=0 }; break;
                case "SquareWave": em.Envelope = new EnvelopeFuncExtensions.SquareWave { Freq=f0, Theta0=0 };; break;
                case "TriangularWave": em.Envelope = new EnvelopeFuncExtensions.TriangularWave { Freq=f0, Theta0=0 };; break;
                case "SawtoothWave": em.Envelope = new EnvelopeFuncExtensions.SawtoothWave { Freq=f0, Theta0=0 };; break;
            }
            apa.Func = (pcmData_, sampleRate_) => {
                result = pcmData_;
            };
            em.Push(nSamples,sampleRate);
            return result;
        }   
	}
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			JsonPipeRpcDispatcher disp = new JsonPipeRpcDispatcher(new TestService());
            disp.RunLoop(args);
		}
	}
}
