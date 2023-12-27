using SoundThing.Services;

namespace SoundThing.Models
{
    struct ScaleChord 
    {
        public ScaleChord(Scale scale, int number, Chord chord)
        {
            Number = number;
            Chord = chord;
            Scale = scale;
        }

        public int Number { get; }
        public Chord Chord { get; }
        public Scale Scale { get; }

        public override string ToString() => $"{Number}-{Chord}";
    }
}
