using System;
using System.Runtime.InteropServices;

using MonoTouch.AudioToolbox;
using MonoTouch.CoreFoundation;

using bit.shared.audio;

namespace bit.shared.ios.audio
{
	public class AudioOutputStream
	{
        private IAudioDataSource _source;
		private OutputAudioQueue _outputAudioQ;
        private AudioStreamBasicDescription _audioFormat;
        private AudioDispatchQueue _dispatchQueue;

		private int _numPackets;
		private int _bufferBytes;
		private int _numBuffers;
        private double _samplingRate;
		private int[] _dataBuf;
        private double[] _dataBufDbl;
	
        private int _frameCounter;
        private volatile int _enable;
        private volatile float _masterGain;
        private double _gain;
        private double _attack_decay;
        private double _onThresh;
        private readonly int _dispatchQueueMaxLen = 128;

        public bool IsRunning { get { return _outputAudioQ.IsRunning; } }
        public bool IsEnabled { get { return (_enable==1); } }
        public float MasterGain { get { return _masterGain; } set { _masterGain = value; } }
        public double CurrentSliceTime { get { return _frameCounter / _samplingRate; } }

        public AudioOutputStream (int numPackets, int numBuffers, double samplingRate, IAudioDataSource sourceIn)
		{		
            _source = sourceIn;
            _numPackets = numPackets;
            _numBuffers = numBuffers;
            _samplingRate = samplingRate;
			_audioFormat = new AudioStreamBasicDescription
			{
				BitsPerChannel = 32,
				BytesPerFrame = 4,
				BytesPerPacket = 4,
				ChannelsPerFrame = 1,
				Format = AudioFormatType.LinearPCM,
				FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked,
				FramesPerPacket = 1,
				SampleRate = _samplingRate,
			};
					
            _outputAudioQ = new OutputAudioQueue(_audioFormat);
            _audioFormat = _outputAudioQ.AudioStreamPacketDescription;
            _dispatchQueue = new AudioDispatchQueue(_dispatchQueueMaxLen);
            _outputAudioQ.OutputCompleted += onOutputCompleted;
			_dataBuf = new int[_numPackets];
            _dataBufDbl = new double[_numPackets];
			_samplingRate = _audioFormat.SampleRate;
			_bufferBytes = _numPackets * _audioFormat.BytesPerPacket;
            _attack_decay =  5.0 / _samplingRate;
            _onThresh = 1.0/int.MaxValue;
            _gain = 0;

			for (int i=0; i<_numBuffers; ++i) {
				IntPtr bufPtr;
                _outputAudioQ.AllocateBuffer(_bufferBytes,out bufPtr);
                fillBuffer(bufPtr);
                _outputAudioQ.EnqueueBuffer(bufPtr, _bufferBytes, null);
			}		
		}
			
		public void Start ()
        {
            int count;
            _outputAudioQ.Prime(0,out count);
            _outputAudioQ.Start ();          
		}

		public void Stop (bool immediate)
        {
            _outputAudioQ.Stop (immediate);	          
		}

        public void SetEnable (bool value)
        {
            _enable = value?1:0;
        }

        public void DispatchAsync (int msgId, bool isSuperSeedable, Action action)
        {
            _dispatchQueue.Push(msgId,isSuperSeedable,action);
        }

		private void onOutputCompleted (object sender, OutputCompletedEventArgs e)
		{	
			var bufPtr = e.IntPtrBuffer;
			if (bufPtr != IntPtr.Zero) {				
                fillBuffer(e.IntPtrBuffer);
                _outputAudioQ.EnqueueBuffer (bufPtr, _bufferBytes, null);
			}
		}

        private void fillBuffer(IntPtr bufPtr)
        {         
            var aqb = (AudioQueueBuffer)Marshal.PtrToStructure (bufPtr, typeof(AudioQueueBuffer));
            if(aqb.AudioData != IntPtr.Zero && aqb.AudioDataByteSize==_bufferBytes) {
                processDispatchQ ();
                loadNextBuffer();
                Marshal.Copy (_dataBuf, 0, aqb.AudioData, _numPackets);
            }
            _frameCounter += _numPackets;
        }

        private void loadNextBuffer ()
        {
            if (_gain > _onThresh || (_enable!=0)) {            
                _source.Pull32BitMonoLinearPCM (_dataBufDbl, _frameCounter / _samplingRate, _samplingRate);
                for (int i=0; i<_dataBuf.Length; ++i) {                   
                    _gain = _attack_decay*((_masterGain*_enable) - _gain) + _gain;
                    _dataBuf [i] = (int)(clamp (_gain * _dataBufDbl [i]) * Int32.MaxValue);
                }
            } else {
                for (int i=0; i<_dataBuf.Length; ++i) {
                    _dataBuf [i] = 0;
                }
                _gain = 0;
            }            
        }

        private void processDispatchQ ()
        {
            for(int i=0;i<_dispatchQueueMaxLen;++i) {
                if (!_dispatchQueue.ExecuteNext()) {
                    break;
                }
            }
        }

        private double clamp (double x)
        {
            if (x > 1.0) {
                return 1.0;
            } else if (x < -1.0) {
                return -1.0;
            }
            return x;
        }
	}
}

