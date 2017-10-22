using System;
using System.IO;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore.ConsoleTest {
    internal static class Program {

        private static void Main(string[] args) {
            var format = new SimpleScoreFormat();
            var sourceOptions = new ReadSourceOptions {
                ScoreIndex = 0
            };

            SourceScore score;
            using (var fileStream = File.Open("simple.ss", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = format.CreateReader()) {
                    score = reader.ReadSourceScore(fileStream, fileStream.Name, sourceOptions);
                }
            }

            Console.WriteLine(score.Notes.Length);
#if DEBUG
            Console.ReadKey();
#endif
        }

    }
}
