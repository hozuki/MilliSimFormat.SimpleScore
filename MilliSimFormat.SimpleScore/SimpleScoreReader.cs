using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using MilliSimFormat.SimpleScore.Internal;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore {
    public sealed class SimpleScoreReader : IScoreReader {

        internal SimpleScoreReader() {
        }

        public void Dispose() {
        }

        public SourceScore ReadSourceScore(Stream stream, string fileName, ReadSourceOptions sourceOptions) {
            var lines = ReadLines(stream);

            var c = 0;

            var offsetTimeSpan = TimeSpan.Parse(lines[c]);
            ++c;

            var leadTimeSpan = TimeSpan.Parse(lines[c]);
            ++c;

            var trackCount = Convert.ToInt32(lines[c]);
            ++c;

            var beatsPerMeasure = Convert.ToInt32(lines[c]);
            ++c;

            // splitter
            ++c;

            var notes = new List<SourceNote>();
            while (!lines[c].StartsWith("---")) {
                var line = lines[c];

                SourceNote[] sourceNotes;
                if (TapPattern.IsMatch(line)) {
                    var tap = Tap.FromString(line);

                    var sourceNote = new SourceNote {
                        Type = NoteType.Tap
                    };
                    FillHeader(sourceNote, tap.Header);
                    sourceNote.Size = StrToSize(tap.Body.Size);

                    sourceNotes = new[] { sourceNote };
                } else if (HoldPairPattern.IsMatch(line)) {
                    var holds = Hold.CreateHolds(line);

                    var sourceNote = new SourceNote {
                        Type = NoteType.Hold
                    };
                    FillHeader(sourceNote, holds[0].Header);
                    sourceNote.Size = StrToSize(holds[0].Body.Size);

                    var holdEnd = new SourceNote {
                        Type = NoteType.Hold
                    };
                    FillHeader(holdEnd, holds[1].Header);
                    holdEnd.Size = sourceNote.Size;
                    holdEnd.FlickDirection = StrToDirection(holds[1].Body.Direction);

                    sourceNote.FollowingNotes = new[] { holdEnd };

                    sourceNotes = new[] { sourceNote };
                } else if (FlickPattern.IsMatch(line)) {
                    var flick = Flick.FromString(line);

                    var sourceNote = new SourceNote {
                        Type = NoteType.Flick
                    };
                    FillHeader(sourceNote, flick.Header);
                    sourceNote.Size = StrToSize(flick.Body.Size);
                    sourceNote.FlickDirection = StrToDirection(flick.Body.Direction);

                    sourceNotes = new[] { sourceNote };
                } else if (SlideSeriesPattern.IsMatch(line)) {
                    var slides = Slide.CreateSlides(line);

                    var sourceNote = new SourceNote {
                        Type = NoteType.Slide
                    };
                    FillHeader(sourceNote, slides[0].Header);

                    var following = new List<SourceNote>();
                    for (var i = 1; i < slides.Count; ++i) {
                        var nodeInSeries = new SourceNote {
                            Type = NoteType.Slide
                        };
                        FillHeader(nodeInSeries, slides[i].Header);

                        if (i == slides.Count - 1) {
                            nodeInSeries.FlickDirection = StrToDirection(slides[i].Body.Direction);
                        }

                        following.Add(nodeInSeries);
                    }

                    sourceNote.FollowingNotes = following.ToArray();

                    sourceNotes = new[] { sourceNote };
                } else if (SpecialPattern.IsMatch(line)) {
                    var special = Special.FromString(line);

                    var sourceNote = new SourceNote {
                        Type = NoteType.Special
                    };
                    FillHeader(sourceNote, special.Header);

                    sourceNotes = new[] { sourceNote };
                } else {
                    throw new FormatException("Error in simple format.");
                }

                notes.AddRange(sourceNotes);

                // next line
                ++c;
            }

            // Sort the added notes.
            notes.Sort((n1, n2) => n1.Ticks.CompareTo(n2.Ticks));

            // splitter
            ++c;

            var conductors = new List<Conductor>();
            for (; c < lines.Count; ++c) {
                var ss = lines[c].Split(':');
                var measureIndex = Convert.ToInt32(ss[0]);
                var bpm = Convert.ToDouble(ss[1]);
                var conductor = new Conductor {
                    Measure = measureIndex - 1,
                    Tempo = bpm,
                    Ticks = (measureIndex - 1) * beatsPerMeasure * NoteBase.TicksPerBeat,
                    SignatureNumerator = beatsPerMeasure,
                    SignatureDenominator = beatsPerMeasure
                };
                conductors.Add(conductor);
            }

            conductors.Sort((n1, n2) => n1.Ticks.CompareTo(n2.Ticks));

            var score = new SourceScore();
            score.Conductors = conductors.ToArray();
            score.Notes = notes.ToArray();
            score.TrackCount = trackCount;
            score.MusicOffset = offsetTimeSpan.TotalSeconds;
            return score;

            void FillHeader(SourceNote note, NoteHeader header) {
                var fraction = (float)(header.Nominator - 1) / header.Denominator;
                note.Beat = (int)(beatsPerMeasure * fraction);
                note.StartX = header.Start - 1;
                note.EndX = header.End - 1;
                note.Speed = header.Speed;
                note.LeadTime = leadTimeSpan.TotalSeconds;
                note.Measure = header.Measure - 1;
                note.Ticks = 60 * (long)(beatsPerMeasure * ((header.Measure - 1) + fraction) * NoteBase.TicksPerBeat);
                note.TrackIndex = (int)note.StartX;

                if (note.TrackIndex < 0 || note.TrackIndex >= trackCount) {
                    Debug.Print("Warning: Invalid track index \"{0}\", changing into range [0, {1}].", note.TrackIndex, trackCount - 1);

                    if (note.TrackIndex < 0) {
                        note.TrackIndex = 0;
                    } else if (note.TrackIndex >= trackCount) {
                        note.TrackIndex = trackCount - 1;
                    }
                }
            }

            NoteSize StrToSize(string str) {
                if (string.IsNullOrEmpty(str)) {
                    return NoteSize.Small;
                } else {
                    switch (str) {
                        case "small":
                            return NoteSize.Small;
                        case "large":
                            return NoteSize.Large;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(str), str, null);
                    }
                }
            }

            FlickDirection StrToDirection(string str) {
                if (string.IsNullOrEmpty(str)) {
                    return FlickDirection.None;
                } else {
                    switch (str) {
                        case "left":
                            return FlickDirection.Left;
                        case "right":
                            return FlickDirection.Right;
                        case "up":
                            return FlickDirection.Up;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(str), str, null);
                    }
                }
            }
        }

        public RuntimeScore ReadCompiledScore(Stream stream, string fileName, ReadSourceOptions sourceOptions, ScoreCompileOptions compileOptions) {
            var sourceScore = ReadSourceScore(stream, fileName, sourceOptions);
            RuntimeScore runtimeScore;
            using (var compiler = new SimpleScoreCompiler()) {
                runtimeScore = compiler.Compile(sourceScore, compileOptions);
            }
            return runtimeScore;
        }

        private static IReadOnlyList<string> ReadLines(Stream stream) {
            var lines = new List<string>();

            using (var reader = new StreamReader(stream)) {
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) {
                        continue;
                    }

                    if (!line.StartsWith("#")) {
                        lines.Add(line.Trim());
                    }
                }
            }

            return lines.ToArray();
        }

        private const string FloatingNumberPattern = @"\d+|\d+\.|\d+\.\d+|\.\d+";
        private const string NoteHeaderStr = @"\[(?<measure>\d+),(?<nom>\d+)/(?<denom>\d+),(?<start>" + FloatingNumberPattern + @")(?:,(?<end>" + FloatingNumberPattern + @")(?:,(?<speed>" + FloatingNumberPattern + @"))?)?\]";
        private const string OptionalSizeStr = @"(?:\((?:(?<size>small|large)!)?\))?";
        private const string DirOptionalSizeStr = @"\((?<direction>left|right|up)!(?:(?<size>small|large)!)?\)";
        private const string OptionalSizeDirStr = @"(?:\((?:(?<direction>left|right|up)!)?(?:(?<size>small|large)!)?\))?";
        private const string TapStr = "^t:" + NoteHeaderStr + OptionalSizeStr + "$";
        private const string HoldStr = "^" + HoldPartStr + "$";
        private const string HoldPartStr = NoteHeaderStr + OptionalSizeDirStr;
        private const string HoldPairStr = "^h:" + HoldPartStr + SeriesSeparatorForRegex + HoldPartStr + "$";
        private const string FlickStr = "^f:" + NoteHeaderStr + DirOptionalSizeStr + "$";
        private const string SlideStr = "^" + SlidePartStr + "$";
        private const string SlidePartStr = NoteHeaderStr + OptionalSizeDirStr;
        private const string SlideSeriesStr = "^s:" + SlidePartStr + @"(?:" + SeriesSeparatorForRegex + SlidePartStr + ")+$";
        private const string SpecialStr = "^x:" + NoteHeaderStr + @"(?:\(\))?$";

        internal const string SeriesSeparator = "->";
        private const string SeriesSeparatorForRegex = @"\-\>";

        private const RegexOptions PatternOptions = RegexOptions.CultureInvariant;

        internal static readonly Regex NoteHeaderPattern = new Regex(NoteHeaderStr, PatternOptions);
        internal static readonly Regex TapPattern = new Regex(TapStr, PatternOptions);
        internal static readonly Regex HoldPattern = new Regex(HoldStr, PatternOptions);
        internal static readonly Regex HoldPairPattern = new Regex(HoldPairStr, PatternOptions);
        internal static readonly Regex FlickPattern = new Regex(FlickStr, PatternOptions);
        internal static readonly Regex SlidePattern = new Regex(SlideStr, PatternOptions);
        internal static readonly Regex SlideSeriesPattern = new Regex(SlideSeriesStr, PatternOptions);
        internal static readonly Regex SpecialPattern = new Regex(SpecialStr, PatternOptions);

    }
}
