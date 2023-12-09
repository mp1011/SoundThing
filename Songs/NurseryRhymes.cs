using Microsoft.Xna.Framework.Audio;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteEventBuilders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoundThing.Songs
{
    class NurseryRhymes
    {
        public static SoundEffectInstance[] BaaBaaBlackSheep()
        {
            var scale = Scale.Create(ScaleType.MajorScale,
              new NoteInfo(MusicNote.C, 2, 1.0));

            var drumBuilder = new NoteEventBuilder(scale: scale, bpm: 120, beatNote: NoteType.Quarter);

            drumBuilder.AddGroup(NoteType.Quarter, DrumPart.Kick, DrumPart.HiHat)
                       .Add(NoteType.Quarter, DrumPart.HiHat)
                       .AddGroup(NoteType.Quarter, DrumPart.Snare, DrumPart.HiHat)
                       .Add(NoteType.Quarter, DrumPart.HiHat)
                       .Repeat(11);

            var noteBuilder = new NoteEventBuilder(scale: scale, bpm: 120, beatNote: NoteType.Quarter)
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

            var noteBuilder2 = new NoteEventBuilder(scale: scale, bpm: 120, beatNote: NoteType.Quarter)
                .SetOctave(1)
                .AddQuarters(8, 8, 5, 5, 6, 6, 3, 3, 4, 4, 8, 8, 5, 5, 5, 5)
                .AddQuarters(8, 8, 8, 8, 6, 6, 6, 6, 3, 3, 3, 3, 4, 4, 4, 4)
                .AddQuarters(8, 8, 5, 5, 6, 6, 3, 3, 4, 4, 5, 5, 8, 8, 8, 8);


            var player1 = new Player(new TestInstrument(), 0, noteBuilder);
            var player2 = new Player(new SawInstrument(), 2, noteBuilder2);
            var player3 = new Player(new TestDrumKit(), 1, drumBuilder);
            var band = new Band(player1, player2, player3);
            return band.GenerateSounds();
        }

        public static SoundEffectInstance[] ItsyBitsySpider()
        {
            var scale = Scale.Create(ScaleType.MajorScale,
              new NoteInfo(MusicNote.C, 3, 1.0))
                .Flat(6);

            var noteBuilder = new NoteEventBuilder(80, NoteType.Quarter, scale);

            noteBuilder
                .Add("1e 4q 3e 4q 5e 6q. 6q 6e 5q 4e 5q 6e 4q. 0q.")
                .Add("6q. 6q 7e 8q. 8q. 7q 6e 7q 8e 6q. 0q.")
                .Add("4q. 4q 5e 6q. 6q. 5q 4e 5q 6e 4q. 1q 1e")
                .Add("4q 3e 4q 5e 6q. 6q 6e 5q 4e 5q 6e 4h. 0w");

            var player1 = new Player(new PluckyInstrument(), 0, noteBuilder);
            var band = new Band(player1);
            return band.GenerateSounds();
        }
    }
}