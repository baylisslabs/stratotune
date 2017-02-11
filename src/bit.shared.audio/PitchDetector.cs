using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using bit.shared.numerics;

namespace bit.shared.audio
{
	public class PitchDetector : IAudioDataProcessor
	{
        private readonly PitchDetectorParams _params;
        private Action<int,PitchDetectorResult> _resultHandler;

        private int _resultSeq;
        private bool _squelch = false;
        private float _min_avg_pwr2 = float.MaxValue;

		public PitchDetector (PitchDetectorParams pdp, Action<int,PitchDetectorResult> resultHandler)
		{
            _params = pdp;
            _resultHandler = resultHandler;
		}

		#region IAudioDataProcessor implementation
	
		public void Process32BitMonoLinearPCM (int[] pcmData, double sampleRate)
		{
			throw new NotImplementedException();
		}

		public void Process32BitMonoLinearPCM (double[] pcmData, double sampleRate)
		{		
            if(pcmData.Length >= _params._last_lag && pcmData.Length >= _params._limit_data_len)
            {
    			var sw = new Stopwatch ();
    			sw.Start ();
              		
                var sdf_result = normalised_square_differences (
                    _params._first_lag,
                    _params._last_lag,
                    _params._step_lag, 
                    pcmData.slice((pcmData.Length-_params._limit_data_len)/2,_params._limit_data_len));			
    
    			var mins = sdf_result.MinimaBetweenZeroCrossing(_params._global_thresh,1.0);
    			var best_min = bestMinimum (mins, _params._local_thresh);
    		    var result = new PitchDetectorResult();
                
    			if (best_min != null) {
    				                
                    var refine_first_lag = Math.Max(1,(int)best_min.Value.X - _params._refine_lag_window / 2);
    				var refine_last_lag = Math.Min(pcmData.Length/2-1,refine_first_lag + _params._refine_lag_window - 1);
    				
                    var refined_sdf_result = normalised_square_differences (
                        refine_first_lag, refine_last_lag, _params._refine_step_lag, pcmData);								
    
                    QuadraticD? quad_out;
                    best_min = refineByLeastSquares(refined_sdf_result, out quad_out);
                    if(best_min.HasValue) {
                        var delay = best_min.Value.X;
                        if(delay >= _params._first_lag && delay <= _params._last_lag) {
            				result.F_0_Hz = sampleRate / delay;
                            result.Q_Hz = quad_out.HasValue?0.0:(sampleRate / (delay + _params._step_lag)) - result.F_0_Hz;
                            result.LeastSquaresData = refined_sdf_result;
                            result.LeastSquaresParabola = quad_out;
                        }
                    }
    			}
    
    			sw.Stop();
       
                result.CorrelationData = sdf_result;
                result.SamplingRate = sampleRate;
                result.ProcessingTimeMs =  (int)sw.ElapsedMilliseconds;
                result.SelectedMinima = best_min;
                result.TimeStamp = DateTime.Now;
                if(_resultHandler!=null) {
                    _resultHandler(_resultSeq++,result);
                }                
            }
		}
				
		#endregion
        
        private Point2D? refineByLeastSquares(DiscreteData2D sdf, out QuadraticD? quad)
        {
            LeastSquaresD lsd = new LeastSquaresD(sdf.Points.ToArray());
            quad = lsd.Parabola2();
            if (quad!=null) {
                var vertex = quad.Value.Vertex;
                if (vertex != null) {
                    return vertex.Value; 
                }
            }
            return null;
        }
              
        /* assumes mins ordered */
		private Point2D? bestMinimum (List<Point2D> ordered_mins, double local_thresh)
		{				
			if (ordered_mins.Count!=0) {
				Point2D best_min = ordered_mins[0];
				foreach(var min in ordered_mins) {
					if(min.Y < best_min.Y) {
						best_min = min;
					}
				}
				foreach(var min in ordered_mins) {
					if(min.X >= best_min.X) {
						break;
					}
					if(min.Y-local_thresh < best_min.Y) {
						best_min = min;
						break;
					}
				}
				return best_min;			
			}
			return null;
		}
           	  
		private DiscreteData2D normalised_square_differences (int first, int last, int step, double[] dataD)
		{	
            var len = dataD.Length;
			var n = (last-first+1)/step;
			var result = new Point2D[n];  
			var cms = new float[len];
			var acf = new float[len];
			var data = new float[len];
			vDSP.vdpsp (dataD, 1, data, 1, len);
            cumulative_squares(cms,data,len);
            var delay = first;
            if(update_squelch(cms[data.Length-1]/len)) {            
    			for(int i=0;i<n;i++) { 
                    result[i].Set(delay,normalised_square_difference_single(delay,data,data,cms,acf,len));
                    delay += step;
    			}
            }
            
			return new DiscreteData2D(result);
		}
        
		private void cumulative_squares(float[] result, float[] data, int len)
        {           
            vDSP.vsq(data,1,result,1,len);   
            /* todo: vector op */
            for(int i=1;i<len;++i) {
                result[i] += result[i-1];
            }
        }
                 
		private unsafe float normalised_square_difference_single (int delay, float[] data, float[] data2,float[] cms, float[] acf, int len)
        {           
            int end = (len - delay);
            float sum_acf;
            float sum_m = cms[end-1] + cms[len-1] - cms[delay-1];
			fixed(float* dataPtr = data, data2Ptr = data2, acfPtr = acf)
            {
				var delayedPtr = data2Ptr + delay;
				vDSP.vmul(dataPtr,1,delayedPtr,1,acfPtr,1,end);
                vDSP.sve(acf,1,out sum_acf,end);
            }
            return (-sum_acf-sum_acf) / sum_m + 1.0f;
        }  
            
        private bool update_squelch (float rms_squared)
        {
            if (rms_squared < _min_avg_pwr2) {
                _min_avg_pwr2 = rms_squared;
            }

            if(_squelch && rms_squared < (_min_avg_pwr2*_params._squelch_close)) {
                _squelch = false;
            }
            else if(!_squelch && rms_squared > (_min_avg_pwr2*_params._squelch_open)) {
                _squelch = true;
            }
            return _squelch;        
        }
	}
}

