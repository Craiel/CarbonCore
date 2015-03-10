namespace CarbonCore.Applications.CrystalBuild.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public interface IBuildLogic
    {
        void Build(IList<CarbonFileResult> sources, CarbonFile target, bool isDebug = false);
        void BuildTemplates(IList<CarbonFileResult> sources, CarbonFile target);
        void BuildData(IList<CarbonFileResult> sources, CarbonFile target);
        void BuildStyleSheets(IList<CarbonFileResult> sources, CarbonFile target);
        void CopyContents(IList<CarbonFileResult> sources, CarbonDirectory target);
    }
}
