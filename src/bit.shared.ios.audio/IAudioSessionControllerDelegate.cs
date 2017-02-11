using System;

//using MonoTouch.CoreFoundation;
//using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;

namespace bit.shared.ios.audio
{
    public interface IAudioSessionControllerDelegate
    {              
        void StartingRecording();
        void StoppingRecording();
        void StartingPlayback();
        void StoppingPlayback();
        void InterruptingPlayback();
        void InterruptingRecording();
        void ResumingPlayback();
        void ResumingRecording();  
        void HardwareRouteChange(AudioSessionInputRouteKind input, AudioSessionOutputRouteKind[] outputs);
        void InputAvailable(bool isAvailable);
    }
}

