using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionsProcessor.CFN.Application.Helpers;
using TransactionsProcessor.CFN.Application.Models;
using static TransactionsProcessor.CFN.Application.Features.Export.Export;

namespace TransactionsProcessor.CFN.Application.Features.Export
{
    public static class Mapper
    {
        public static IEnumerable<ExportModel> Map(IEnumerable<ParseModel> records, IEnumerable<Export.Customer> customerCards)
        {
            string GetNameOnCard(string cardId)
            {
                var nameOnCard = customerCards.Where(cc => cc.CardId == cardId).Select(cc => cc.NameOnCard).FirstOrDefault();
                return string.IsNullOrWhiteSpace(nameOnCard) ? "Not found" : nameOnCard;
            }

            return records.Select(item => new ExportModel
            {
                Ticket = item.TransactionNumber,
                TotalGallons = item.Quantity,
                ProductType = item.ProductDescription,
                Date = item.DateCompleted.ToDateTime().ToShortDateString(),
                TransactionType = item.TransactionLocationIndicator,
                CustomerName = GetNameOnCard(item.CardId.ToString()),
                UnitCost = item.PumpPrice,
                Margin = item.HaulRate,
                Taxes = Extensions.DecimalSum(item.SiteTaxAmount1, item.SiteTaxAmount2, item.SiteTaxAmount3,
                        item.SiteTaxAmount4, item.SiteTaxAmount5, item.SiteTaxAmount6,
                        item.SiteTaxAmount7, item.SiteTaxAmount8, item.SiteTaxAmount9, item.SiteTaxAmount10),
                TotalPPG = item.CFNPrice,
                TicketPrice = item.TotalAmount,
                FileDate = DateTime.Now.ToString()
            });
        }
    }
}
