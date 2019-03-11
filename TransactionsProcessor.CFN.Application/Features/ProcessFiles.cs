using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Models;
using TransactionsProcessor.Infrastructure.Processors;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class ProcessFiles
    {
        public class Command : IRequest<Result>
        {
            public string FullName { get; set; }
        }

        public class Result
        {
            public Dictionary<int, CfnFileModel> CfnRecordsDictonary { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnProcessor _cfnProcessor;

            public Handler(ICfnProcessor cfnProcessor)
            {
                _cfnProcessor = cfnProcessor;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var parseResponse = _cfnProcessor.ParseFile(request.FullName);
                var cfnRecords = Mapper.Map<List<CfnFileModel>>(parseResponse);

                var result = new Result
                {
                    CfnRecordsDictonary = ToDictionary(cfnRecords)
                };

                await Task.CompletedTask; // TODO: this must be changed

                return result;
            }

            private static Dictionary<int, CfnFileModel> ToDictionary(List<CfnFileModel> cfnRecords)
            {
                var cfnDictionary = new Dictionary<int, CfnFileModel>();
                var index = 0;

                foreach(var cfnRecord in cfnRecords)
                {
                    cfnDictionary.Add(index, cfnRecord);
                    index++;
                }

                return cfnDictionary;
            }
        }

    }
}
