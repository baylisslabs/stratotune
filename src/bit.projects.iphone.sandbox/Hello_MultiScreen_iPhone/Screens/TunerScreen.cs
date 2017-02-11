
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AudioToolbox;

using bit.shared.audio;

namespace Hello_MultiScreen_iPhone
{
	public partial class TunerScreen : UIViewController
	{
        TunerAudioProcessor _audioProcessor;
		TunerReadoutView _trv;

		public TunerScreen () : base ("TunerScreen", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		public override void ViewWillAppear (bool animated)
        {
            if (_trv == null) {
                _trv = new TunerReadoutView (vwReadout.Bounds);			
                vwReadout.AddSubview (_trv);
            }
            if (_audioProcessor == null) {
                _audioProcessor = new TunerAudioProcessor(processPitchResult);
            }
			_audioProcessor.Start();
		}

		public override void ViewWillDisappear (bool animated)
        {
            if (_audioProcessor != null) {
                _audioProcessor.Stop (false);
            }
		}			
        		
        private void processPitchResult (int seq, PitchDetectorResult pdr)
        {
            if(_trv!=null&&this.lblValue1!=null) {
                var result = pdr;
                if(result!=null) {
                    string text = String.Format("T={0}, F={1:.##}, Q={2:.##}",
                                                result.ProcessingTimeMs,
                                                result.F_0_Hz.GetValueOrDefault(0),
                                                result.Q_Hz.GetValueOrDefault(0));
                    
                    if(result.F_0_Hz.HasValue) {
                        var note = MidiNote.FromHz(result.F_0_Hz.Value);
                        var q_cents = MidiNote.HzToCents(result.F_0_Hz.Value,result.Q_Hz.GetValueOrDefault(0));
                        
                        text += String.Format(" {0}{1}{2}\u00A2 Q={3}",
                                              note.FlatName(),
                                              note.Octave(),
                                              note.PitchBendCents().ToString("+0;-#"),
                                              q_cents);
                    }
                    
                    this.lblValue1.Text = text;                             
                    _trv.DataPoints = result.CorrelationData;
                    _trv.FirstMin = result.SelectedMinima;
                    _trv.SetNeedsDisplay();
                    
                }
            }
        }                    	
	}
}

