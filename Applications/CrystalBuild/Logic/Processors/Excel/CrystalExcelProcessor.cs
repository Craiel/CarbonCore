namespace CarbonCore.Applications.CrystalBuild.Logic.Processors.Excel
{
    using System.Collections.Generic;

    using CarbonCore.Processing.Processors.Excel;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts.Processors;
    
    public class CrystalExcelProcessor : ContentProcessor, IExcelProcessor
    {
        private readonly IDictionary<BuildTargetPlatform, IExcelFormatter> formatterTargetLookup;

        private readonly IList<ExcelData> data;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CrystalExcelProcessor()
        {
            this.formatterTargetLookup = new Dictionary<BuildTargetPlatform, IExcelFormatter>
                                             {
                                                 { BuildTargetPlatform.Java, new ExcelFormatterJava() },
                                                 { BuildTargetPlatform.Unity, new ExcelFormatterUnity() }
                                             };

            this.data = new List<ExcelData>();
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoProcess(CarbonFile source)
        {
            if (!this.formatterTargetLookup.ContainsKey(this.Context.TargetPlatform))
            {
                Diagnostic.Warning("No Excel Formatter for target Platform {0}", this.Context.TargetPlatform);
                return;
            }

            ExcelData sourceData = ExcelProcessor.Process(source);
            if (sourceData != null)
            {
                this.data.Add(sourceData);
            }
        }
        
        protected override string PostProcessData(string localData)
        {
            IExcelFormatter formatter;
            if (this.data == null 
                || !this.formatterTargetLookup.TryGetValue(this.Context.TargetPlatform, out formatter))
            {
                return null;
            }
            
            return formatter.Format(this.data);
        }
    }
}
