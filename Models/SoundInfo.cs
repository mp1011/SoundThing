using SoundThing.Extensions;

namespace SoundThing.Models
{
    struct SoundInfo
    {
        public ushort Frequency { get; }
        public double Volume => VolumePercent * Constants.MaxVolume;

        public double VolumePercent { get; }
        public SoundInfo(ushort frequency, double volumePercent)
        {
            Frequency = frequency;
            VolumePercent = volumePercent;
        }

        public SoundInfo(int frequency, double volume) : this((ushort)frequency, volume)
        {
        }

        public SoundInfo(double frequency, double volume) : this((ushort)frequency, volume)
        {
        }

        public SoundInfo ChangeVolumePercent(double volume) => new SoundInfo(Frequency, volume);

        public NoteInfo ToNoteInfo() => this;

        public static implicit operator NoteInfo(SoundInfo s)
        {
            var note = s.Frequency.GetNote();
            return new NoteInfo(note.Item1, note.Item2, s.VolumePercent);
        }
    }

}
