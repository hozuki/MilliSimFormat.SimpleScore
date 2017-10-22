using System;
using System.Collections.Generic;
using System.Linq;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class Hold : IEquatable<Hold> {

        internal NoteHeader Header { get; set; }

        internal HoldBody Body { get; set; }

        internal static Hold FromString(string str) {
            var match = SimpleScoreReader.HoldPattern.Match(str);
            if (!match.Success) {
                throw new FormatException();
            }

            var header = NoteHeader.FromMatch(match);
            var body = HoldBody.FromMatch(match);
            return new Hold {
                Header = header,
                Body = body
            };
        }

        internal static IReadOnlyList<Hold> CreateHolds(string str) {
            var match = SimpleScoreReader.HoldPairPattern.Match(str);
            if (!match.Success) {
                throw new FormatException();
            }

            var realValue = match.Value.Substring(2);
            var strs = realValue.Split(new[] { SimpleScoreReader.SeriesSeparator }, StringSplitOptions.None);
            var holds = strs.Select(FromString).ToArray();
            return holds;
        }

        public bool Equals(Hold other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Header, other.Header) && Equals(Body, other.Body);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is Hold && Equals((Hold)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Header != null ? Header.GetHashCode() : 0) * 397) ^ (Body != null ? Body.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Hold left, Hold right) {
            return Equals(left, right);
        }

        public static bool operator !=(Hold left, Hold right) {
            return !Equals(left, right);
        }

    }
}
