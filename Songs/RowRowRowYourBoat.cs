using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class RowRowRowYourBoat : Song
    {
        protected override IEnumerable<Player> Players
        {
            get
            {

                var melody = new NoteEventBuilder(scale: Scale, bpm: BPM, beatNote: NoteType.Quarter);

                melody.Add("1h. 1h. 1h 2q 3h.")
                      .Add("3h 2q 3h 4q 5h. 0h.")
                      .Add("8q 8q 8q 5q 5q 5q 3q 3q 3q 1q 1q 1q")
                      .Add("5h 4q 3h 2q 1h. 0h.")
                      .Repeat(1);

                var noteBuilder = new NoteEventBuilder(scale: Scale, bpm: BPM, beatNote: NoteType.Quarter);
                noteBuilder.AddRounds(4, 12, melody,
                    (b, part) =>
                    {
                        // octave 4
                        // octave 3, up a fifth
                        // octave 3
                        // octave 2

                        var scale = Scale;
                        if (part <= 1)
                            scale = scale.ChangeOctave(Scale.Root.Octave - 1);
                        else
                            scale = scale.ChangeOctave(Scale.Root.Octave - 2);

                        if (part == 0)
                            scale = scale.ChangeKey(scale.GetNote(5).Note);

                        return b.SetScale(scale);
                    });

                //    (b, part) => b.SetOctave(b.Octave - 1));

                yield return new Player(new SquareInstrument(), 0, noteBuilder);
            }
        }

        protected override int DefaultBPM => 240;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
             new NoteInfo(MusicNote.C, 4, 1.0));
    }
}
