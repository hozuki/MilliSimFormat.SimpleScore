using System;
using System.Text.RegularExpressions;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class NoteHeader : IEquatable<NoteHeader> {

        internal static NoteHeader FromString(string str) {
            var match = SimpleScoreReader.NoteHeaderPattern.Match(str);
            return FromMatch(match);
        }

        internal static NoteHeader FromMatch(Match match) {
            if (!match.Success) {
                throw new FormatException();
            }

            var header = new NoteHeader();

            header.Measure = Convert.ToInt32(match.Groups["measure"].Value);
            header.Nominator = Convert.ToInt32(match.Groups["nom"].Value);
            header.Denominator = Convert.ToInt32(match.Groups["denom"].Value);
            header.Start = Convert.ToSingle(match.Groups["start"].Value);

            try {
                header.End = Convert.ToSingle(match.Groups["end"].Value);
            } catch (FormatException) {
                header.End = header.Start;
            }

            try {
                header.Speed = Convert.ToSingle(match.Groups["speed"].Value);
            } catch (FormatException) {
                header.Speed = 1;
            }

            return header;
        }

        internal int Measure { get; set; }

        internal int Nominator { get; set; }

        internal int Denominator { get; set; }

        internal float Start { get; set; }

        internal float End { get; set; }

        internal float Speed { get; set; }

        public bool Equals(NoteHeader other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Measure == other.Measure && Nominator == other.Nominator && Denominator == other.Denominator && Start.Equals(other.Start) && End.Equals(other.End) && Speed.Equals(other.Speed);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is NoteHeader && Equals((NoteHeader)obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Measure;
                hashCode = (hashCode * 397) ^ Nominator;
                hashCode = (hashCode * 397) ^ Denominator;
                hashCode = (hashCode * 397) ^ Start.GetHashCode();
                hashCode = (hashCode * 397) ^ End.GetHashCode();
                hashCode = (hashCode * 397) ^ Speed.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(NoteHeader left, NoteHeader right) {
            return Equals(left, right);
        }

        public static bool operator !=(NoteHeader left, NoteHeader right) {
            return !Equals(left, right);
        }

    }
}
