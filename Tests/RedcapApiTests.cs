using Moq;

using Redcap;
using Redcap.Interfaces;
using Redcap.Models;

using Tynamix.ObjectFiller;

namespace Tests
{
    public partial class RedcapApiTests
    {
        private readonly RedcapApi _sut;
        private readonly Mock<IRedcap> _redcapApiMock = new Mock<IRedcap>();
        private readonly static string _apiToken = "SOME_TOKEN";
        private string apiUri = "http://localhost/redcap";
        public RedcapApiTests()
        {
            _sut = new RedcapApi(apiUri);
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
