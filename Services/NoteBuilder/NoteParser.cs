using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundThing.Services.NoteBuilder
{
    static class NoteParser
    {
        public static NoteEventBuilder AddEvents(NoteEventBuilder builder, EventAction action, string notes)
        {
            foreach (var token in notes.Tokenize())
                ParseToken(builder, action, token);   

            return builder;
        }

        private static void ParseToken(NoteEventBuilder builder, EventAction action, string token)
        {
            List<ScaleNoteIndex> notes = new List<ScaleNoteIndex>();
       
            char previousChar = ' ';
            NoteType noteType = NoteType.Quarter;
            bool negateNext = false;
            
            foreach(char chr in token)
            {
                if (chr == '-')
                    negateNext = true;

                if (char.IsNumber(chr))
                {
                    var number = int.Parse(chr.ToString());
                    if(negateNext)
                    {
                        number = -number;
                        negateNext = false;
                    }
                    notes.Add(new ScaleNoteIndex(number));
                }

                if (char.IsNumber(previousChar))
                {
                    if (chr == '#')
                        notes[notes.Count - 1] = notes[notes.Count - 1].Sharp();
                    else if (chr == 'b')
                        notes[notes.Count - 1] = notes[notes.Count - 1].Flat();
                }

                NoteType? maybeNoteType = chr.TryParseNoteType() ?? $"{previousChar}{chr}".TryParseNoteType();
                if(maybeNoteType.HasValue)                
                    noteType = maybeNoteType.Value;

                previousChar = chr;
            }

            builder.AddEventGroup(noteType, action, notes);
        }

        private static IEnumerable<string> Tokenize(this string notes)
        {
            bool insideParens = false;
            string token = null;
            char expectedParens = '(';
            var tokenBuilder = new StringBuilder();
            string parensApplyToken = null;
            int index = 0;

            foreach (char c in notes)
            {
                if (c == '(')
                {
                    parensApplyToken = tokenBuilder.ToString();
                    tokenBuilder.Clear();
                }
                else if (c == ' ' || c == ')')
                {
                    token = tokenBuilder.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        if (!insideParens)
                            yield return token;
                        else
                            yield return $"{parensApplyToken}{token}";
                    }
                    tokenBuilder.Clear();
                }
                else
                {
                    tokenBuilder.Append(c);
                }

                if (!insideParens)
                    expectedParens = '(';
                else
                    expectedParens = ')';

                if (c == expectedParens)
                    insideParens = !insideParens;
                else if (c == '(' || c == ')')
                    throw new Exception($"Unexpected {c} at {index} of {notes}");

                index++;
            }

            token = tokenBuilder.ToString();
            if (!string.IsNullOrEmpty(token))
                yield return token;
        }
    }
}
