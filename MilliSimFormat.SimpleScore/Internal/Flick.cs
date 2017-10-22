using System;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class Flick : IEquatable<Flick> {

        internal NoteHeader Header { get; set; }

        internal FlickBody Body { get; set; }

        internal static Flick FromString(string str) {
            var match = SimpleScoreReader.FlickPattern.Match(str);
            if (!match.Success) {
                throw new FormatException();
            }

            var header = NoteHeader.FromMatch(match);
            var body = FlickBody.FromMatch(match);
            return new Flick {
                Header = header,
                Body = body
            };
        }

        public bool Equals(Flick other) {
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
            return obj is Flick && Equals((Flick)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Header != null ? Header.GetHashCode() : 0) * 397) ^ (Body != null ? Body.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Flick left, Flick right) {
            return Equals(left, right);
        }

        public static bool operator !=(Flick left, Flick right) {
            return !Equals(left, right);
        }

    }
}
