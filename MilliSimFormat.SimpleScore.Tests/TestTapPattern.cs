using MilliSimFormat.SimpleScore.Internal;
using NUnit.Framework;

namespace MilliSimFormat.SimpleScore.Tests {
    [TestFixture]
    public sealed class TestTapPattern {

        [Test]
        public void Tap_SimpleInput() {
            var s = @"t:[1,1/4,3]()";
            var tap = Tap.FromString(s);

            var std = new Tap {
                Header = new NoteHeader {
                    Measure = 1,
                    Nominator = 1,
                    Denominator = 4,
                    Start = 3,
                    End = 3,
                    Speed = 1
                },
                Body = new TapBody {
                    Size = string.Empty
                }
            };

            Assert.AreEqual(std, tap);
        }

        [Test]
        public void Tap_InputWithSize() {
            var s = @"t:[1,1/4,3](large!)";
            var tap = Tap.FromString(s);

            var std = new Tap {
                Header = new NoteHeader {
                    Measure = 1,
                    Nominator = 1,
                    Denominator = 4,
                    Start = 3,
                    End = 3,
                    Speed = 1
                },
                Body = new TapBody {
                    Size = "large"
                }
            };

            Assert.AreEqual(std, tap);
        }

    }
}
