using SoundThing.Extensions;
using SoundThing.Services;

namespace SoundThing.Models
{
    struct NoteInfo
    {
        public NoteInfo(MusicNote note, int octave, double volumePercent)
        {
            Note = note;
            Octave = octave;
            VolumePercent = volumePercent;
        }

        public MusicNote Note { get; }
        public int Octave { get; }
        public double VolumePercent { get; }

        public NoteInfo Step(ScaleStep step)
        {
            return Step((int)step);
        }

        public NoteInfo Step(int amount)
        {
            var newNote = Note + amount;
            var newOctave = Octave;

            if(Note > MusicNote.GSharp)
            {
                newOctave++;
                newNote -= 12;
            }

            return new NoteInfo(newNote, newOctave, VolumePercent);
        }

        public static implicit operator SoundInfo(NoteInfo n) =>
            new SoundInfo(n.Note.GetFrequency(n.Octave), n.VolumePercent);
    }

}
