namespace SoundThing.Models
{
    class Parameter
    {
        private double _value;

        public Parameter(string name, double min, double max, double value)
        {
            Name = name;
            Min = min;
            Max = max;
            Value = value;
        }

        public double Min { get; }
        public double Max { get; }
        public string Name { get; }
        public double Value
        {
            get => _value;
            set
            {
                if (value < Min)
                    _value = Min;
                else if (value > Max)
                    _value = Max;
                else
                    _value = value;
            }
        }

        public static implicit operator double(Parameter p) => p.Value;
        public static implicit operator float(Parameter p) => (float)p.Value;
    }
}
