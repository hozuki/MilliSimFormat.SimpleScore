using MilliSimFormat.SimpleScore.Internal;
using NUnit.Framework;

namespace MilliSimFormat.SimpleScore.Tests {
    [TestFixture]
    public sealed class TestHoldPattern {

        [Test]
        public void Hold_SimplePairWithoutParentheses() {
            var s = @"h:[1,1/4,3]->[2,3/4,1,1]";
            var holds = Hold.CreateHolds(s);

            var stds = new[] {
                new Hold {
                    Header = new NoteHeader{ Measure = 1, Nominator = 1, Denominator = 4, Start = 3, End = 3, Speed = 1 },
                    Body = new HoldBody { Direction = string.Empty, Size = string.Empty }
                },
                new Hold {
                    Header = new NoteHeader{ Measure = 2, Nominator = 3, Denominator = 4, Start = 1, End = 1, Speed = 1 },
                    Body = new HoldBody { Direction = string.Empty, Size = string.Empty }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], holds[i]);
            }
        }

        [Test]
        public void Hold_SimplePair() {
            var s = @"h:[1,1/4,3]()->[2,3/4,1,1]()";
            var holds = Hold.CreateHolds(s);

            var stds = new[] {
                new Hold {
                    Header = new NoteHeader{ Measure = 1, Nominator = 1, Denominator = 4, Start = 3, End = 3, Speed = 1 },
                    Body = new HoldBody { Direction = string.Empty, Size = string.Empty }
                },
                new Hold {
                    Header = new NoteHeader{ Measure = 2, Nominator = 3, Denominator = 4, Start = 1, End = 1, Speed = 1 },
                    Body = new HoldBody { Direction = string.Empty, Size = string.Empty }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], holds[i]);
            }
        }

        [Test]
        public void Hold_AdvancedPair() {
            var s = @"h:[1,1/4,3,2,1.4](left!)->[2,3/4,1,1](up!small!)";
            var holds = Hold.CreateHolds(s);

            var stds = new[] {
                new Hold {
                    Header = new NoteHeader{ Measure = 1, Nominator = 1, Denominator = 4, Start = 3, End = 2, Speed = 1.4f },
                    Body = new HoldBody { Direction = "left", Size = string.Empty }
                },
                new Hold {
                    Header = new NoteHeader{ Measure = 2, Nominator = 3, Denominator = 4, Start = 1, End = 1, Speed = 1 },
                    Body = new HoldBody { Direction = "up", Size = "small" }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], holds[i]);
            }
        }

    }
}
