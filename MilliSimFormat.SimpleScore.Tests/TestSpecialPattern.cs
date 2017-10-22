using MilliSimFormat.SimpleScore.Internal;
using NUnit.Framework;

namespace MilliSimFormat.SimpleScore.Tests {
    [TestFixture]
    public sealed class TestSpecialPattern {

        [Test]
        public void Special_SimplePairWithoutParentheses() {
            var s = @"x:[1,1/4,3]";
            var special = Special.FromString(s);

            var std = new Special {
                Header = new NoteHeader {
                    Measure = 1,
                    Nominator = 1,
                    Denominator = 4,
                    Start = 3,
                    End = 3,
                    Speed = 1
                }
            };

            Assert.AreEqual(std, special);
        }

        [Test]
        public void Special_SimplePair() {
            var s = @"x:[1,1/4,3]()";
            var special = Special.FromString(s);

            var std = new Special {
                Header = new NoteHeader {
                    Measure = 1,
                    Nominator = 1,
                    Denominator = 4,
                    Start = 3,
                    End = 3,
                    Speed = 1
                }
            };

            Assert.AreEqual(std, special);
        }

    }
}
