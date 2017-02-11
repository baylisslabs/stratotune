using System;

namespace bit.projects.iphone.chromatictuner.model
{
    [Flags]
    public enum FlipFlopOp
    {
        Hold = 0x0,
        Reset = 0x1,
        Set = 0x2,
        Toggle = 0x3
    }
 
    public static class FlipFlopOpExtensions
    {
        public static bool NextState (this FlipFlopOp input, bool state)
        {
            switch (input) {
            case FlipFlopOp.Hold: return state;
            case FlipFlopOp.Reset : return false;
            case FlipFlopOp.Set : return true;
            case FlipFlopOp.Toggle : return !state;
            default:
                throw new ArgumentException();
            }
        }
    }
}
