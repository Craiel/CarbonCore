namespace CarbonCore.GrammarParser.PostProcessing
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.Contracts;
    using CarbonCore.GrammarParser.Contracts.PostProcessing;

    public abstract class BasePostProcessor<T, TN, TL> : IPostProcessor
        where T : ProcessingData
        where TN : ProcessingInstruction
        where TL : class, ILine
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<string> Process(ProcessingInstruction instruction)
        {
            var data = this.PrepareData((TN)instruction);
            if (!this.PrepareProcess(data, (TN)instruction))
            {
                return null;
            }

            TL line = null;
            for (int i = 0; i < instruction.Data.Count; i++)
            {
                if (line != null && line.UpdateMode == LineUpdateMode.Abort)
                {
                    return null;
                }

                string next = i < instruction.Data.Count - 1 ? instruction.Data[i + 1] : null;
                TL previousLine = line;
                line = (TL)Activator.CreateInstance(typeof(TL), instruction.Data[i], next, i);
                line.Previous = previousLine != null ? line.Current : null;

                // Defaults first, then normal
                this.ProcessLine(data, line);

                if (line.UpdateMode == LineUpdateMode.Abort)
                {
                    return null;
                }

                this.BeforeFinalize(data);
                this.FinalizeLine(data, line);
            }

            this.FinalizeProcess(data, (TN)instruction);
            return data.Processed;
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual T PrepareData(TN instruction)
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        protected virtual void BeforeFinalize(T data)
        {
        }

        protected virtual bool PrepareProcess(T data, TN instruction)
        {
            return true;
        }

        protected virtual void FinalizeProcess(T data, TN instruction)
        {
        }
        
        protected virtual void FinalizeLine(T data, TL line)
        {
            switch (line.UpdateMode)
            {
                case LineUpdateMode.None:
                    {
                        line.ProcessedLineNumber = data.Processed.Count;
                        data.Processed.Add(line.Current);
                        break;
                    }

                case LineUpdateMode.Update:
                    {
                        line.ContentBegin = data.Processed.Count;

                        if (line.PrecedingContent != null)
                        {
                            foreach (string content in line.PrecedingContent)
                            {
                                data.Processed.Add(content);
                            }
                        }

                        line.ProcessedLineNumber = data.Processed.Count;
                        this.AddProcessedData(data, line.Processed);

                        if (line.SubsequentContent != null)
                        {
                            foreach (string content in line.SubsequentContent)
                            {
                                data.Processed.Add(content);
                            }
                        }

                        line.ContentEnd = data.Processed.Count;

                        break;
                    }

                case LineUpdateMode.Skip:
                    {
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        protected virtual bool ProcessLine(T data, TL line)
        {
            return false;
        }

        protected virtual void AddProcessedData(T data, string processed)
        {
            // Separate out the individual lines if needed, processing can add content
            if (processed.Contains(Environment.NewLine))
            {
                string[] lines = processed.Split(
                    new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string lineEntry in lines)
                {
                    data.Processed.Add(lineEntry);
                }
            }
            else
            {
                data.Processed.Add(processed);
            }
        }
    }
}
