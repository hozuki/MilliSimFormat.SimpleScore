using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using OpenMLTD.MilliSim.Core.Entities.Extending;

namespace MilliSimFormat.SimpleScore {
    [Export(typeof(IScoreFormat))]
    public sealed class SimpleScoreFormat : ScoreFormat {

        public override IScoreReader CreateReader() {
            return new SimpleScoreReader();
        }

        public override IScoreWriter CreateWriter() {
            throw new NotSupportedException();
        }

        public override IScoreCompiler CreateCompiler() {
            return new SimpleScoreCompiler();
        }

        public override bool SupportsReadingFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return SupportedReadExtensions.Any(ext => fileName.EndsWith(ext));
        }

        public override bool CanReadAsSource => true;

        public override bool CanReadAsCompiled => true;

        public override bool CanBeCompiled => true;

        public override bool CanWriteSource => false;

        public override bool CanWriteCompiled => false;

        public override IReadOnlyList<string> SupportedReadExtensions => FormatExtensions;

        public override IReadOnlyList<string> SupportedWriteExtensions => null;

        public override string FormatDescription => "Simple score";

        public override string PluginID => "plugin.score.mltd.simple_score";

        public override string PluginName => "Simple Score Format";

        public override string PluginDescription => "Simple score format.";

        public override string PluginAuthor => "hozuki";

        public override Version PluginVersion => MyVersion;

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

        private static readonly string[] FormatExtensions = { ".ss" };

    }
}
