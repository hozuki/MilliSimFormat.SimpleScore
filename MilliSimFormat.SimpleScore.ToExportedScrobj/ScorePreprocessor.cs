using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj {
    public static class ScorePreprocessor {

        public static void FixNoteTickInfo([NotNull] NoteBase note, [NotNull] SourceScore sourceScore) {
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

        public static double GetCurrentBpm(long currentTicks, [NotNull, ItemNotNull] Conductor[] conductors) {
            Conductor con = conductors[0];

            for (var i = 1; i < conductors.Length; ++i) {
                if (conductors[i].Ticks > currentTicks) {
                    break;
                }

                con = conductors[i];
            }

            return con.Tempo;
        }

        public static long SecondsToTicks(double seconds, [NotNull, ItemNotNull] Conductor[] conductors) {
            if (conductors.Length == 1) {
                return (long)Math.Round(seconds * conductors[0].Tempo * MltdTicksPerBeat);
            }

            double timeElapsed = 0;

            for (var i = 0; i < conductors.Length - 1; ++i) {
                var thisConductorDuration = GetConductorDuration(i);

                if (timeElapsed + thisConductorDuration < seconds) {
                    timeElapsed += thisConductorDuration;

                    continue;
                }

                return (long)Math.Round((seconds - timeElapsed) * conductors[i].Tempo * MltdTicksPerBeat);
            }

            return (long)Math.Round((seconds - timeElapsed) * conductors[conductors.Length - 1].Tempo * MltdTicksPerBeat);

            double GetConductorDuration(int conductorIndex) {
                var tempo = conductors[conductorIndex].Tempo;
                var nextTicks = conductors[conductorIndex + 1].Ticks;
                var deltaTicks = nextTicks - conductors[conductorIndex].Ticks;
                var deltaTime = deltaTicks / (tempo * MltdTicksPerBeat);

                return deltaTime;
            }
        }

        public const int MltdTicksPerBeat = 8;

    }
}
