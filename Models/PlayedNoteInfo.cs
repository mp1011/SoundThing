namespace SoundThing.Models
{
    class PlayedNoteInfo
    {
        public PlayedNoteInfo(MusicNote note, int octave, double volumePercent, double duration)
        {
            Duration = duration;
            NoteInfo = new NoteInfo(note, octave, volumePercent);
        }

        public PlayedNoteInfo(NoteInfo noteInfo, double duration)
        {
            Duration = duration;
            NoteInfo = noteInfo;
        }

        public double Duration { get; }
        public int SampleDuration => (int)(Duration * Constants.SamplesPerSecond);
        public NoteInfo NoteInfo { get; }

        public PlayedNoteInfo ChangeVolume(double newVolumePercent)
        {
            return new PlayedNoteInfo(NoteInfo.ChangeVolumePercent(newVolumePercent), Duration);
        }

        public PlayedNoteInfo ChangeSampleDuration(int newSampleDuration)
        {
            return new PlayedNoteInfo(NoteInfo, newSampleDuration / (double)Constants.SamplesPerSecond);
        }

        public PlayedNoteInfo ChangeOctave(int octave)
        {
            return new PlayedNoteInfo(NoteInfo.ChangeOctave(octave), Duration);
        }
    }
}
