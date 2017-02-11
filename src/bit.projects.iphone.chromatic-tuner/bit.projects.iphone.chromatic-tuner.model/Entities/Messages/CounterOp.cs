using System;

namespace bit.projects.iphone.chromatictuner.model
{
    [Flags]
    public enum CounterOp
    {
        Hold = 0x0,
        Set = 0x1,
        Reset = 0x2,
        Up = 0x3,
        Down = 0x7
    }
    
    public static class CounterOpExtensions
    {
        public static int NextState (this CounterOp input, int state, int numStates)
        {
            switch (input) {
            case CounterOp.Hold: return state;
            case CounterOp.Reset : return 0;
            case CounterOp.Set : return numStates-1;
            case CounterOp.Up : return ((state+1) % (numStates));
            case CounterOp.Down : return ((numStates+state-1) % (numStates));
            default:
                throw new ArgumentException();
            }
        }
    }     
}
