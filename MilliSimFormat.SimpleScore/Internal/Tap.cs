using System;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class Tap : IEquatable<Tap> {

        internal NoteHeader Header { get; set; }

        internal TapBody Body { get; set; }

        internal static Tap FromString(string str) {
            var match = SimpleScoreReader.TapPattern.Match(str);
            if (!match.Success) {
                throw new FormatException();
            }

            var header = NoteHeader.FromMatch(match);
            var body = TapBody.FromMatch(match);
            return new Tap {
                Header = header,
                Body = body
            };
        }

        public bool Equals(Tap other) {
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
            return obj is Tap && Equals((Tap)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Header != null ? Header.GetHashCode() : 0) * 397) ^ (Body != null ? Body.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Tap left, Tap right) {
            return Equals(left, right);
        }

        public static bool operator !=(Tap left, Tap right) {
            return !Equals(left, right);
        }

    }
}
