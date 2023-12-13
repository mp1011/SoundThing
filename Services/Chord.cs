using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services
{
    abstract class Chord
    {
        private static Type[] _chordTypes;

        private static IEnumerable<Type> FindChordTypes()
        {
            foreach (var type in typeof(Chord).Assembly.GetTypes())
            {
                if(typeof(Chord).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    yield return type;
                }
            }
        }

        static Chord()
        {
            _chordTypes = FindChordTypes().ToArray();
        }

        public static Chord Create(Type chordType, NoteInfo root)
        {
            return (Chord)Activator.CreateInstance(chordType, root);
        }

        public static Chord FromNotes(IEnumerable<NoteInfo> notes)
        {
            var root = notes.First();
            var intervals = notes
                .Skip(1)
                .Select(p => root.IntervalTo(p))
                .ToArray();

            foreach(var maybeChordType in _chordTypes)
            {
                var maybeChord = Create(maybeChordType, root);
                if (maybeChord.Intervals.Length != intervals.Length)
                    continue;

                bool isMatch = true;
                for(int i = 0; i < intervals.Length; i++)
                {
                    if (maybeChord.Intervals[i] != intervals[i])
                    {
                        isMatch = false;
                        continue;
                    }
                }
                if (!isMatch)
                    continue;

                return maybeChord;
            }

            throw new Exception($"Unable to find chord for {string.Join(" ", notes.Select(p => p.ToString()).ToArray())}");
        }


        private NoteInfo _root;

        protected Chord(NoteInfo root)
        {
            _root = root;
        }

        protected abstract Interval[] Intervals { get; }

        public IEnumerable<NoteInfo> Notes
        {
            get
            {
                yield return _root;
                foreach(var interval in Intervals)
                    yield return _root.Step(interval);
            }
        }

        public IEnumerable<int> NoteIndices
        {
            get
            {
                int index = 1;
                yield return index;
                foreach (var interval in Intervals)
                    yield return index += (int)interval;
            }
        }

        public override string ToString()
         => $"{_root} {GetType().Name.Replace("Chord", "")}";

    }

    class MajorChord : Chord
    {
        public MajorChord(NoteInfo root) : base(root)
        {
        }

        public MajorChord(MusicNote note, int octave, double volumePercent) : base(new NoteInfo(note, octave, volumePercent))
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorThird, Interval.PerfectFifth };
    }

    class MinorChord : Chord
    {
        public MinorChord(NoteInfo root) : base(root)
        {
        }

        public MinorChord(MusicNote note, int octave) : base(new NoteInfo(note, octave, 1.0))
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MinorThird, Interval.PerfectFifth };
    }

    class DiminishedChord : Chord
    {
        public DiminishedChord(NoteInfo root) : base(root)
        {
        }

        public DiminishedChord(MusicNote note, int octave) : base(new NoteInfo(note, octave, 1.0))
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MinorThird, Interval.DiminishedFifth };
    }
}
