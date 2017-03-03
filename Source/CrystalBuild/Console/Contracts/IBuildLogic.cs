namespace CarbonCore.Applications.CrystalBuild.CSharp.Contracts
{
    using System.Collections.Generic;
    
    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.CrystalBuild.Sharp.Logic;
    using CarbonCore.Utils.IO;

    public interface IBuildLogic
    {
        void BuildProjectFile(IList<CarbonFile> sources, CarbonDirectory projectRoot);
    }
}
