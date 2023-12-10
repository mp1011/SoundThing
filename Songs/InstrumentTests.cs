using Microsoft.Xna.Framework.Audio;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteEventBuilders;
using System;

namespace SoundThing.Songs
{
    class InstrumentTests
    {
        public static SoundEffectInstance[] LfoTest()
        {
            var scale = Scale.Create(ScaleType.MajorScale,
                new NoteInfo(MusicNote.C, 3, 1.0));


            var noteBuilder = new NoteEventBuilder(80, NoteType.Quarter, scale);

            noteBuilder.AddWholes(1, 3, 5, 6, 8);
            var player = new Player(new LfoTestInstrument(), 0, noteBuilder);
            return Band.CreateSounds(player);
        }

        public static SoundEffectInstance[] PadTest()
        {
            var scale = Scale.Create(ScaleType.MajorScale,
                new NoteInfo(MusicNote.C, 3, 1.0));


            var noteBuilder = new NoteEventBuilder(120, NoteType.Quarter, scale);

            noteBuilder.AddWholes(1, 3, 5)
                       .AddChords(NoteType.Whole, 1, 3, 5);

            var player = new Player(new PadInstrument(), 0, noteBuilder);
            return Band.CreateSounds(player);
        }


        public static SoundEffectInstance[] ArpeggioTest()
        {

            var noteBuilder = new NoteEventBuilder(120,
                 NoteType.Quarter,
                 new PhrygianDominantScale(new NoteInfo(MusicNote.D, 2, 1.0)));

            var builderA = noteBuilder.NewSection()
                       .AddArpeggio(NoteType.Sixteenth, ArpeggioStyle.Rising, 1, 1, 1, 1)
                       .AddArpeggio(NoteType.Sixteenth, ArpeggioStyle.Falling, 3, 3, 3, 3)
                       .AddArpeggio(NoteType.Sixteenth, ArpeggioStyle.Rising, 1, 1, 1, 1)
                       .AddArpeggio(NoteType.Sixteenth, ArpeggioStyle.Falling, 5, 5, 5, 5);

            noteBuilder.AddSection(builderA);
            noteBuilder.AddSection(builderA);

            var builderB = noteBuilder.NewSection();
            builderB.AddArpeggio(NoteType.Eighth, ArpeggioStyle.RiseAndFall, 2, 6, 2, 5);

            noteBuilder.AddSection(builderB)
                       .AddSection(builderA)
                       .AddChords(NoteType.Quarter, 1, 3, 5, 7, 9);

            var player = new Player(new PluckyInstrument(), 0, noteBuilder);

            return Band.CreateSounds(player);
        }

        public static SoundEffectInstance[] AllScalesTest()
        {
            var noteBuilder = new NoteEventBuilder(120, 
                NoteType.Quarter, 
                new MajorScale(new NoteInfo(MusicNote.C, 3, 1.0)));

            foreach (ScaleType scaleType in Enum.GetValues(typeof(ScaleType)))
            {
                var scale = Scale.Create(scaleType,
                                new NoteInfo(MusicNote.C, 3, 1.0));

                if (scale.IsDiatonic)
                {
                    noteBuilder.SetScale(scale);

                    noteBuilder.AddArpeggio(NoteType.Sixteenth, ArpeggioStyle.FallAndRise, 1, 4, 5);
                   
                    var iChord = Chord.FromNotes(scale.GetChord(1));
                    var ivChord = Chord.FromNotes(scale.GetChord(4));
                    var vChord = Chord.FromNotes(scale.GetChord(5));

                    System.Diagnostics.Debug.WriteLine($"I-IV-V for {scale}: {iChord} {ivChord} {vChord}");
                }
            }


            var player = new Player(new PluckyInstrument(), 0, noteBuilder);

            return Band.CreateSounds(player);

        }
    }
}
