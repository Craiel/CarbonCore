namespace CarbonCore.Applications.CrystalBuild.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Applications.CrystalBuild.Logic;
    using CarbonCore.Utils.Compat.IO;

    public interface IBuildLogic
    {
        void Build(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context);
        void BuildTemplates(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context);
        void BuildData(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context);
        void BuildStyleSheets(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context);
        void BuildImages(IList<CarbonFileResult> sources, ProcessingContext context);
        void CopyContents(IList<CarbonFileResult> sources, CarbonDirectory target);
    }
}
