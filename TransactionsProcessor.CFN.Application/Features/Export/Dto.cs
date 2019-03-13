using System;
using System.Collections.Generic;
using System.Text;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Export
{
    public partial class Export
    {
        public class ExportModel
        {
            public long? Ticket { get; set; }

            public decimal TotalGallons { get; set; }

            public string ProductType { get; set; }

            public string Date { get; set; }

            public string TransactionType { get; set; }

            public string CustomerName { get; set; }

            public decimal? UnitCost { get; set; }

            public decimal? Margin { get; set; }

            public decimal? Taxes { get; set; }

            public decimal? TotalPPG { get; set; }

            public decimal TicketPrice { get; set; }

            public string FileDate { get; set; }
        }

        public class Request
        {
            public string FullName { get; set; }

            public IEnumerable<ExportModel> ExportTransactions { get; set; }
        }

        public class Customer
        {
            public string CardId { get; set; }

            public int CustomerId { get; set; }

            public string NameOnCard { get; set; }
        }

        public class Context
        {
            public List<ParseModel> ParseTransactions { get; set; }

            public ICfnDatabase Database { get; set; }
        }
    }
}
