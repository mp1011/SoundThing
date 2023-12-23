using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class ItsyBitsySpider : Song
    {
        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
                 new NoteInfo(MusicNote.C, 3, 1.0));

        protected override int DefaultBPM => 160;
        protected override IEnumerable<Player> CreatePlayers()
        {
            var noteBuilder = new NoteEventBuilder(BPM, NoteType.Quarter, Scale.Flat(6));

            noteBuilder
                .Add("1e 4q 3e 4q 5e 6q. 6q 6e 5q 4e 5q 6e 4q. 0q.")
                .Add("6q. 6q 7e 8q. 8q. 7q 6e 7q 8e 6q. 0q.")
                .Add("4q. 4q 5e 6q. 6q. 5q 4e 5q 6e 4q. 1q 1e")
                .Add("4q 3e 4q 5e 6q. 6q 6e 5q 4e 5q 6e 4h. 0w");

            yield return new Player(new PluckyInstrument(), 0, noteBuilder);
        }
    }
}
