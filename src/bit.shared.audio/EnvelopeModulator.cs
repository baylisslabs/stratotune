using System;

namespace bit.shared.audio
{
    public class EnvelopeModulator : IAudioDataSource, IAudioDataProcessor
    {
        private IAudioDataSource _inputStage;
        private IAudioDataProcessor _outputStage;      

        private double[] _bufferDbl;
        //private int[] _bufferInt;
        private uint _frameCount;

        private double _gain;
        private EnvelopeFunc _envelope;
           
        public double Gain { get { return _gain; } set { _gain = value; } }
        public EnvelopeFunc Envelope { get { return _envelope; } set { _envelope = value; } }
             
        public EnvelopeModulator ()
        {
            _gain = 0;
            _envelope = new EnvelopeFuncExtensions.EnvelopeFuncAdapter() { Delegate = t=>1.0 };
        }

        public EnvelopeModulator (IAudioDataSource inputStage) : this()
        {           
            _inputStage = inputStage;
        }

        public EnvelopeModulator (IAudioDataProcessor outputStage) : this()
        {           
            _outputStage = outputStage;
        }
        
        /// <summary>
        /// Generate the specified nSamples and sampleRate.
        /// </summary>
        /// <param name='nSamples'>
        /// N samples.
        /// </param>
        /// <param name='sampleRate'>
        /// Sample rate.
        /// </param>
        public void Push (int nSamples, double sampleRate)
        {          
            if (_outputStage != null) {              
                var pcmDataOut = getBufferDbl (nSamples);
                for (int i=0; i<nSamples; i++, _frameCount++) {
                    pcmDataOut [i] = _gain * _envelope.F(_frameCount / sampleRate);
                }           
          
                _outputStage.Process32BitMonoLinearPCM (pcmDataOut, sampleRate);
            }
        }

        #region IAudioDataSource implementation

        public void Pull32BitMonoLinearPCM (int[] pcmData, double t, double sampleRate)
        {
            throw new NotImplementedException ();
        }

        public void Pull32BitMonoLinearPCM (double[] pcmData, double t, double sampleRate)
        {
            int nSamples = pcmData.Length;           
            if (_inputStage != null) {               
                _inputStage.Pull32BitMonoLinearPCM (pcmData, t, sampleRate);
                for(int i=0;i<nSamples;i++) {
                    pcmData[i] = _gain * pcmData[i] * _envelope.F(t+i/sampleRate);
                }  
            } else {
                for(int i=0;i<nSamples;i++) {
                    pcmData[i] = _gain * _envelope.F(t+i/sampleRate);
                }  
            }                    
        }

        #endregion

        #region IAudioDataProcessor implementation
        public void Process32BitMonoLinearPCM (int[] pcmData, double sampleRate)
        {
            throw new NotImplementedException ();
        }

        public void Process32BitMonoLinearPCM (double[] pcmData, double sampleRate)
        {           
            int nSamples = pcmData.Length;
            var pcmDataOut = getBufferDbl(nSamples);
            for(int i=0;i<nSamples;i++,_frameCount++) {
                pcmDataOut[i] = _gain * pcmData[i] * _envelope.F(_frameCount/sampleRate);
            }
            if (_outputStage != null) {
                _outputStage.Process32BitMonoLinearPCM (pcmDataOut, sampleRate);
            }
        }
        #endregion
        
        private double[] getBufferDbl(int size)
        {
            if(_bufferDbl==null || _bufferDbl.Length!=size) {
                _bufferDbl = new double[size];
            }
            return _bufferDbl;
        }            
    }        
}

