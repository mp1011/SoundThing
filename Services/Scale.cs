using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services
{
    enum ScaleStep
    {
        Half=1,
        Whole=2,
        AugmentedSecond=3
    }

    enum ScaleType
    {
        MajorScale,
        MelodicMinorScale,
        HarmonicMinorScale,
        NaturalMinorScale,
        PhrygianDominantScale,
        ChromaticScale,
        LydianMode
    }

    abstract class Scale
    {
        protected Scale(NoteInfo root)
        {
            Root = root;
        }

        public NoteInfo Root { get; }

        protected abstract ScaleStep[] Steps { get; }
        public NoteInfo GetNote(int number)
        {
            var note = Root;
            for (int i = 1; i < number; i++)
            {
                note = note.Step(Steps[(i - 1) % Steps.Length]);
            }

            return note;
        }

        public IEnumerable<NoteInfo> GetNotes(params int[] numbers)
        {
            return numbers.Select(GetNote);
        }

        public static Scale Create(ScaleType type, NoteInfo root)
        {
            Type scaleType = typeof(Scale).Assembly.GetType($"SoundThing.Services.{type}");
            return Activator.CreateInstance(scaleType, root) as Scale;
        }
    }

    class MajorScale : Scale
    {
        public MajorScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half
        };
    }

    class NaturalMinorScale : Scale
    {
        public NaturalMinorScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole
        };
    }
    class HarmonicMinorScale : Scale
    {
        public HarmonicMinorScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.AugmentedSecond,
            ScaleStep.Half
        };
    }

    class MelodicMinorScale : Scale
    {
        public MelodicMinorScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half
        };
    }

    class LydianMode : Scale
    {
        public LydianMode(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half
        };
    }

    class PhrygianDominantScale : Scale
    {
        public PhrygianDominantScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Half,
            ScaleStep.AugmentedSecond,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole
        };
    }

    class ChromaticScale : Scale
    {
        public ChromaticScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] Steps => new ScaleStep[]
        {
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
            ScaleStep.Half,
        };
    }
}
