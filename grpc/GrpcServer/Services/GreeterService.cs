using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using GrpcGreeting;

namespace GrpcServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            this._logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            string message = this.Greet(request);
            return Task.FromResult(new HelloReply { Message = message });
        }

        private string Greet(HelloRequest request)
        {
            switch (request.Language)
            {
                case "en":
                    switch (request.Friendliness)
                    {
                        case 1: return $"Oh no. It's {request.Name}.";
                        case 2: return $"Hello {request.Name}";
                        case 3: return $"YAY! It's {request.Name}! Amazing!";
                    }
                    break;
            }
            return $"Sorry, I don't know how to greet people in {request.Language}";
        }
    }
}
