using System;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj.Models {
    [Serializable]
    public sealed class NoteScrObj {

        public EventNoteData[] evts = {
            new EventNoteData {
                absTime = 1,
                beat = 4,
                duration = 1000
            },
            new EventNoteData {
                poly = new [] {
                    new PolyPoint {
                        posx = 5.5f,
                        subtick = 999
                    }
                }
            }
        };

        public EventConductorData[] ct = {
            new EventConductorData()
        };

        public float[] scoreSpeed = new float[10];

        public float[] judgeRange = new float[20];

        public float BGM_offset;

    }
}
