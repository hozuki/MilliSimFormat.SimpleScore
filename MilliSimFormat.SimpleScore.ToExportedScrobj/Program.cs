using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj {
    internal static class Program {

        private static int Main([NotNull, ItemNotNull] string[] args) {
            if (args.Length == 0) {
                Console.Error.WriteLine(HelpText);
                return 0;
            }

            var inputFile = Path.GetFullPath(args[0]);

            string outputFile;

            if (args.Length >= 2) {
                outputFile = args[1];
            } else {
                var fi = new FileInfo(inputFile);
                var name = fi.FullName;
                outputFile = name.Substring(0, name.Length - fi.Extension.Length) + ".txt";
            }

            var format = new SimpleScoreFormat();

            SourceScore sourceScore;

            using (var fileStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = format.CreateReader()) {
                    var sourceOptions = new ReadSourceOptions();

                    sourceScore = reader.ReadSourceScore(fileStream, fileStream.Name, sourceOptions);
                }
            }

            for (var i = 1; i < sourceScore.Conductors.Length; ++i) {
                FixNoteTickInfo(sourceScore.Conductors[i], sourceScore);
            }

            foreach (var sourceNote in sourceScore.Notes) {
                FixNoteTickInfo(sourceNote, sourceScore);
            }

            using (var fileStream = File.Open(outputFile, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                using (var writer = new StreamWriter(fileStream, Utf8WithoutBom)) {
                    WriteScore(sourceScore, writer);
                }
            }

            return 0;
        }

        private static void WriteScore([NotNull] SourceScore sourceScore, [NotNull] TextWriter writer) {
            TrackType trackType = TrackType.MillionMix;

            switch (sourceScore.TrackCount) {
                case 2:
                    trackType = TrackType.D2MixPlus;
                    break;
                case 4:
                    trackType = TrackType.D4Mix;
                    break;
                case 6:
                    trackType = TrackType.MillionMix;
                    break;
            }

            writer.WriteHeader();

            var phantoms = GeneratePhantomNotes(sourceScore);

            var notesBuffer = new List<SourceNote>();

            for (var i = 0; i < phantoms.Length; ++i) {
                notesBuffer.Add(phantoms[i]);
                notesBuffer.AddRange(sourceScore.Notes.Where(n => {
                    if (i < phantoms.Length - 1) {
                        return phantoms[i].Ticks <= n.Ticks && n.Ticks < phantoms[i + 1].Ticks;
                    } else {
                        return phantoms[i].Ticks <= n.Ticks;
                    }
                }));
            }

            var notes = notesBuffer.ToArray();

            writer.WriteEventNotes(sourceScore, notes, trackType);
            writer.WriteConductors(sourceScore);
            writer.WriteEndings();
            writer.WriteBgmOffset();
        }

        private static SourceNote[] GeneratePhantomNotes([NotNull] SourceScore sourceScore) {
            var result = new List<SourceNote>();

            var maxMeasure = 224;
            var ticksFactor = NoteBase.TicksPerBeat / 8;

            for (var measure = 0; measure <= maxMeasure; ++measure) {
                var primary = new SourceNote {
                    TrackIndex = -1,
                    Type = (NoteType)(-1),
                    Speed = 1,
                    Measure = measure + 1,
                    Beat = 1,
                    Ticks = 1920 * measure * ticksFactor
                };

                result.Add(primary);

                for (var j = 2; j <= 4; ++j) {
                    var secondary = new SourceNote {
                        TrackIndex = -1,
                        Type = (NoteType)(-2),
                        Speed = 1,
                        Measure = measure + 1,
                        Beat = j,
                        Ticks = primary.Ticks + (480 * (j - 1)) * ticksFactor
                    };

                    result.Add(secondary);
                }
            }

            return result.ToArray();
        }

        private static void WriteHeader([NotNull] this TextWriter writer) {
            const string header = @"PPtr<GameObject> m_GameObject
	int m_FileID = 0
	SInt64 m_PathID = 0
UInt8 m_Enabled = 1
PPtr<MonoScript> m_Script
	int m_FileID = 0
	SInt64 m_PathID = -4896244744343370365
string m_Name = ""shtstr_fumen_sobj""";

            writer.WriteLine(header);
        }

        private static void WriteEventNotes([NotNull] this TextWriter writer, [NotNull] SourceScore sourceScore, [NotNull, ItemNotNull] SourceNote[] notes, TrackType trackType) {
            writer.WriteLine(@"EventNoteData evts
	Array Array");
            writer.WriteLine("	int size = " + notes.Length);

            for (var i = 0; i < notes.Length; ++i) {
                writer.WriteEventNote(sourceScore, notes, i, trackType);
            }
        }

        private static void WriteConductors([NotNull] this TextWriter writer, [NotNull] SourceScore sourceScore) {
            writer.WriteLine(@"EventConductorData ct
	Array Array");
            writer.WriteLine("	int size = " + sourceScore.Conductors.Length);

            for (var i = 0; i < sourceScore.Conductors.Length; ++i) {
                writer.WriteConductor(sourceScore, i);
            }
        }

        private static void WriteEventNote([NotNull] this TextWriter writer, [NotNull] SourceScore sourceScore, [NotNull, ItemNotNull] SourceNote[] notes, int index, TrackType trackType) {
            writer.WriteLine($"		[{index}]");

            var sourceNote = notes[index];

            writer.WriteLine("		EventNoteData data");

            // MLTD: "offset > 0" means music is AFTER the beatmap
            // MilliSim: "offset > 0" means music is BEFORE the beatmap
            var absTime = ScoreCompileHelper.TicksToSeconds(sourceNote.Ticks, sourceScore.Conductors);

            writer.WriteLine("			double absTime = " + absTime.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("			UInt8 selected = 0");
            writer.WriteLine("			SInt64 tick = " + (sourceNote.Ticks / (NoteBase.TicksPerBeat / 8)).ToString());

            if (sourceNote.Type < 0) {
                writer.WriteLine("			int measure = " + sourceNote.Measure.ToString());
                writer.WriteLine("			int beat = " + sourceNote.Beat.ToString());

                writer.WriteLine("			int track = -1");
            } else {
                writer.WriteLine("			int measure = 0");
                writer.WriteLine("			int beat = 0");

                var tracks = MltdHelper.GetTrackIndicesFromTrackType(trackType);

                writer.WriteLine("			int track = " + tracks[sourceNote.TrackIndex].ToString());
            }

            writer.WriteLine("			int type = " + ((int)MltdHelper.GetMltdNoteType(sourceNote)).ToString());
            writer.WriteLine("			float startPosx = " + sourceNote.StartX.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("			float endPosx = " + sourceNote.EndX.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("			float speed = " + sourceNote.Speed.ToString(CultureInfo.InvariantCulture));

            if (sourceNote.FollowingNotes != null && sourceNote.FollowingNotes.Length > 0) {
                var duration = (int)((sourceNote.FollowingNotes[sourceNote.FollowingNotes.Length - 1].Ticks - sourceNote.Ticks) / (NoteBase.TicksPerBeat / 8));
                writer.WriteLine("			int duration = " + duration.ToString());
            } else {
                writer.WriteLine("			int duration = 0");
            }

            writer.WritePolyPoints(sourceNote, sourceNote.FollowingNotes);

            if (sourceNote.FollowingNotes != null && sourceNote.FollowingNotes.Length > 0) {
                writer.WriteLine("			int endType = " + ((int)sourceNote.FollowingNotes[sourceNote.FollowingNotes.Length - 1].FlickDirection).ToString());
            } else {
                writer.WriteLine("			int endType = 0");
            }

            if (sourceNote.Type < 0) {
                writer.WriteLine("			double leadTime = 0");
            } else {
                // TODO: What is this "magic number"?
                const double someConstant = 198.3471011848414;
                var bpm = GetCurrentBpm(sourceNote.Ticks, sourceScore.Conductors);
                var leadTime = someConstant / bpm;
                // TODO: Another guess... Didn't look too carefully inside speed variated notes.
                leadTime /= sourceNote.Speed;
                writer.WriteLine("			double leadTime = " + leadTime.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void WriteConductor([NotNull] this TextWriter writer, [NotNull] SourceScore sourceScore, int index) {
            writer.WriteLine($"		[{index}]");

            var conductor = sourceScore.Conductors[index];

            writer.WriteLine("		EventConductorData data");

            var absTime = ScoreCompileHelper.TicksToSeconds(conductor.Ticks, sourceScore.Conductors);

            writer.WriteLine("			double absTime = " + absTime.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("			UInt8 selected = 0");
            writer.WriteLine("			SInt64 tick = " + conductor.Ticks.ToString());
            writer.WriteLine("			int measure = " + conductor.Measure.ToString());
            writer.WriteLine("			int beat = " + conductor.Beat.ToString());
            writer.WriteLine("			int track = 0");
            writer.WriteLine("			double tempo = " + conductor.Tempo.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("			int tsigNumerator = " + conductor.SignatureNumerator.ToString());
            writer.WriteLine("			int tsigDenominator = " + conductor.SignatureDenominator.ToString());
            writer.WriteLine("			string marker = \"\"");
        }

        private static void WritePolyPoints([NotNull] this TextWriter writer, [NotNull] SourceNote note, [CanBeNull, ItemNotNull] SourceNote[] notes) {
            writer.WriteLine(@"			PolyPoint poly
				Array Array");

            var count = notes?.Length ?? 0;

            writer.WriteLine("				int size = " + (count == 0 ? 0 : count + 1).ToString());

            if (count > 0) {
                Debug.Assert(notes != null, nameof(notes) + " != null");

                writer.WriteLine(@"					[0]
					PolyPoint data
						int subtick = 0
						float posx = " + note.EndX.ToString(CultureInfo.InvariantCulture));

                for (var i = 0; i < notes.Length; ++i) {
                    var subtick = (int)((notes[i].Ticks - note.Ticks) / (NoteBase.TicksPerBeat / 8));
                    writer.WriteLine($"					[{i + 1}]");
                    writer.WriteLine("					PolyPoint data");
                    writer.WriteLine("						int subtick = " + subtick.ToString());
                    writer.WriteLine("						float posx = " + notes[i].EndX.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private static void WriteEndings([NotNull] this TextWriter writer) {
            writer.WriteLine(@"vector scoreSpeed
	Array Array
	int size = 16
		[0]
		float data = 1.13
		[1]
		float data = 1.17
		[2]
		float data = 1
		[3]
		float data = 1
		[4]
		float data = 1.27
		[5]
		float data = 1
		[6]
		float data = 1
		[7]
		float data = 1
		[8]
		float data = 1.33
		[9]
		float data = 1.53
		[10]
		float data = 1
		[11]
		float data = 1
		[12]
		float data = 1.13
		[13]
		float data = 0
		[14]
		float data = 0
		[15]
		float data = 0
vector judgeRange
	Array Array
	int size = 64
		[0]
		float data = 5
		[1]
		float data = 9
		[2]
		float data = 14
		[3]
		float data = 16
		[4]
		float data = 3
		[5]
		float data = 6
		[6]
		float data = 11
		[7]
		float data = 13
		[8]
		float data = 3
		[9]
		float data = 6
		[10]
		float data = 9
		[11]
		float data = 11
		[12]
		float data = 3
		[13]
		float data = 6
		[14]
		float data = 9
		[15]
		float data = 11
		[16]
		float data = 5
		[17]
		float data = 9
		[18]
		float data = 14
		[19]
		float data = 16
		[20]
		float data = 5
		[21]
		float data = 9
		[22]
		float data = 14
		[23]
		float data = 16
		[24]
		float data = 5
		[25]
		float data = 9
		[26]
		float data = 14
		[27]
		float data = 16
		[28]
		float data = 5
		[29]
		float data = 9
		[30]
		float data = 14
		[31]
		float data = 16
		[32]
		float data = 3
		[33]
		float data = 6
		[34]
		float data = 11
		[35]
		float data = 13
		[36]
		float data = 3
		[37]
		float data = 6
		[38]
		float data = 9
		[39]
		float data = 11
		[40]
		float data = 3
		[41]
		float data = 6
		[42]
		float data = 9
		[43]
		float data = 11
		[44]
		float data = 3
		[45]
		float data = 6
		[46]
		float data = 9
		[47]
		float data = 11
		[48]
		float data = 0
		[49]
		float data = 0
		[50]
		float data = 0
		[51]
		float data = 0
		[52]
		float data = 0
		[53]
		float data = 0
		[54]
		float data = 0
		[55]
		float data = 0
		[56]
		float data = 0
		[57]
		float data = 0
		[58]
		float data = 0
		[59]
		float data = 0
		[60]
		float data = 0
		[61]
		float data = 0
		[62]
		float data = 0
		[63]
		float data = 0");
        }

        private static void WriteBgmOffset([NotNull] this TextWriter writer) {
            // How long is the duration that BGM comes after the first measure
            writer.WriteLine("float BGM_offset = 0");
        }

        private static void FixNoteTickInfo([NotNull] NoteBase note, [NotNull] SourceScore sourceScore) {
            var baseConductor = sourceScore.Conductors[0];

            // For a properly-designed beatmap, the extra measure count SHOULD be an integer.
            // Otherwise you need to move your music start to make it on a beat.
            var extraMeasures = (int)Math.Round(sourceScore.MusicOffset / (60 / baseConductor.Tempo) / baseConductor.SignatureDenominator);

            var tickDiff = NoteBase.TicksPerBeat * 60 * extraMeasures * baseConductor.SignatureDenominator;

            note.Measure += extraMeasures;
            note.Ticks += tickDiff;

            if (note is SourceNote n) {
                if (n.FollowingNotes != null && n.FollowingNotes.Length > 0) {
                    foreach (var fn in n.FollowingNotes) {
                        fn.Ticks += tickDiff;
                    }
                }
            }
        }

        private static double GetCurrentBpm(long currentTicks, [NotNull, ItemNotNull] Conductor[] conductors) {
            Conductor con = conductors[0];

            for (var i = 1; i < conductors.Length; ++i) {
                if (conductors[i].Ticks > currentTicks) {
                    break;
                }

                con = conductors[i];
            }

            return con.Tempo;
        }

        private const string HelpText = "Export <source ss> [<output txt>]";

        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

    }
}
