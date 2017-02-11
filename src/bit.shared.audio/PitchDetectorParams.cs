using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using bit.shared.numerics;

namespace bit.shared.audio
{
	public class PitchDetectorParams
	{
        public int _limit_data_len = 3600;  // limits sdf sample length for for coarse detection only
        public int _first_lag = 10;
        public int _last_lag = 1800;                   
        public int _step_lag = 1;
        public int _refine_lag_window = 8;
        public int _refine_step_lag = 1;
        public double _global_thresh = 0.2;
        public double _local_thresh = 0.05;
        public double _squelch_open = 10; 
        public double _squelch_close = 2; 
    }
}
       