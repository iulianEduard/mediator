using System.Collections.Generic;
using TransactionsProcessor.Infrastructure.Processors.Templates;

namespace TransactionsProcessor.Infrastructure.Processors
{
    public interface ICfnProcessor
    {
        List<CfnProcessorTemplate> ParseFile(string fullName);
    }

    public class CfnProcessor : ImportProcessor<CfnProcessorTemplate>, ICfnProcessor
    {
        public List<CfnProcessorTemplate> ParseFile(string fullName)
        {
            ReadRecordsFromFile(fullName);

            return RecordList;
        }
    }
}
