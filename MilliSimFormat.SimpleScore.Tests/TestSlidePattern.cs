using MilliSimFormat.SimpleScore.Internal;
using NUnit.Framework;

namespace MilliSimFormat.SimpleScore.Tests {
    [TestFixture]
    public sealed class TestSlidePattern {

        [Test]
        public void Slide_SimplePairWithoutParentheses() {
            var s = @"s:[5,1/4,4]->[6,4/4,5]";
            var slides = Slide.CreateSlides(s);

            var stds = new[] {
                new Slide {
                    Header = new NoteHeader{ Measure = 5, Nominator = 1, Denominator = 4, Start = 4, End = 4, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                },
                new Slide {
                    Header = new NoteHeader{ Measure = 6, Nominator = 4, Denominator = 4, Start = 5, End = 5, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], slides[i]);
            }
        }

        [Test]
        public void Slide_SimplePair() {
            var s = @"s:[1,1/4,3]()->[2,3/4,1,1]()";
            var slides = Slide.CreateSlides(s);

            var stds = new[] {
                new Slide {
                    Header = new NoteHeader{ Measure = 1, Nominator = 1, Denominator = 4, Start = 3, End = 3, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                },
                new Slide {
                    Header = new NoteHeader{ Measure = 2, Nominator = 3, Denominator = 4, Start = 1, End = 1, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], slides[i]);
            }
        }

        [Test]
        public void Slide_SimpleTriplet() {
            var s = @"s:[1,1/4,3]()->[2,3/4,1,1]()->[3,1/3,5,5,2]()";
            var slides = Slide.CreateSlides(s);

            var stds = new[] {
                new Slide {
                    Header = new NoteHeader { Measure = 1, Nominator = 1, Denominator = 4, Start = 3, End = 3, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                },
                new Slide {
                    Header = new NoteHeader { Measure = 2, Nominator = 3, Denominator = 4, Start = 1, End = 1, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                },
                new Slide {
                    Header = new NoteHeader { Measure = 3, Nominator = 1, Denominator = 3, Start = 5, End = 5, Speed = 2 },
                    Body = new SlideBody { Direction = string.Empty, Size = string.Empty }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], slides[i]);
            }
        }

        [Test]
        public void Slide_AdvancedTriplet() {
            var s = @"s:[1,1/4,3](left!)->[2,3/4,1,1](large!)->[3,1/3,5,5,2](up!large!)";
            var slides = Slide.CreateSlides(s);

            var stds = new[] {
                new Slide {
                    Header = new NoteHeader { Measure = 1, Nominator = 1, Denominator = 4, Start = 3, End = 3, Speed = 1 },
                    Body = new SlideBody { Direction = "left", Size = string.Empty }
                },
                new Slide {
                    Header = new NoteHeader { Measure = 2, Nominator = 3, Denominator = 4, Start = 1, End = 1, Speed = 1 },
                    Body = new SlideBody { Direction = string.Empty, Size = "large" }
                },
                new Slide {
                    Header = new NoteHeader { Measure = 3, Nominator = 1, Denominator = 3, Start = 5, End = 5, Speed = 2 },
                    Body = new SlideBody { Direction = "up", Size = "large" }
                }
            };

            for (var i = 0; i < stds.Length; ++i) {
                Assert.AreEqual(stds[i], slides[i]);
            }
        }

    }
}
