using System;
using System.Collections.Generic;
using System.Linq;

namespace MilliSimFormat.SimpleScore.Internal {
    internal sealed class Slide : IEquatable<Slide> {

        internal NoteHeader Header { get; set; }

        internal SlideBody Body { get; set; }

        internal static Slide FromString(string str) {
            var match = SimpleScoreReader.SlidePattern.Match(str);
            if (!match.Success) {
                throw new FormatException();
            }

            var header = NoteHeader.FromMatch(match);
            var body = SlideBody.FromMatch(match);
            return new Slide {
                Header = header,
                Body = body
            };
        }

        internal static IReadOnlyList<Slide> CreateSlides(string str) {
            var match = SimpleScoreReader.SlideSeriesPattern.Match(str);
            var realValue = match.Value.Substring(2);
            var strs = realValue.Split(new[] { SimpleScoreReader.SeriesSeparator }, StringSplitOptions.None);
            var slides = strs.Select(FromString).ToArray();
            return slides;
        }

        public bool Equals(Slide other) {
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
            return obj is Slide && Equals((Slide)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Header != null ? Header.GetHashCode() : 0) * 397) ^ (Body != null ? Body.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Slide left, Slide right) {
            return Equals(left, right);
        }

        public static bool operator !=(Slide left, Slide right) {
            return !Equals(left, right);
        }

    }
}
