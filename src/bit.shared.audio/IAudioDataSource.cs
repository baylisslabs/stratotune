using System;

namespace bit.shared.audio
{
	public interface IAudioDataSource
	{
		void Pull32BitMonoLinearPCM(int[] pcmData, double t, double sampleRate);
        void Pull32BitMonoLinearPCM(double[] pcmData, double t, double sampleRate);
	}
}

