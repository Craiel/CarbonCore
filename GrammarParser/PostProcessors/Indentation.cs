namespace CarbonCore.GrammarParser.PostProcessors
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.PostProcessing;

    public class Indentation : BasePostProcessor<IndentationData, IndentationInstruction, Line>
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override IndentationData PrepareData(IndentationInstruction instruction)
        {
            return new IndentationData
                           {
                               IncreaseAt = new HashSet<char>(instruction.IncreaseAt),
                               DecreaseAt = new HashSet<char>(instruction.DecreaseAt),
                               IndentationChar = instruction.IndentationChar,
                               IndentationAmount = instruction.IndentationAmount
                           };
        }

        protected override bool ProcessLine(IndentationData data, Line line)
        {
            if (line.IsEmpty)
            {
                return false;
            }

            int indentationAmount = 0;
            if (data.Level > 0)
            {
                indentationAmount = data.Level * data.IndentationAmount;
            }

            if (data.IncreaseAt.Contains(line.TrimmedCurrent[0]))
            {
                data.Level++;
            }

            if (data.DecreaseAt.Contains(line.TrimmedCurrent[0]))
            {
                data.Level--;

                // Decrease by one to force the char in the correct indentation
                indentationAmount -= data.IndentationAmount;
            }

            if (indentationAmount <= 0)
            {
                return false;
            }

            line.Processed = string.Concat(new string(data.IndentationChar, indentationAmount), line.Current);
            line.UpdateMode = LineUpdateMode.Update;

            return true;
        }
    }
}
