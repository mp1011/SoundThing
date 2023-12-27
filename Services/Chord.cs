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
                    if(type != typeof(UnknownChord))
                        yield return type;
                }
            }
        }

        static Chord()
        {
            _chordTypes = FindChordTypes().ToArray();
        }

        public static IEnumerable<Chord> CreateChordTypes(NoteInfo root)
        {
            return _chordTypes.Select(p => Create(p, root));
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

            return new UnknownChord(root, intervals);

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

        public IEnumerable<int> GetNoteIndices(Scale scale)
            => Notes.Select(p => scale.IndexOf(p));

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

    class MajorSeventhChord : Chord
    {
        public MajorSeventhChord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh };
    }

    class MinorSeventhChord : Chord
    {
        public MinorSeventhChord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MinorThird, Interval.PerfectFifth, Interval.MajorSeventh };
    }

    class DominantSeventhChord : Chord
    {
        public DominantSeventhChord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh };
    }

    class Sus2Chord : Chord
    {
        public Sus2Chord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorSecond, Interval.PerfectFifth };
    }

    class Sus4Chord : Chord
    {
        public Sus4Chord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.PerfectFourth, Interval.PerfectFifth };
    }

    class AugmentedChord : Chord
    {
        public AugmentedChord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorThird, Interval.AugmentedFifth };
    }

    class MajorNinthChord : Chord
    {
        public MajorNinthChord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh,
            Interval.MajorNinth};
    }

    class MinorNinthChord : Chord
    {
        public MinorNinthChord(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh,
            Interval.MajorNinth};
    }

    class DominantNinth : Chord
    {
        public DominantNinth(NoteInfo root) : base(root)
        {
        }

        protected override Interval[] Intervals => new Interval[] { Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh,
            Interval.MajorNinth};
    }

    class UnknownChord : Chord
    {
        public UnknownChord(NoteInfo root, Interval[] intervals) : base(root)
        {
            Intervals = intervals;
        }

        protected override Interval[] Intervals { get; }

        public override string ToString()
        {
            return "Unknown Chord";
        }
    }
}
