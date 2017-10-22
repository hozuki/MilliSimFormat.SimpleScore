using MilliSimFormat.SimpleScore.Internal;
using NUnit.Framework;

namespace MilliSimFormat.SimpleScore.Tests {
    [TestFixture]
    public sealed class TestNoteHeaderPattern {

        [Test]
        public void Header_SimpleInput() {
            var s = @"[1,1/4,3]";
            var header = NoteHeader.FromString(s);

            var std = new NoteHeader {
                Measure = 1,
                Nominator = 1,
                Denominator = 4,
                Start = 3,
                End = 3,
                Speed = 1
            };

            Assert.AreEqual(std, header);
        }

        [Test]
        public void Header_SpecifyStartEnd() {
            var s = @"[1,1/4,3,4]";
            var header = NoteHeader.FromString(s);

            var std = new NoteHeader {
                Measure = 1,
                Nominator = 1,
                Denominator = 4,
                Start = 3,
                End = 4,
                Speed = 1
            };

            Assert.AreEqual(std, header);
        }

        [Test]
        public void Header_SpecifyStartEndSpeed() {
            var s = @"[1,1/4,3,4,10]";
            var header = NoteHeader.FromString(s);

            var std = new NoteHeader {
                Measure = 1,
                Nominator = 1,
                Denominator = 4,
                Start = 3,
                End = 4,
                Speed = 10
            };

            Assert.AreEqual(std, header);
        }

        [Test]
        public void Header_DotSpeed1() {
            var s = @"[1,1/4,3,4,.10]";
            var header = NoteHeader.FromString(s);

            var std = new NoteHeader {
                Measure = 1,
                Nominator = 1,
                Denominator = 4,
                Start = 3,
                End = 4,
                Speed = 0.1f
            };

            Assert.AreEqual(std, header);
        }

        [Test]
        public void Header_DotSpeed2() {
            var s = @"[1,1/4,3,4,3.10]";
            var header = NoteHeader.FromString(s);

            var std = new NoteHeader {
                Measure = 1,
                Nominator = 1,
                Denominator = 4,
                Start = 3,
                End = 4,
                Speed = 3.1f
            };

            Assert.AreEqual(std, header);
        }

        [Test]
        public void Header_DotSpeed3() {
            var s = @"[1,1/4,3,4,2.]";
            var header = NoteHeader.FromString(s);

            var std = new NoteHeader {
                Measure = 1,
                Nominator = 1,
                Denominator = 4,
                Start = 3,
                End = 4,
                Speed = 2
            };

            Assert.AreEqual(std, header);
        }

    }
}
