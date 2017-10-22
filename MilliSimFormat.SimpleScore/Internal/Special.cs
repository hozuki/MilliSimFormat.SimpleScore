using System;
using System.Collections.Generic;
using System.Linq;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class Special : IEquatable<Special> {

        internal NoteHeader Header { get; set; }

        internal static Special FromString(string str) {
            var match = SimpleScoreReader.SpecialPattern.Match(str);
            if (!match.Success) {
                throw new FormatException();
            }

            var header = NoteHeader.FromMatch(match);
            return new Special {
                Header = header
            };
        }

        public bool Equals(Special other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Header, other.Header);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is Special && Equals((Special)obj);
        }

        public override int GetHashCode() {
            return (Header != null ? Header.GetHashCode() : 0);
        }

        public static bool operator ==(Special left, Special right) {
            return Equals(left, right);
        }

        public static bool operator !=(Special left, Special right) {
            return !Equals(left, right);
        }

    }
}
