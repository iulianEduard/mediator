using System;
using System.Globalization;

namespace TransactionsProcessor.CFN.Application.Core.Extensions
{
    public static class GlobalExtensions
    {
        public static DateTime ToDateTime(this int yyMMdd)
        {
            var customFormat = yyMMdd.ToString();

            var finalDateTime = DateTime.ParseExact(customFormat, "yyMMdd", CultureInfo.InvariantCulture);

            return finalDateTime;
        }

        public static DateTime ToDateTime(this int hhMM, DateTime dateTime)
        {
            var hhMMString = hhMM.ToString();

            hhMMString = hhMMString.Length == 3 ? $"0{hhMMString}" : (hhMMString.Length == 2 ? $"00{hhMMString}" : hhMMString);

            DateTime finalDateTime;

            try
            {
                var timeSpan = TimeSpan.ParseExact(hhMMString, "hhmm", CultureInfo.InvariantCulture);

                finalDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeSpan.Hours, timeSpan.Minutes, 0);
            }
            catch (Exception ex)
            {
                throw;
            }

            return finalDateTime;
        }

        public static decimal DecimalSum(params decimal?[] decimalArray)
        {
            decimal result = 0;

            for (int i = 0; i <= decimalArray.Length - 1; i++)
            {
                result += decimalArray[i] ?? 0;
            }

            return result;
        }
    }
}
