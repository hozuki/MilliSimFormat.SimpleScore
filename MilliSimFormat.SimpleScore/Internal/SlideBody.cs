using System;
using System.Text.RegularExpressions;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class SlideBody : IEquatable<SlideBody> {

        internal static SlideBody FromMatch(Match match) {
            var body = new SlideBody {
                Size = match.Groups["size"].Value,
                Direction = match.Groups["direction"].Value
            };
            return body;
        }

        internal string Size { get; set; }

        internal string Direction { get; set; }

        public bool Equals(SlideBody other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Size, other.Size) && string.Equals(Direction, other.Direction);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is SlideBody && Equals((SlideBody)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Size != null ? Size.GetHashCode() : 0) * 397) ^ (Direction != null ? Direction.GetHashCode() : 0);
            }
        }

        public static bool operator ==(SlideBody left, SlideBody right) {
            return Equals(left, right);
        }

        public static bool operator !=(SlideBody left, SlideBody right) {
            return !Equals(left, right);
        }

    }
}
