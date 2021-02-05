using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;

using Redcap.Broker;
using Redcap.Models;
using Redcap.Services;

using Tynamix.ObjectFiller;

namespace Tests
{
    public partial class RedcapApiTests
    {
        private readonly IApiService apiService;
        private readonly Mock<IApiBroker> apiBrokerMock;
        private string apiUri = "http://localhost:8080/redcap";
        public RedcapApiTests()
        {
            apiBrokerMock = new Mock<IApiBroker>();
            apiService = new ApiService(apiBroker: apiBrokerMock.Object);
        }
        private static Demographic CreateDemographicsInstrument()
        {
            return CreateDemographicsInstrumentFiller().Create();
        }
        private static Filler<Demographic> CreateDemographicsInstrumentFiller()
        {
            var filler = new Filler<Demographic>();
            return filler;
        }
    }
}
