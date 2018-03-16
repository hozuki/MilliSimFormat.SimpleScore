using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MilliSimFormat.SimpleScore.ToExportedScrobj.Models;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj {
    public static class WriteScenario {

        public static void Write([NotNull] SourceScore score, [NotNull] TextWriter writer) {
            var template = new ScenarioScrObj();

            ScrObjLoader.LoadScenario(template, "Resources/Templates/blsymp_scenario_sobj_shtstr.txt");

            writer.WriteHeader();

            writer.WriteNotes(score, template);

            writer.WriteTexs();
            writer.WriteScenarioNote(0, "ap_st", template.ap_st);
            writer.WriteScenarioNote(0, "ap_pose", template.ap_pose);
            writer.WriteScenarioNote(0, "ap_end", template.ap_end);
            writer.WriteScenarioNote(0, "fine_ev", template.fine_ev);
        }

        private static void WriteNotes([NotNull] this TextWriter writer, [NotNull] SourceScore score, [NotNull] ScenarioScrObj template) {
            var notes = score.Notes;
            var noteList = new List<EventScenarioData>();

            var specialNote = notes.FirstOrDefault(n => n.Type == NoteType.Special);

            // Mofify time: the tap buttons animation before the special note (the big round note)
            if (specialNote != null) {
                noteList.AddRange(template.scenario);

                var animNote = noteList.Find(n => n.type == ScenarioNoteType.BeginTapButtonsAnimation);

                if (animNote != null) {
                    var specialNoteTime = ScoreCompileHelper.TicksToSeconds(specialNote.Ticks, score.Conductors);

                    animNote.absTime = specialNoteTime - 0.5f;
                    animNote.tick = ScorePreprocessor.SecondsToTicks(animNote.absTime, score.Conductors);
                }

                // Modify time: second tap buttons appearing time (the one that is after the special note)
                var reappearNote = notes.FirstOrDefault(n => n.Ticks > specialNote.Ticks);

                if (reappearNote != null && animNote != null) {
                    var reaNote = noteList.Where(n => n.type == ScenarioNoteType.ShowTapButtons).Skip(1).FirstOrDefault();

                    if (reaNote != null) {
                        var reappearNoteTime = ScoreCompileHelper.TicksToSeconds(reappearNote.Ticks, score.Conductors);

                        reaNote.absTime = reappearNoteTime - 0.8f;
                        reaNote.tick = ScorePreprocessor.SecondsToTicks(reaNote.absTime, score.Conductors);
                    }
                }
            }

            noteList.Sort((n1, n2) => {
                var t = n1.tick.CompareTo(n2.tick);

                if (t != 0) {
                    return t;
                }

                t = ((int)n1.type).CompareTo((int)n2.type);

                if (t != 0) {
                    return t;
                }

                t = n1.idol.CompareTo(n2.idol);

                if (t != 0) {
                    return t;
                }

                return n1.track.CompareTo(n2.track);
            });

            writer.Write(@"EventScenarioData scenario
	Array Array
	int size = ");
            writer.WriteLine(noteList.Count.ToString());

            for (var i = 0; i < noteList.Count; ++i) {
                var sn = noteList[i];

                writer.WriteLine("		[{0}]", i.ToString());
                writer.WriteScenarioNote(2, "data", sn);
            }
        }

        private static void WriteHeader([NotNull] this TextWriter writer) {
            const string header = @"PPtr<GameObject> m_GameObject
	int m_FileID = 0
	SInt64 m_PathID = 0
UInt8 m_Enabled = 1
PPtr<MonoScript> m_Script
	int m_FileID = 0
	SInt64 m_PathID = -4896244744343370365
string m_Name = ""shtstr_scenario_sobj""";

            writer.WriteLine(header);
        }

        private static void WriteScenarioNote([NotNull] this TextWriter writer, int baseIndent, [NotNull] string name, EventScenarioData data) {

            WriteLine("EventScenarioData " + name, baseIndent);
            WriteLine("double absTime = " + data.absTime.ToString(CultureInfo.InvariantCulture), baseIndent + 1);
            WriteLine("UInt8 selected = 0", baseIndent + 1);
            WriteLine("SInt64 tick = " + data.tick.ToString(), baseIndent + 1);
            WriteLine("int measure = " + data.measure.ToString(), baseIndent + 1);
            WriteLine("int beat = " + data.beat.ToString(), baseIndent + 1);
            WriteLine("int track = " + data.track.ToString(), baseIndent + 1);
            WriteLine("int type = " + ((int)data.type).ToString(), baseIndent + 1);
            WriteLine("int param = " + data.param.ToString(), baseIndent + 1);
            WriteLine("int target = " + data.target.ToString(), baseIndent + 1);
            WriteLine("SInt64 duration = " + data.duration.ToString(), baseIndent + 1);
            WriteLine("string str = \"" + data.str + "\"", baseIndent + 1);
            WriteLine("string info = \"" + data.info + "\"", baseIndent + 1);
            WriteLine("int on = " + data.on.ToString(), baseIndent + 1);
            WriteLine("int on2 = " + data.on2.ToString(), baseIndent + 1);
            WriteLine("ColorRGBA col", baseIndent + 1);
            WriteLine("float r = " + data.col.r.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("float g = " + data.col.g.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("float b = " + data.col.b.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("float a = " + data.col.a.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("ColorRGBA col2", baseIndent + 1);
            WriteLine("float r = " + data.col2.r.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("float g = " + data.col2.g.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("float b = " + data.col2.b.ToString(CultureInfo.InvariantCulture), baseIndent + 2);
            WriteLine("float a = " + data.col2.a.ToString(CultureInfo.InvariantCulture), baseIndent + 2);

            const string fixedFormat = @"	vector cols
		Array Array
		int size = 0
	PPtr<$Texture> tex
		int m_FileID = 0
		SInt64 m_PathID = 0
	int texInx = -1
	int trig = 0
	float speed = 1
	int idol = 0
	vector mute
		Array Array
		int size = 0
	UInt8 addf = 0
	float eye_x = 0
	float eye_y = 0
	vector formation
		Array Array
		int size = 0
	UInt8 appeal = 0
	int cheeklv = 0
	UInt8 eyeclose = 0
	UInt8 talking = 0
	UInt8 delay = 0
	vector clratio
		Array Array
		int size = 0
	vector clcols
		Array Array
		int size = 0
	int camCut = -1
	VjParam vjparam
		UInt8 use = 0
		UInt8 renderTex = 0
		int col = 8
		int row = 4
		int begin = 0
		int speed = 16
		ColorRGBA color
			float r = 0
			float g = 0
			float b = 0
			float a = 0";

            var fixedLines = fixedFormat.Split(WinNewLineSeparators, StringSplitOptions.None);

            foreach (var line in fixedLines) {
                WriteLine(line, baseIndent);
            }

            void WriteLine(string str, int indent) {
                if (indent > 0) {
                    writer.Write(new string('\t', indent));
                }

                writer.WriteLine(str);
            }
        }

        private static void WriteTexs([NotNull] this TextWriter writer) {
            const string texs = @"TexTargetName texs
	Array Array
	int size = 4
		[0]
		TexTargetName data
			int target = 20
			string name = ""vj_white""
		[1]
		TexTargetName data
			int target = 20
			string name = ""vj_kaleida_04""
		[2]
		TexTargetName data
			int target = 20
			string name = ""vj_kaleida_01""
		[3]
		TexTargetName data
			int target = 13
			string name = ""ltmap_square01""";

            writer.WriteLine(texs);
        }

        private static readonly string[] WinNewLineSeparators = { "\r\n" };

    }
}
