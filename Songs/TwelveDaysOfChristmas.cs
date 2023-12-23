using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteBuilder;
using System;
using System.Collections.Generic;

namespace SoundThing.Songs
{
    class TwelveDaysOfChristmas : Song
    {
        protected override IEnumerable<Player> CreatePlayers()
        {
            var scale = Scale.Flat(6);

            var builder = new NoteEventBuilder(BPM, NoteType.Quarter, scale);

            Func<int, string> getNthDayPattern = (int day) =>
            {
                if (day == 2 || day == 7)
                    return "e(1 1)";
                else if (day == 11)
                    return "s(1 1) e1";
                else
                    return "q1";
            };

            Func<int, string> getNthDayCountPattern = (int day) =>
            {
                if (day == 7)
                    return "e(8 8)";
                else if (day == 11)
                    return "s(8 8) e7";
                else
                    return "q8";
            };

            Func<int, string> getBeginPattern = (int day) =>
            {
                string endCap = (day == 1) ? "q.6 e7" : "h6";
                return $"e(1 1) {getNthDayPattern(day)} e(4 4) q4 e(3 4 5 6 7 5) {endCap}";
            };

            Func<int, int, string> getGiftPattern = (int day, int round) =>
            {
                var standard = "q8 e(5 6) q7";
                  
                return day switch
                {
                    1 => "q8 e(9 7 6 4) q5 h.4",
                    2 => (round >= 5) ? "e(5 4 3 2) q1 e(6 7)" : $"{standard} e(6 7)",
                    3 => (round >= 5) ? "q(7 2 4)" : standard,
                    4 => (round >= 5) ? "e(8 7 6 5) 4q" : standard,
                    5 => "h8 q(9 7#) w8",
                    _ => $"{getNthDayCountPattern(day)} e(5 6 7 5)"
                };                   
            };

            for(int day = 1; day <= 12; day++)
            {
                builder.Add(getBeginPattern(day));

                for(int day2 = day; day2 >= 1; day2--)
                    builder.Add(getGiftPattern(day2, day));
            }

            builder.AddChord(NoteType.Whole, 1, 3, 5, 8);
                               
            yield return new Player(new PluckyInstrument(), 0, builder);
        }

        protected override int DefaultBPM => 90;

        protected override Scale DefaultScale => Scale.Create(ScaleType.MajorScale,
             new NoteInfo(MusicNote.F, 2, 1.0));

        public override string ToString() => "12 Days of XMas";
    }
}
