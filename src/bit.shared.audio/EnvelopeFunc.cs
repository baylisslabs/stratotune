using System;

namespace bit.shared.audio
{
    public interface EnvelopeFunc
    {
        double F(double t);
    }

    public interface PeriodicEnvelopeFunc : EnvelopeFunc
    {
        double Freq { get; set; }
        double Theta0 { get; set; }        
    }
        
    public abstract class AdjustablePeriodicEnvelopeFunc : PeriodicEnvelopeFunc
    {
        protected const double _2pi = 2.0*Math.PI;
        protected double _freq;
        protected double _omega;
        protected double _theta0;
        protected double _t0;

        public double Freq { get { return _freq; }  set { setFreq(value,0); } }
        public double Theta0 { get { return _theta0; }  set { _theta0 = value; } }
        public abstract double F(double t);

        public void AdjustFreq(double newFreq, double t)
        {
            setFreq(newFreq,t);
        }
                    
        protected virtual void setFreq (double hz, double t)
        {
            _theta0 = _theta0 + _omega*(t-_t0);
            _t0 = t;
            _freq = hz;
            _omega = _2pi*_freq;
        }               
    }

    public static class EnvelopeFuncExtensions
    {
        public class EnvelopeFuncAdapter : EnvelopeFunc
        {
            public Func<double,double> Delegate { get; set; }
          
            public double F(double t)
            {
                return this.Delegate(t);
            }
        }
                       
        public class SineWave : AdjustablePeriodicEnvelopeFunc
        {                      
            public override double F(double t)
            {
                return Math.Sin(_theta0+_omega*(t-_t0));
            }                 
        }

        public class SquareWave : AdjustablePeriodicEnvelopeFunc
        {          
            public override double F(double t)
            {
                var s = _theta0/_2pi + _freq*(t-_t0);
                var s0 = Math.Floor(s);
                if(s-s0<0.5) {
                    return 1.0;
                } else {
                    return -1.0;
                }   
            }
        }

        public class TriangularWave : AdjustablePeriodicEnvelopeFunc
        {           
            public override double F(double t)
            {
                var s = _theta0/_2pi + _freq*(t-_t0);
                var s0 = Math.Floor(s);
                if(s-s0<0.5) {
                    return 4*(s - s0) - 1;
                } else {
                    return 3-4*(s - s0);
                }    
            }
        }

        public class SawtoothWave : AdjustablePeriodicEnvelopeFunc
        {           
            public override double F(double t)
            {
                var s = _theta0/_2pi + _freq*(t-_t0);
                var s0 = Math.Floor(s);
                return 2*(s - s0)-1;
            }
        }              
    }
}

