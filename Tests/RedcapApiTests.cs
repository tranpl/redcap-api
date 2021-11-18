
using Moq;

using Redcap.Broker;
using Redcap.Models;
using Redcap.Services;

using Tynamix.ObjectFiller;

namespace Tests
{
    public partial class RedcapApiTests
    {
        private readonly IApiService sut;
        private readonly Mock<IApiBroker> apiBrokerMock;
        private string apiUri = "http://localhost/redcap";
        public RedcapApiTests()
        {
            apiBrokerMock = new Mock<IApiBroker>();
            sut = new ApiService(apiBroker: apiBrokerMock.Object);
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
