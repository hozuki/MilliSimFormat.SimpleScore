using System;
using System.Text.RegularExpressions;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class TapBody : IEquatable<TapBody> {

        internal static TapBody FromMatch(Match match) {
            var body = new TapBody();
            body.Size = match.Groups["size"].Value;
            return body;
        }

        internal string Size { get; set; }

        public bool Equals(TapBody other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Size, other.Size);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is TapBody && Equals((TapBody)obj);
        }

        public override int GetHashCode() {
            return (Size != null ? Size.GetHashCode() : 0);
        }

        public static bool operator ==(TapBody left, TapBody right) {
            return Equals(left, right);
        }

        public static bool operator !=(TapBody left, TapBody right) {
            return !Equals(left, right);
        }

    }
}
