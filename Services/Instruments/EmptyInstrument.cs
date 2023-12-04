using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class EmptyInstrument : Instrument
    {
        protected override Func<int, NoteEvent, short> NoteGenerator => (i,s) => 0;
    }
}
