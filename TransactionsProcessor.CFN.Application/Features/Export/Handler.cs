using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Models;
using TransactionsProcessor.FileManager;

namespace TransactionsProcessor.CFN.Application.Features.Export
{
    public partial class Export
    {
        public class Command : IRequest<Result>
        {
            public string ContentType { get; set; }

            public List<ParseModel> ParseModel { get; set; }

            public Guid ProcessId { get; set; }
        }

        public class Result
        {

        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _database;
            private readonly IFileManagerFileConfigurations _fileConfigurations;

            public Handler(ICfnDatabase database, IFileManagerFileConfigurations fileConfigurations)
            {
                _database = database;
                _fileConfigurations = fileConfigurations;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var customerCards = await _database.Query<Export.Customer>("cfn.usp_GetCustomerCards");
                var fileSettings = await _fileConfigurations.GetFileConfiguration(request.ContentType);
                var exportData = Mapper.Map(request.ParseModel, customerCards);

                var csvRequest = new Export.Request
                {
                    FullName = Path.Combine(fileSettings.FolderPath, fileSettings.FileName),
                    ExportTransactions = exportData
                };

                Exporter.ToCsv(csvRequest);

                return await Task.FromResult(new Result());
            }
        }
    }
}
