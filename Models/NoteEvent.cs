namespace SoundThing.Models
{
    struct NoteEvent
    {
        public NoteEvent(PlayedNoteInfo note, double startTime)
        {
            Note = note;
            SampleIndexStart = (int)(startTime * Constants.SamplesPerSecond);
        }

        public NoteEvent(PlayedNoteInfo note, int startIndex)
        {
            Note = note;
            SampleIndexStart = startIndex;
        }

        public PlayedNoteInfo Note { get; }
        public int SampleIndexStart { get; }
        public int SampleIndexEnd => SampleIndexStart + Note.SampleDuration;

        public double StartTime => SampleIndexStart / (double)Constants.SamplesPerSecond;
        public double Duration => Note.Duration;
        public bool IsRest => Note.NoteInfo.Note == MusicNote.Rest;
        public int SampleDuration => Note.SampleDuration;
        public MusicNote MusicNote => Note.NoteInfo.Note;

        public DrumPart DrumPart => (DrumPart)Note.NoteInfo.Note;

        public NoteEvent ChangeVolume(double newVolumePercent)
        {
            return new NoteEvent(Note.ChangeVolume(newVolumePercent), SampleIndexStart);
        }

        public NoteEvent ChangeSampleDuration(int newDuration)
        {
            return new NoteEvent(Note.ChangeSampleDuration(newDuration), SampleIndexStart);
        }

        public NoteEvent ChangeStartTime(double newStart)
        {
            return new NoteEvent(Note, newStart);
        }

        public NoteEvent ChangeOctave(int octave)
        {
            return new NoteEvent(Note.ChangeOctave(octave), SampleIndexStart);
        }

        public NoteEvent ShiftStart(int amount) => new NoteEvent(Note, SampleIndexStart + amount);

        public static implicit operator SoundInfo(NoteEvent n) =>
            n.Note.NoteInfo;

        public override string ToString()
            => $"{Note.NoteInfo.Note} {StartTime} ({Duration})";
    }
}
