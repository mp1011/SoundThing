using SoundThing.Models;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Extensions
{
    static class NoteEventExtensions
    {
        public static IEnumerable<NoteEvent> ToNoteEvents(this IEnumerable<NoteInfo> notes, double timePerNote, double startTime)
        {
            return notes.Select((noteInfo,index) =>
                 new NoteEvent(new PlayedNoteInfo(noteInfo, timePerNote), startTime + (timePerNote * index)));               
        }
    }
}
