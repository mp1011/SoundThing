﻿using SoundThing.Models;
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
        PhrygianScale,
        PhrygianDominantScale,
        ChromaticScale,
        LydianMode
    }

    abstract class Scale
    {
        protected Scale(NoteInfo root)
        {
            Root = root;
            _steps = CreateSteps();
        }

        public NoteInfo Root { get; }

        public bool IsDiatonic => _steps.Length == 7;

        protected abstract ScaleStep[] CreateSteps();

        private ScaleStep[] _steps;

        public NoteInfo[] GetChord(int number)
        {
            return new NoteInfo[]
            {
                GetNote(number),
                GetNote(number + 2),
                GetNote(number + 4)
            };
        }

        public NoteInfo GetNote(int number)
        {
            if (number <= 0)
                return new NoteInfo(MusicNote.Rest, 0, 0.0);

            var note = Root;
            for (int i = 1; i < number; i++)
            {
                note = note.Step(_steps[(i - 1) % _steps.Length]);
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
            return Construct(scaleType, root);
        }

        private static Scale Construct(Type scaleType, NoteInfo root)
        {
            return Activator.CreateInstance(scaleType, root) as Scale;
        }

        public Scale ChangeOctave(int octave)
        {
            var newRoot = new NoteInfo(Root.Note, octave, Root.VolumePercent);
            return Construct(GetType(), newRoot);
        }

        public Scale ChangeKey(MusicNote note)
        {
            var newRoot = new NoteInfo(note, Root.Octave, Root.VolumePercent);
            return Construct(GetType(), newRoot);
        }

        public Scale ChangeScaleType(ScaleType type)
        {
            return Create(type, Root);
        }

        public Scale Sharp(int index)
        {
            var newScale = Construct(GetType(), Root);
            newScale._steps[index - 1] = _steps[index - 1] + 1;
            newScale._steps[index] = _steps[index] - 1;

            return newScale;
        }

        public Scale Flat(int index)
        {
            var newScale = Construct(GetType(), Root);
            newScale._steps[index - 1] = _steps[index - 1] - 1;
            newScale._steps[index] = _steps[index] + 1;

            return newScale;
        }

        public override string ToString()
            => $"{Root} {GetType().Name.Replace("Scale", "").Replace("Mode", "")}";
    }

    class MajorScale : Scale
    {
        public MajorScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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
    class PhrygianScale : Scale
    {
        public PhrygianScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
        {
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Whole,
            ScaleStep.Half,
            ScaleStep.Whole,
            ScaleStep.Whole
        };
    }

    class PhrygianDominantScale : Scale
    {
        public PhrygianDominantScale(NoteInfo root) : base(root)
        {
        }

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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

        protected override ScaleStep[] CreateSteps() => new ScaleStep[]
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
