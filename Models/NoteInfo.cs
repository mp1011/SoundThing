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

        public int Semitones => (Octave * 12) + (int)Note;

        public NoteInfo ChangeVolumePercent(double newVolumePercent)
        {
            return new NoteInfo(Note, Octave, newVolumePercent);
        }
 
        public NoteInfo ChangeOctave(int octave)
        {
            return new NoteInfo(Note, octave, VolumePercent);
        }

        public NoteInfo Step(ScaleStep step)
        {
            return Step((int)step);
        }

        public NoteInfo Step(Interval interval)
        {
            return Step((int)interval);
        }

        public NoteInfo Step(int amount)
        {
            var newNote = Note + amount;
            var newOctave = Octave;

            if(newNote > MusicNote.GSharp)
            {
                newOctave++;
                newNote -= 12;
            }
            if (newNote < MusicNote.A)
            {
                newOctave--;
                newNote += 12;
            }

            return new NoteInfo(newNote, newOctave, VolumePercent);
        }

        public static implicit operator SoundInfo(NoteInfo n) =>
            new SoundInfo(n.Note.GetFrequency(n.Octave), n.VolumePercent);

        public override string ToString()
        {
            return $"{Note.ToString().Replace("Sharp","#")}{Octave}";
        }
    }

}
