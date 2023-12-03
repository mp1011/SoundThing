using SoundThing.Models;

namespace SoundThing.Extensions
{
    static class NoteInfoExtensions
    {
        public static NoteInfo Octave(this NoteInfo note) =>
            new NoteInfo(note.Note, note.Octave + 1, note.VolumePercent);

        public static NoteInfo Fifth(this NoteInfo note)
        {
            var rootFrequency = note.Note.GetFrequency(note.Octave);
            var fifthFrequency = (rootFrequency * 3.0) / 2.0;

            var fifthNote = fifthFrequency.GetNote();
            return new NoteInfo(fifthNote.Item1, fifthNote.Item2, note.VolumePercent);
        }
    }
}
