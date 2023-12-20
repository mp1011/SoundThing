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
        DorianMode,
        PhyrgianMode,
        LydianMode,
        MixolydianMode,
        AeolianMode,
        LocrianMode
    }

    struct ScaleParameters
    {
        public ScaleParameters(NoteInfo root, int[] sharps=null, int[] flats = null)
        {
            Sharps = sharps ?? Array.Empty<int>();
            Flats = flats ?? Array.Empty<int>();
            Root = root;
        }

        public readonly int[] Sharps { get; }
        public readonly int[] Flats { get; }

        public NoteInfo Root { get; }

        public ScaleParameters ChangeRoot(NoteInfo newRoot) =>
            new ScaleParameters(newRoot, Sharps, Flats);

        public ScaleParameters ChangeSharps(IEnumerable<int> newSharps) =>
            new ScaleParameters(Root, newSharps.ToArray(), Flats);

        public ScaleParameters ChangeFlats(IEnumerable<int> newFlats) =>
            new ScaleParameters(Root, Sharps, newFlats.ToArray());

    }

    abstract class Scale
    {
        protected Scale(ScaleParameters scaleParameters)
        {
            _parameters = scaleParameters;
            Steps = CreateSteps(_parameters.Sharps, _parameters.Flats);
        }

        private readonly ScaleParameters _parameters;

        public NoteInfo Root => _parameters.Root;

        public bool IsDiatonic => Steps.Length == 7;

        private ScaleStep[] CreateSteps(int[] sharps, int[] flats)
        {
            var steps = CreateSteps();
            foreach(var index in sharps)
            {
                steps[index - 1] = steps[index - 1] + 1;
                steps[index] = steps[index] - 1;
            }

            foreach (var index in flats)
            {
                steps[index - 1] = steps[index - 1] - 1;
                steps[index] = steps[index] + 1;
            }

            return steps;
        }
        protected abstract ScaleStep[] CreateSteps();

        public ScaleStep[] Steps { get; }
      
        public NoteInfo[] GetChord(int number)
        {
            return new NoteInfo[]
            {
                GetNote(number),
                GetNote(number + 2),
                GetNote(number + 4)
            };
        }

        public int[] GetChordIndices(int number)
            => new int[] { number, number + 2, number + 4 };

        public NoteInfo GetNote(int number)
        {
            if (number == 0)
                return new NoteInfo(MusicNote.Rest, 0, 0.0);

            var note = Root;


            if (number < 0)
            {
                note = note.ChangeOctave(note.Octave - 1);
                number += (Steps.Length + 1);
            }

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
            => Create(type, 
                      new ScaleParameters(root));
        
        public static Scale Create(ScaleType type, ScaleParameters parameters)
        {
            Type scaleType = typeof(Scale).Assembly.GetType($"SoundThing.Services.{type}");
            return Construct(scaleType, parameters);
        }

        private static Scale Construct(Type scaleType, ScaleParameters parameters)
        {
            return Activator.CreateInstance(scaleType, parameters) as Scale;
        }

        public Scale ChangeOctave(int octave)
        {
            var newRoot = new NoteInfo(Root.Note, octave, Root.VolumePercent);
            return Construct(GetType(), _parameters.ChangeRoot(newRoot));
        }

        public Scale ChangeKey(MusicNote note)
        {
            var newRoot = new NoteInfo(note, Root.Octave, Root.VolumePercent);
            return Construct(GetType(), _parameters.ChangeRoot(newRoot));
        }

        public Scale Copy() => Construct(GetType(), _parameters);

        public Scale ChangeScaleType(ScaleType type)
        {
            return Create(type, _parameters);
        }

        public Scale Sharp(int index)
        {
            var newSharps = _parameters.Sharps.ToList();
            newSharps.Add(index);

            return Construct(GetType(), _parameters.ChangeSharps(newSharps));
        }

        public Scale Flat(int index)
        {
            var newFlats = _parameters.Flats.ToList();
            newFlats.Add(index);

            return Construct(GetType(), _parameters.ChangeFlats(newFlats));
        }

        public override string ToString()
            => $"{Root} {GetType().Name.Replace("Scale", "").Replace("Mode", "")}";
    }



    class MajorScale : Scale
    {
        public MajorScale(ScaleParameters parameters) : base(parameters)
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
        public NaturalMinorScale(ScaleParameters parameters) : base(parameters)
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
        public HarmonicMinorScale(ScaleParameters parameters) : base(parameters)
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
        public MelodicMinorScale(ScaleParameters parameters) : base(parameters)
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

    abstract class Mode : Scale
    {
        protected abstract int Degree { get; }
        protected Mode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }

        protected override ScaleStep[] CreateSteps()
        {
            var basis = new MajorScale(new ScaleParameters(Root)).Steps;

            return basis
                .Skip(Degree - 1)
                .Concat(
                    basis.Take(Degree - 1))
                .ToArray();
        }
    }

    class DorianMode : Mode
    {
        protected override int Degree => 2;

        public DorianMode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }
    }

    class PhyrgianMode : Mode
    {
        protected override int Degree => 3;

        public PhyrgianMode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }
    }

    class LydianMode : Mode
    {
        protected override int Degree => 4;

        public LydianMode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }
    }

    class MixolydianMode : Mode
    {
        protected override int Degree => 5;

        public MixolydianMode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }
    }

    class AeolianMode : Mode
    {
        protected override int Degree => 6;

        public AeolianMode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }
    }

    class LocrianMode : Mode
    {
        protected override int Degree => 7;

        public LocrianMode(ScaleParameters scaleParameters) : base(scaleParameters)
        {
        }
    }

    class PhrygianDominantScale : Scale
    {
        public PhrygianDominantScale(ScaleParameters parameters) : base(parameters)
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
        public ChromaticScale(ScaleParameters parameters) : base(parameters)
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
