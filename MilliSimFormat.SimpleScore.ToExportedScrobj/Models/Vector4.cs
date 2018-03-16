using System;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj.Models {
    [Serializable]
    public struct Vector4 {

        public Vector4(float x, float y, float z, float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float X, Y, Z, W;

    }
}
