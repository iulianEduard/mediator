using AutoMapper;
using FileHelpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Parse
{
    public partial class Parse
    {
        public class Command : IRequest<Result>
        {
            public string FullName { get; set; }
        }

        public class Result
        {
            // TODO: Remodel for TransactionsGuidID

            public List<ParseModel> Transactions { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IMapper _mapper;

            public Handler()
            {
                _mapper = ConfigMapper();
            }

            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var parseResult = new List<Parse.Template>();

                try
                {
                    var engine = new FileHelperEngine<Parse.Template>();
                    parseResult = engine.ReadFile(request.FullName).ToList();
                }
                catch (Exception ex)
                {
                    // TODO: log exception and exit
                }

                var cfnRecords = _mapper.Map<List<ParseModel>>(parseResult);

                var result = new Result
                {
                    Transactions = cfnRecords
                };

                return Task.FromResult<Result>(result);
            }

            private IMapper ConfigMapper()
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Template, ParseModel>()
                        .ForMember(dest => dest.TransactionUID, opts => opts.MapFrom(src => Guid.NewGuid()))
                        .ForMember(dest => dest.DeliveryId, opts => opts.Ignore())
                        .ForMember(dest => dest.NameOnCard, opts => opts.Ignore());
                });

                return config.CreateMapper();
            }
        }
    }
}
