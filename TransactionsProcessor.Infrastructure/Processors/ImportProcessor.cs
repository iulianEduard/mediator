using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TransactionsProcessor.Infrastructure.Processors.Templates;

namespace TransactionsProcessor.Infrastructure.Processors
{
    public abstract class ImportProcessor<T> where T : BaseImportProcessorTemplate
    {
        public List<T> RecordList { get; set; }

        public void ReadRecordsFromFile(string file)
        {
            RecordList = new List<T>();

            try
            {
                var engine = new FileHelperEngine<T>();
                RecordList = engine.ReadFile(file).ToList();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
