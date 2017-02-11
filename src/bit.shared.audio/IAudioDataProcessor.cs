using System;

namespace bit.shared.audio
{
	public interface IAudioDataProcessor
	{
		void Process32BitMonoLinearPCM(int[] pcmData, double sampleRate);
		void Process32BitMonoLinearPCM(double[] pcmData, double sampleRate);
	}
}

