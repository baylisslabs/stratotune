using System;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;

using bit.shared.logging;

namespace bit.shared.ios.audio
{
    public class AudioSessionControllerState
    {
        public enum Status { Off=0,On,Interrupted,Pending };
        
        public Status Recording { get; set; }
        public Status Playback { get; set; }
        
        public bool IsActive ()
        {
            return (this.Recording==AudioSessionControllerState.Status.On
                    ||this.Playback==AudioSessionControllerState.Status.On);
        }

        public bool IsInterrupted ()
        {
            return (this.Recording==AudioSessionControllerState.Status.Interrupted
                    ||this.Playback==AudioSessionControllerState.Status.Interrupted);
        }

        public bool AnyPending ()
        {
            return (this.Recording==AudioSessionControllerState.Status.Pending
                    ||this.Playback==AudioSessionControllerState.Status.Pending);
        }

        public AudioSessionCategory ToCategory ()
        {
            return AudioSessionCategory.PlayAndRecord;
            /*if (this.Recording != AudioSessionControllerState.Status.Off &&
                this.Playback == AudioSessionControllerState.Status.Off) {
                return AudioSessionCategory.RecordAudio;
            }

            if (this.Recording == AudioSessionControllerState.Status.Off &&
                this.Playback != AudioSessionControllerState.Status.Off) {
                return AudioSessionCategory.MediaPlayback;
            }

            if (this.Recording != AudioSessionControllerState.Status.Off &&
                this.Playback != AudioSessionControllerState.Status.Off) {
                return AudioSessionCategory.PlayAndRecord;
            }

            return AudioSessionCategory.AmbientSound;*/
        }

        public bool IsPlayBackOn { get { return this.Playback == AudioSessionControllerState.Status.On; } }
        public bool IsRecordingOn { get { return this.Recording == AudioSessionControllerState.Status.On; } }
    };
	
}
