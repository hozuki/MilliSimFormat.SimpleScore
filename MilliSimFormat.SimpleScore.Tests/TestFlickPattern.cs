using System;
using MilliSimFormat.SimpleScore.Internal;
using NUnit.Framework;

namespace MilliSimFormat.SimpleScore.Tests {
    [TestFixture]
    public sealed class TestFlickPattern {

        [Test]
        public void Flick_WithProperties1() {
            var s = @"f:[1,1/4,3](left!)";
            var flick = Flick.FromString(s);

            var std = new Flick {
                Header = new NoteHeader {
                    Measure = 1,
                    Nominator = 1,
                    Denominator = 4,
                    Start = 3,
                    End = 3,
                    Speed = 1
                },
                Body = new FlickBody {
                    Direction = "left",
                    Size = string.Empty
                }
            };

            Assert.AreEqual(std, flick);
        }

        [Test]
        public void Flick_WithProperties2() {
            var s = @"f:[1,1/4,3](left!small!)";
            var flick = Flick.FromString(s);

            var std = new Flick {
                Header = new NoteHeader {
                    Measure = 1,
                    Nominator = 1,
                    Denominator = 4,
                    Start = 3,
                    End = 3,
                    Speed = 1
                },
                Body = new FlickBody {
                    Direction = "left",
                    Size = "small"
                }
            };

            Assert.AreEqual(std, flick);
        }

        [Test]
        public void Flick_InvalidFormat() {
            var s = @"f:[1,1/4,3](left)";
            try {
                var flick = Flick.FromString(s);
                Assert.Fail("Should fail.");
            } catch (FormatException) {
            }
        }

    }
}
