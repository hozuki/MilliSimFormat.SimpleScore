using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj {
    internal static class MltdHelper {

        internal static MltdNoteType GetMltdNoteType([NotNull] SourceNote note) {
            switch (note.Type) {
                case NoteType.Tap:
                    switch (note.Size) {
                        case NoteSize.Small:
                            return MltdNoteType.TapSmall;
                        case NoteSize.Large:
                            return MltdNoteType.TapLarge;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case NoteType.Flick:
                    switch (note.FlickDirection) {
                        case FlickDirection.Left:
                            return MltdNoteType.FlickLeft;
                        case FlickDirection.Up:
                            return MltdNoteType.FlickUp;
                        case FlickDirection.Right:
                            return MltdNoteType.FlickRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case NoteType.Hold:
                    switch (note.Size) {
                        case NoteSize.Small:
                            return MltdNoteType.HoldSmall;
                        case NoteSize.Large:
                            return MltdNoteType.HoldLarge;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case NoteType.Slide:
                    return MltdNoteType.SlideSmall;
                case NoteType.Special:
                    return MltdNoteType.Special;
                case (NoteType)(-1):
                    return MltdNoteType.PrimaryBeat;
                case (NoteType)(-2):
                    return MltdNoteType.SecondaryBeat;
                case NoteType.SpecialEnd:
                case NoteType.SpecialPrepare:
                case NoteType.ScorePrepare:
                    throw new NotSupportedException($"Warning: not supported source note type: {note.Type}");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static int[] GetTrackIndicesFromTrackType(TrackType trackType) {
            switch (trackType) {
                case TrackType.Block:
                    return TracksList[0];
                case TrackType.Conductor:
                    return TracksList[1];
                case TrackType.D2Mix:
                    return TracksList[2];
                case TrackType.D2MixPlus:
                    return TracksList[3];
                case TrackType.D4Mix:
                    return TracksList[4];
                case TrackType.D6Mix:
                    return TracksList[5];
                case TrackType.MillionMix:
                    return TracksList[6];
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackType), trackType, null);
            }
        }

        private static readonly int[][] TracksList = {
            new[] { -1 },
            new[] { 0 },
            new[] { 1, 2 },
            new[] { 3, 4 },
            new[] { 9, 10, 11, 12 },
            new[] { 25, 26, 27, 28, 29, 30 },
            new[] { 31, 32, 33, 34, 35, 36 }
        };

    }
}
