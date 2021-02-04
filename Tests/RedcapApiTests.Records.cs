using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using Redcap.Broker;
using Redcap.Models;
using Redcap.Services;

using RestSharp;

using Xunit;

namespace Tests
{
    public partial class RedcapApiTests
    {
        [Fact]
        public async Task ShouldExportRecordAsync()
        {
            // arrange
            Demographic demographicInstrument = CreateDemographicsInstrument();
            Demographic inputDemographicInstrument = demographicInstrument;
            Demographic retrievedDemographicInstrument = inputDemographicInstrument;
            Demographic expectedDemographicInstrument = retrievedDemographicInstrument;
            var request = new RestRequest(apiUri, Method.POST);
            apiBrokerMock.Setup(broker => broker.ExecuteAsync<Demographic>(request))
                .ReturnsAsync(retrievedDemographicInstrument);
            // act
            Demographic actualDemographicInstrument = await apiService.ExportRecordAsync<Demographic>();

            // assert
            actualDemographicInstrument.Should().BeEquivalentTo(expectedDemographicInstrument);
        }
    }
}
