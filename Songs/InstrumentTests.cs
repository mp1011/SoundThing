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
                var nb1 = new NoteEventBuilder(BPM, NoteType.Quarter, Scale);
                nb1.AddWholes(1, 4, 5, 2);

                var nb2 = new NoteEventBuilder(BPM, NoteType.Quarter, Scale);
                nb2.Add(EventAction.ChangeInterval, "1e 1e 1q 2q 0e 1e");

                var nb3 = nb1 + nb2;
                nb3.AddWholes(1);
                yield return new Player(new SawInstrument(), 0, nb3);
            } 
        }

        protected override int BPM => 120;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
            new NoteInfo(MusicNote.C, 2, 1.0));

       
    }
}
