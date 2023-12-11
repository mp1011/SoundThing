using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteEventBuilders;
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
                    (b, part) => b.SetOctave(b.Octave - 1));

                yield return new Player(new PluckyInstrument(), 0, noteBuilder);
            }
        }

        protected override int BPM => 240;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
             new NoteInfo(MusicNote.C, 5, 1.0));
    }
}
