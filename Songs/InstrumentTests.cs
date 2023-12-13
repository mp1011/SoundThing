using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class InstrumentTest : Song
    {
        protected override IEnumerable<Player> Players
        { 
            get
            {
                var noteBuilder = new NoteEventBuilder(BPM, NoteType.Quarter, Scale);
                noteBuilder.AddQuarters(1, 2, 3, 4, 5, 6, 7, 8, 7, 6, 5, 4, 3, 2, 1);
                noteBuilder.AddChords(NoteType.Half, 1, 4, 5);

                yield return new Player(new TestInstrument(), 0, noteBuilder);
            } 
        }

        protected override int BPM => 120;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
            new NoteInfo(MusicNote.C, 2, 1.0));

       
    }
}
