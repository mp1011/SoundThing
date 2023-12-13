namespace SoundThing.Services.NoteBuilder
{
    enum EventAction
    {
        PlayScaleNote,
        PlayDrumPart
    }

    struct EventBlock
    {
        public EventBlock(double start, double duration, int argument, Scale scale, EventAction action)
        {
            Start = start;
            Duration = duration;
            Argument = argument;
            Action = action;
            Scale = scale;
        }

        public double Start { get; }
        public double End => Start + Duration;
        public double Duration { get; }
        public int Argument { get; }
        public Scale Scale { get; }
        public EventAction Action { get; }

        public EventBlock ChangeStartTime(double newStart)
            => new EventBlock(newStart, Duration, Argument, Scale, Action);
        public EventBlock ChangeScale(Scale newScale)
           => new EventBlock(Start, Duration, Argument, newScale, Action);

        public EventBlock AddStartTime(double amount)
         => new EventBlock(Start + amount, Duration, Argument, Scale, Action);

    }
}
