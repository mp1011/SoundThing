﻿using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class InstrumentTest : Song
    {
        protected override IEnumerable<Player> CreatePlayers()
        { 
           // yield return BuilderCombinationTest();
            yield return ParserTest();
            //yield return NoteCutoffTest();
        }

        private Player NoteCutoffTest()
        {
            var builder = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                               .AddQuarters(1, 3, 5, 3, 1);

            return new Player(new TestInstrument(), 0, builder);
        }

        private Player BuilderCombinationTest()
        {
            var shape = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                                .AddWholes(1, 4, 5, 2);

            var shape2 = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                            .AddQuarters(5, 5, 6, 7, 8, 8, 7, 6, 5, 5, 4, 3, 2, 2, 1, 2);

            var rhythmA = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                            .Add(EventAction.ChangeInterval, "1q 1q 2q 1q");

            var rhythmB = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                            .Add(EventAction.ChangeInterval, "1e 1e 1q 2q 0e 1e");


            var fifths = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                            .Add(EventAction.AddInterval, "5q 5q 5q 5q");

            var noteBuilder = (shape
                              + (shape * rhythmA)
                              + ((shape * fifths * rhythmB) * 2)
                              + (shape * shape2)
                              ).AddWholes(1);

            return new Player(new LfoTestInstrument(), 0, noteBuilder);
        }

        private Player ParserTest()
        {
            var builder = new NoteEventBuilder(BPM, NoteType.Quarter, Scale)
                               .Add(action: EventAction.PlayScaleNote, 
                                    notes: "w(5 3 2 1) q(1 1# 2 2# 3 3# 4 5 6 7 8 7 6 5 4 3 2 1 -1 -2 -3 -4 -5) q135 e(135 357 579) h(157) s(7 5 3 2 -1) w1");


            return new Player(new ParameterModTestInstrument(), 0, builder);
        }

        protected override int DefaultBPM => 120;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
            new NoteInfo(MusicNote.C, 2, 1.0));

   
    }
}
