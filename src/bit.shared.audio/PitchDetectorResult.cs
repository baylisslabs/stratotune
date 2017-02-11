using System;

using bit.shared.numerics;

namespace bit.shared.audio
{
	public class PitchDetectorResult
	{
		public double? F_0_Hz { get; set; }
		public double? Q_Hz { get; set; }
		public double SamplingRate { get; set; }
		public DiscreteData2D CorrelationData { get; set; }
		public Point2D? SelectedMinima { get; set; }
		public int ProcessingTimeMs { get; set; }
        public DiscreteData2D LeastSquaresData { get; set; }
        public QuadraticD? LeastSquaresParabola { get; set; }
        public DateTime TimeStamp { get; set; }
	}
}

