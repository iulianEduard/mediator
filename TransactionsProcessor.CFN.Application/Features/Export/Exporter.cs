using FileHelpers;
using System.Collections.Generic;

namespace TransactionsProcessor.CFN.Application.Features.Export
{
    public static class Exporter
    {
        public static void ToCsv(Export.Request request)
        {
            FileHelperEngine engine = new FileHelperEngine(typeof(Export.ExportModel));
            
            var csv = new List<Export.ExportModel>();
            foreach (var item in request.ExportTransactions)
            {
                 csv.Add(item);
            }

            engine.HeaderText = "Ticket,TotalGallons,ProductType,Date,TransactionType,CustomerName,UnitCost,Margin,Taxes,TotalPPG,TicketPrice,FileDate";
            engine.WriteFile(request.FullName, csv);
        }
    }
}
