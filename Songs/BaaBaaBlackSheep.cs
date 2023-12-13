using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class BaaBaaBlackSheep : Song
    {

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
             new NoteInfo(MusicNote.C, 2, 1.0));

        protected override int BPM => 120;

        protected override IEnumerable<Player> Players
        {
            get
            {
                var drumBuilder = new NoteEventBuilder(scale: Scale, bpm: BPM, beatNote: NoteType.Quarter);

                drumBuilder.AddGroup(NoteType.Quarter, DrumPart.Kick, DrumPart.HiHat)
                           .Add(NoteType.Quarter, DrumPart.HiHat)
                           .AddGroup(NoteType.Quarter, DrumPart.Snare, DrumPart.HiHat)
                           .Add(NoteType.Quarter, DrumPart.HiHat)
                           .Repeat(11);

                var noteBuilder = new NoteEventBuilder(scale: Scale, bpm: BPM, beatNote: NoteType.Quarter)
                    .AddQuarters(1, 1, 5, 5)
                    .AddEighths(6, 6, 6, 6)
                    .AddHalves(5)
                    .AddQuarters(4, 4, 3, 3, 2, 2)
                    .AddHalves(1)
                    .AddQuarters(5).AddEighths(5, 5).AddQuarters(4, 4)
                    .AddQuarters(3).AddEighths(3, 3).AddHalves(2)
                    .AddQuarters(5).AddEighths(5, 5, 4, 4, 4, 4)
                    .AddQuarters(3).AddEighths(3, 3).AddHalves(2)
                    .AddQuarters(1, 1, 5, 5)
                    .AddEighths(6, 6, 6, 6)
                    .AddHalves(5)
                    .AddQuarters(4, 4, 3, 3, 2, 2)
                    .AddHalves(1)
                    .AddWholes(0);

                var noteBuilder2 = new NoteEventBuilder(scale: Scale, bpm: BPM, beatNote: NoteType.Quarter)
                    .SetOctave(1)
                    .AddQuarters(8, 8, 5, 5, 6, 6, 3, 3, 4, 4, 8, 8, 5, 5, 5, 5)
                    .AddQuarters(8, 8, 8, 8, 6, 6, 6, 6, 3, 3, 3, 3, 4, 4, 4, 4)
                    .AddQuarters(8, 8, 5, 5, 6, 6, 3, 3, 4, 4, 5, 5, 8, 8, 8, 8);


                yield return new Player(new TestInstrument(), 0, noteBuilder);
                yield return new Player(new SawInstrument(), 2, noteBuilder2);
                yield return new Player(new TestDrumKit(), 1, drumBuilder);
            }
        }

    }
}
