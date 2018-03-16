using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj {
    internal static class Program {

        private static int Main([NotNull, ItemNotNull] string[] args) {
            if (args.Length == 0) {
                Console.Error.WriteLine(HelpText);
                return 0;
            }

            var inputFile = Path.GetFullPath(args[0]);

            string outputScoreFile;

            if (args.Length >= 2) {
                outputScoreFile = args[1];
            } else {
                var fi = new FileInfo(inputFile);
                var name = fi.FullName;
                outputScoreFile = name.Substring(0, name.Length - fi.Extension.Length) + ".txt";
            }

            string outputScenarioFile;

            if (args.Length >= 3) {
                outputScenarioFile = args[2];
            } else {
                var fi = new FileInfo(inputFile);
                var name = fi.FullName;
                outputScenarioFile = name.Substring(0, name.Length - fi.Extension.Length) + "_scenario.txt";
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
                ScorePreprocessor.FixNoteTickInfo(sourceScore.Conductors[i], sourceScore);
            }

            foreach (var sourceNote in sourceScore.Notes) {
                ScorePreprocessor.FixNoteTickInfo(sourceNote, sourceScore);
            }

            using (var fileStream = File.Open(outputScoreFile, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                using (var writer = new StreamWriter(fileStream, Utf8WithoutBom)) {
                    WriteScore.Write(sourceScore, writer);
                }
            }

            using (var fileStream = File.Open(outputScenarioFile, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                using (var writer = new StreamWriter(fileStream, Utf8WithoutBom)) {
                    WriteScenario.Write(sourceScore, writer);
                }
            }

            return 0;
        }

        private const string HelpText = "Export <source ss> [<output score txt> [<output scenario txt>]]";

        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

    }
}
