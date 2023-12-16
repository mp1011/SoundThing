using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services.NoteBuilder
{
    enum EventAction
    {
        PlayScaleNote,
        PlayDrumPart,
        ChangeInterval
    }

    abstract class EventBlock
    {
        private static Dictionary<EventAction, Type> _types;
        static EventBlock()
        {
            var types = typeof(EventBlock).Assembly.GetTypes()
                .Where(t => typeof(EventBlock).IsAssignableFrom(t))
                .ToArray();

            _types = Enum.GetValues(typeof(EventAction))
                         .OfType<EventAction>()
                         .ToDictionary(k => k,
                                       v => types.First(p => p.Name == v.ToString()));
        }

        public static EventBlock Create(EventAction action, double start, double duration, int argument, Scale scale)
            => Create(_types[action], start, duration, argument, scale);

        protected static EventBlock Create(Type type, double start, double duration, int argument, Scale scale)
        {
            if (duration <= 0)
                return null;

            return (EventBlock)Activator.CreateInstance(type, start, duration, argument, scale);
        }

        protected EventBlock(double start, double duration, int argument, Scale scale)
        {
            Start = start;
            Duration = duration;
            Argument = argument;
            Scale = scale;
        }

        public double Start { get; }
        public double End => Start + Duration;
        public double Duration { get; }
        public int Argument { get; }
        public Scale Scale { get; }

        public bool Overlaps(EventBlock other)
            => other.End > Start && other.Start < End;

        public EventBlock[] SpliceAt(double time)
        {
            if (time < Start || time > End)
                return new EventBlock[] { null, null };

            return new EventBlock[]
            {
                ChangeTimes(Start, time - Start),
                ChangeTimes(time, End - time)
            };
         }

        public IEnumerable<EventBlock> SpliceAt(params double[] times)
        {
            var spliceTarget = this;
            EventBlock[] splice = null;
            foreach(var time in times
                                .Where(p=> p >= Start && p <= End)
                                .OrderBy(p=>p)
                                .Distinct())
            {
                splice = spliceTarget.SpliceAt(time);
                if (splice[0] != null)
                    yield return splice[0];

                spliceTarget = splice[1];
                if (spliceTarget == null)
                    break;
            }

            if (splice != null && splice[1] != null)
                yield return splice[1];
        }

        public EventBlock ChangeStartTime(double newStart)
            => Create(GetType(), newStart, Duration, Argument, Scale);

        public EventBlock ChangeTimes(double newStart, double newEnd)
            => Create(GetType(), newStart, newEnd, Argument, Scale);

        public EventBlock ChangeScale(Scale newScale)
           => Create(GetType(), Start, Duration, Argument, newScale);
        public EventBlock AddStartTime(double amount)
            => Create(GetType(), Start + amount, Duration, Argument, Scale);
        public EventBlock Copy()
           => Create(GetType(), Start, Duration, Argument, Scale);

        public override string ToString() =>
            $"{GetType().Name} {Start} ({Duration})";
    }

    abstract class GeneratorBlock : EventBlock
    {
        protected GeneratorBlock(double start, double duration, int argument, Scale scale) : base(start, duration, argument, scale)
        {
        }

        public abstract NoteEvent CreateEvent();
    }

    abstract class ModifierBlock : EventBlock
    {
        protected ModifierBlock(double start, double duration, int argument, Scale scale) : base(start, duration, argument, scale)
        {
        }
        public abstract GeneratorBlock Modify(GeneratorBlock targetBlock);
    }

    class PlayScaleNote : GeneratorBlock
    {
        public PlayScaleNote(double start, double duration, int argument, Scale scale) : base(start, duration, argument, scale)
        {
        }

        public override NoteEvent CreateEvent()
            => new NoteEvent(new PlayedNoteInfo(
                                noteInfo: Scale.GetNote(Argument),
                                duration: Duration),
                            startTime: Start);
    }

    class PlayDrumPart : GeneratorBlock
    {
        public PlayDrumPart(double start, double duration, int argument, Scale scale) : base(start, duration, argument, scale)
        {
        }

        private NoteInfo ToNoteInfo(DrumPart p) 
            => new NoteInfo((MusicNote)p, Scale.Root.Octave, Scale.Root.VolumePercent);


        public override NoteEvent CreateEvent()
            => new NoteEvent(new PlayedNoteInfo(
                                noteInfo: ToNoteInfo((DrumPart)Argument),
                                duration: Duration),
                            startTime: Start);
    }

    class ChangeInterval : ModifierBlock
    {
        public ChangeInterval(double start, double duration, int argument, Scale scale) 
            : base(start, duration, argument, scale)
        {
        }

        public override GeneratorBlock Modify(GeneratorBlock targetBlock)
        {
            if(Argument == 0)
                return Create(targetBlock.GetType(),
                   targetBlock.Start,
                   targetBlock.Duration,
                   0,
                   targetBlock.Scale) as GeneratorBlock;

            if (Argument == 1)
                return targetBlock;

            return Create(targetBlock.GetType(), 
                targetBlock.Start, 
                targetBlock.Duration, 
                targetBlock.Argument + (Argument - 1), 
                targetBlock.Scale) as GeneratorBlock;
        }
    }
}
