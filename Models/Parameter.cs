using System;

namespace SoundThing.Models
{
    interface IParameter
    {
        string Name { get; }
    }

    class Parameter : IParameter
    {
        private double _value;

        public Parameter(string name, double min, double max, double value, string format)
        {
            Name = name;
            Min = min;
            Max = max;
            Value = value;
            Format = format;
        }

        public double Min { get; }
        public double Max { get; }
        public string Name { get; }

        public string Format { get; }

        public double Mod { get; set; } = 1.0;

        public double ModifiedValue => _value * Mod;
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

        public static implicit operator double(Parameter p) => p.ModifiedValue;
        public static implicit operator float(Parameter p) => (float)p.ModifiedValue;
        public static implicit operator int(Parameter p) => (int)Math.Round(p.ModifiedValue);

        public override string ToString() => $"{Name}={Value.ToString(Format)}";
    }

    class Parameter<T> : IParameter
    {
        public T Value { get; set; }
        public string Name { get; }

        public Parameter(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
