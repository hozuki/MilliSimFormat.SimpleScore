﻿using System;

namespace MilliSimFormat.SimpleScore.ToExportedScrobj.Models {
    [Serializable]
    public sealed class EventScenarioData {

        public double absTime;

        public bool selected;

        public long tick;

        public int measure;

        public int beat;

        public int track;

        public ScenarioNoteType type;

        public int param;

        public int target;

        public long duration;

        public string str = string.Empty;

        public string info = string.Empty;

        public int on;

        public int on2;

        public ColorRGBA col;

        public ColorRGBA col2;

        public float[] cols;

        public object tex;

        public int texInx = -1;

        public int trig;

        public float speed;

        public int idol;

        public bool[] mute;

        public bool addf;

        public float eye_x;

        public float eye_y;

        public Vector4[] formation;

        public bool appeal;

        public int cheeklv;

        public bool eyeclose;

        public bool talking;

        public bool delay;

        public int[] clratio;

        public int[] clcols;

        public int camcut = -1;

        public VjParam vjparam;

    }
}
