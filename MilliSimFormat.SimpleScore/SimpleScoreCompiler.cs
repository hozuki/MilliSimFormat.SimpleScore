using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace MilliSimFormat.SimpleScore {
    public sealed class SimpleScoreCompiler : IScoreCompiler {

        internal SimpleScoreCompiler() {
        }

        public void Dispose() {
        }

        public RuntimeScore Compile(SourceScore score, ScoreCompileOptions compileOptions) {
            compileOptions.Offset = score.MusicOffset;
            return ScoreCompileHelper.CompileScore(score, compileOptions);
        }

    }
}
