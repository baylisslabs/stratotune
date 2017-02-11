using System;

namespace bit.shared.audio
{
	public struct MidiNote
	{       
        public const double A4_STANDARD_HZ = 440;               
        public const  int SEMI_TONES_PER_OCTAVE = 12;

        public static readonly double _LOG_BASE_2 = Math.Log(2);
               
        private static string[] sharp_lookup = new string[] {"C","C\u266f","D","D\u266f","E","F","F\u266f","G","G\u266f","A","A\u266f","B"};
		private static string[] flat_lookup = new string[] {"C","D\u266d","D","E\u266d","E","F","G\u266d","G","A\u266d","A","B\u266d","B"};

		private readonly double _noteNumber;

		private MidiNote (double noteNumber)
		{
			_noteNumber = noteNumber;
		}

        public static MidiNote FromNote(double noteNumber) {
            return new MidiNote(noteNumber);
        }

        public static MidiNote FromHz(double hz, double a4hz = A4_STANDARD_HZ) {
			double note = _note_from_freq_even_temp(hz,a4hz);
			return new MidiNote(note);
		}

        public static MidiNote FromOctaveAndSemitone(int octave, double semiTone) {
            return new MidiNote(((octave+1)*SEMI_TONES_PER_OCTAVE)+semiTone);
        }
	
		public static int HzToCents (double f0_hz, double bend_hz)
		{
			var n1 = _note_from_freq_even_temp(f0_hz,A4_STANDARD_HZ);
            var n2 = _note_from_freq_even_temp(f0_hz+bend_hz,A4_STANDARD_HZ);
			return (int)Math.Round((n2 - n1)*100);
		}

		public int PitchBendCents ()
		{
			return (int)Math.Round((_noteNumber - (double)_nearestNote())*100.0);
		}

        public double PitchBendFraction ()
        {
            return _noteNumber - (double)_nearestNote();
        }

		public string SharpName ()
		{
            return this.Name(sharp_lookup);
		}

		public string FlatName ()
		{
            return this.Name(flat_lookup);
		}

        public string Name (string[] lookup)
        {
            var st = this.SemiTone ();
            if (st < lookup.Length) {
                return lookup [this.SemiTone ()];
            }
            return string.Empty;
        }
               
		public int Octave ()
		{
			return (_nearestNote() / SEMI_TONES_PER_OCTAVE) - 1;
		}

		public int SemiTone ()
		{
			return (_nearestNote() % SEMI_TONES_PER_OCTAVE);
		}

        public double SemiToneWithFraction ()
        {
            return _noteNumber - (Math.Floor(_noteNumber/SEMI_TONES_PER_OCTAVE)*SEMI_TONES_PER_OCTAVE);
        }

        public double SemiToneDistance (MidiNote other)
        {
            var semiToneA = this.SemiToneWithFraction ();
            var semiToneB = other.SemiToneWithFraction ();
            var diff = semiToneA - semiToneB;
            if (diff >= SEMI_TONES_PER_OCTAVE/2d) {
                diff -= SEMI_TONES_PER_OCTAVE;
            }
            else if (diff < -SEMI_TONES_PER_OCTAVE/2d) {
                diff += SEMI_TONES_PER_OCTAVE;
            }
            return diff;
        }

        public MidiNote Transpose (double semiToneShift)
        {
            return new MidiNote(_noteNumber+semiToneShift);
        }

        public MidiNote TransposeOctave (int octaveShift)
        {
            return new MidiNote(_noteNumber+(octaveShift*SEMI_TONES_PER_OCTAVE));
        }

        public double ToHz (double a4Hz)
        {
            return _freq_from_note_even_temp(_noteNumber,a4Hz);
        }

        public MidiNote NearestNote ()
        {
            return MidiNote.FromNote(_nearestNote());
        }

        public double NoteNumber ()
        {
            return _noteNumber;
        }
            
        public static bool operator>(MidiNote lhs, MidiNote rhs) 
        {
            return lhs._noteNumber>rhs._noteNumber;
        }

        public static bool operator<(MidiNote lhs, MidiNote rhs) 
        {
            return lhs._noteNumber<rhs._noteNumber;
        }

        public static bool operator==(MidiNote lhs, MidiNote rhs) 
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(MidiNote lhs, MidiNote rhs) 
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals (object obj)
        {
            if (obj == null || GetType () != obj.GetType ()) {
                return false;
            }
            MidiNote rhs = (MidiNote)obj;
            return this._noteNumber == rhs._noteNumber;
        }   

        public bool Equals (MidiNote rhs)
        {         
            return this._noteNumber == rhs._noteNumber;
        }   

        public override int GetHashCode ()
        {
            return _noteNumber.GetHashCode();
        }

		private int _nearestNote ()
		{
			return (int)Math.Round(_noteNumber);
		}

		private static double _note_from_freq_even_temp(double hz, double a4Hz) 
        {
            return 69.0+12.0*Math.Log(hz/a4Hz)/_LOG_BASE_2;
		}

        private static double _freq_from_note_even_temp(double noteNumber, double a4Hz) 
        {
            return Math.Pow(2.0,(noteNumber-69.0)/12.0)*a4Hz;
        }
	}

    public static class MidiNoteExtensions
    {
        public static class Notes
        {
            public static readonly MidiNote C0 = MidiNote.FromNote(12);
            public static readonly MidiNote C4 = MidiNote.FromNote(60);
            public static readonly MidiNote C5 = MidiNote.FromNote(72);
            public static readonly MidiNote C9 = MidiNote.FromNote(120);
            public static readonly MidiNote C10 = MidiNote.FromNote(132);
        }

        public enum SemiTones
        {
            C = 0,
            Cis = 1,
            D = 2,
            Es = 3,
            E = 4,
            F = 5,
            Fis = 6,
            G = 7,
            As = 8,
            A = 9,
            Bes = 10,
            B = 11
        }
    }

}

