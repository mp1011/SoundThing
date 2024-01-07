using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class WheelsOnTheBus : Song
    {
        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
                 new NoteInfo(MusicNote.C, 3, 1.0));

        protected override int DefaultBPM => 160;
        protected override IEnumerable<Player> CreatePlayers()
        {
         
            // the wheels on the bus go round and round ...
            var a = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                .Add("1q 4e. 4s 4e. 4s 4q 6q 8q 6q 4h");

            // round and round, round and round
            var b = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
               .Add("5q 3q 1h 8q 6q 4q");

            // all through the town
            var c = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
             .Add("5h 1q. 1e 4w");

            var refrain = a + b + a + c;
            //var b = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
            // .Add("1q 4e. 4s 4e. 4s 4q 6q 8q 6q 4h 5q 3q 1h 8q 6q 4q");

            yield return new Player(new ParameterModTestInstrument(), 0, refrain);
        }
    }
}
