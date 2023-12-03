namespace SoundThing.Models
{
    public struct Envelope
    {
        public Envelope(double sustainVolumePercent, double attack, double decay, double release)
        {
            SustainVolumePercent = sustainVolumePercent;
            Attack = attack;
            Decay = decay;
            Release = release;
        }

        public double SustainVolumePercent { get; }
        public double Attack { get; }
        public double Decay { get; }
        public double Release { get; }

        public int AttackSamples => (int)(Attack * Constants.SamplesPerSecond);
        public int DecaySamples => (int)(Decay * Constants.SamplesPerSecond);
        public int ReleaseSamples => (int)(Release * Constants.SamplesPerSecond);

    }

}
