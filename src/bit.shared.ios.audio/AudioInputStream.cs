using System;
using System.Runtime.InteropServices;

using MonoTouch.AudioToolbox;

using bit.shared.audio;

namespace bit.shared.ios.audio
{
	public class AudioInputStream
	{
		private IAudioDataProcessor _processor;
		private InputAudioQueue _inputAudioQ;
        private AudioStreamBasicDescription _audioFormat;
	
		private int _numPackets;
		private int _bufferBytes;
		private int _numBuffers; // = 3
        private double _samplingRate; // = 44100
		private int[] _dataBuf;
	
        private volatile bool _enable;

        public bool IsRunning { get { return _inputAudioQ.IsRunning; } }
        public bool IsEnabled { get { return _enable; } }

        public AudioInputStream (int numPackets, int numBuffers, double samplingRate, IAudioDataProcessor processorIn)
		{		
			_processor = processorIn;
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
					
			_inputAudioQ = new InputAudioQueue (_audioFormat);
			_audioFormat = _inputAudioQ.AudioStreamPacketDescription;
			_inputAudioQ.InputCompleted += onInputCompleted;
			_dataBuf = new int[_numPackets];
			_samplingRate = _audioFormat.SampleRate;
			_bufferBytes = _numPackets * _audioFormat.BytesPerPacket;

			for (int i=0; i<_numBuffers; ++i) {
				IntPtr bufPtr;
				_inputAudioQ.AllocateBuffer(_bufferBytes,out bufPtr);
				_inputAudioQ.EnqueueBuffer(bufPtr, _bufferBytes, null);
			}		
		}
			
		public void Start ()
        {
            _inputAudioQ.Start ();          
		}

		public void Stop (bool immediate)
        {          
            _inputAudioQ.Stop (immediate);		
		}

        public void SetEnable (bool value)
        {
            _enable = value;
        }

		private void onInputCompleted (object sender, InputCompletedEventArgs e)
		{	
			var bufPtr = e.IntPtrBuffer;
			if (bufPtr != IntPtr.Zero) {
				var aqb = (AudioQueueBuffer)Marshal.PtrToStructure (bufPtr, typeof(AudioQueueBuffer));	
                if(_enable && _inputAudioQ.IsRunning && aqb.AudioData != IntPtr.Zero && aqb.AudioDataByteSize==_bufferBytes) {
					Marshal.Copy (aqb.AudioData, _dataBuf, 0, _numPackets);
					_processor.Process32BitMonoLinearPCM(_dataBuf,_samplingRate);
				}
				_inputAudioQ.EnqueueBuffer (bufPtr, _bufferBytes, null);
			}
		}
	}
}

