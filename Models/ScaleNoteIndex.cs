namespace SoundThing.Models
{
    struct ScaleNoteIndex
    {
        public int Value { get; }
        public Accidental Accidental { get; }

        public ScaleNoteIndex(int number) : this(number, Accidental.None) { }

        public ScaleNoteIndex(int number, Accidental accidental)
        {
            Value = number;
            Accidental = accidental;
        }

        public ScaleNoteIndex Sharp() => new ScaleNoteIndex(Value, Accidental.Sharp);
        public ScaleNoteIndex Flat() => new ScaleNoteIndex(Value, Accidental.Flat);
    }
}
