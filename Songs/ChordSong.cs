using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class ChordSong : Song
    {
        private readonly ScaleChord _chord;

        public ChordSong(ScaleChord chord)
        {
            _chord = chord;
        }

        protected override int DefaultBPM => 120;

        protected override Scale DefaultScale => _chord.Scale;

        protected override IEnumerable<Player> CreatePlayers()
        {
            var noteBuilder = new NoteEventBuilder(BPM, NoteType.Quarter, _chord.Scale);
            noteBuilder.AddArpeggio(NoteType.Quarter, ArpeggioStyle.Rising, _chord.Chord);
            noteBuilder.AddChord(NoteType.Whole, _chord.Chord);
            yield return new Player(new ParameterModTestInstrument(), 0, noteBuilder);
        }
    }
}
