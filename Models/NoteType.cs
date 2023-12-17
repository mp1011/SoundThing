using System.ComponentModel;

namespace SoundThing.Models
{
    enum NoteType
    {
        [Description("w")]
        Whole,

        [Description("h")]
        Half,

        [Description("q")]
        Quarter,

        [Description("e")]
        Eighth,

        [Description("s")]
        Sixteenth,

        [Description("w.")]
        DottedWhole,

        [Description("h.")]
        DottedHalf,

        [Description("q.")]
        DottedQuarter,

        [Description("e.")]
        DottedEighth,

        [Description("s.")]
        DottedSixteenth
    }
}
