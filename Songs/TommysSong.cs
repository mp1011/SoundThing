using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class TommysSong : Song
    {
        public TommysSong()
        {
        }

        protected override IEnumerable<Player> CreatePlayers()
        {
            var noteBuilder = new NoteEventBuilder(BPM, NoteType.Quarter, Scale);

            noteBuilder
                .Add("1e 1e 6q 5q 4h 0h")
                .Add("1e. 1s 6q 5q 4e. 4s 3q 4q 5h")
                .Add("5q 1e. 1s 5e 5e 1e 1e 5e 5e 4q 5q 0q")
                .Add("1e 1e 6q 5q 4e. 4s 3q 2q 1h");

            yield return new Player(new ParameterModTestInstrument(), 0, noteBuilder);
        }

        protected override int DefaultBPM => 100;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
                   new NoteInfo(MusicNote.C, 3, 1.0));


        public override string ToString() => "Tommy's Song";
    }
}
