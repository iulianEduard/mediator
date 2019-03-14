using System;
using System.Collections.Generic;
using System.Linq;
using TransactionsProcessor.CFN.Application.Core.Extensions;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Export
{
    public static class Mapper
    {
        public static IEnumerable<Export.ExportModel> Map(IEnumerable<ParseModel> records, IEnumerable<Export.Customer> customerCards)
        {
            string GetNameOnCard(string cardId)
            {
                var nameOnCard = customerCards.Where(cc => cc.CardId == cardId).Select(cc => cc.NameOnCard).FirstOrDefault();
                return string.IsNullOrWhiteSpace(nameOnCard) ? "Not found" : nameOnCard;
            }

            return records.Select(item => new Export.ExportModel
            {
                Ticket = item.TransactionNumber,
                TotalGallons = item.Quantity,
                ProductType = item.ProductDescription,
                Date = item.DateCompleted.ToDateTime().ToShortDateString(),
                TransactionType = item.TransactionLocationIndicator,
                CustomerName = GetNameOnCard(item.CardId.ToString()),
                UnitCost = item.PumpPrice,
                Margin = item.HaulRate,
                Taxes = item.ToTaxes(),
                TotalPPG = item.CFNPrice,
                TicketPrice = item.TotalAmount,
                FileDate = DateTime.Now.ToString()
            });
        }
    }

    public static class MapperExtensions
    {
        public static decimal ToTaxes(this ParseModel parseModel)
        {
            return GlobalExtensions.DecimalSum(parseModel.SiteTaxAmount1, parseModel.SiteTaxAmount2, parseModel.SiteTaxAmount3,
                        parseModel.SiteTaxAmount4, parseModel.SiteTaxAmount5, parseModel.SiteTaxAmount6,
                        parseModel.SiteTaxAmount7, parseModel.SiteTaxAmount8, parseModel.SiteTaxAmount9, parseModel.SiteTaxAmount10);
        }
    }
}
