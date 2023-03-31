using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Redcap.Models;
using RedcapApiDemo.Models;
using Tynamix.ObjectFiller;

namespace RedcapApiDemo.Utilities
{
    public class RedcapDemoUtilities
    {
        static Random rand = new Random();
        public static Demographic GetRandomPerson(string id, bool includeBio = false)
        {
            var person = new Demographic();
            person.RecordId = id;
            person.FirstName = RedcapTestVariables.Names[rand.Next(RedcapTestVariables.Names.Length)];
            person.LastName = RedcapTestVariables.Places[rand.Next(RedcapTestVariables.Places.Length)];
            if (includeBio)
            {
                person.Bio = RedcapTestVariables.VeryLargeText;
            }
            return person;
        }
        public static List<Demographic> CreateDemographics(bool includeBio = false, int count = 1)
        {
            var demographics = new List<Demographic>();
            for (var i = 1; i <= count; i++)
            {
                var _demographicFiller = new Filler<Demographic>();

                _demographicFiller.Setup().OnProperty(x => x.RecordId).Use(i.ToString());
                var _demographic = _demographicFiller.Create();
                if (includeBio)
                {
                    _demographic.Bio = RedcapTestVariables.VeryLargeText;
                }
                demographics.Add(_demographic);
            }

            return demographics;
        }
        public static List<RedcapArm> CreateArms(int count = 1)
        {

            var arms = new List<RedcapArm>();
            for (var i = 0; i < count; i++)
            {
                var _demographicFiller = new Filler<RedcapArm>();
                _demographicFiller.Setup().OnProperty(x => x.ArmNumber).Use(i.ToString());
                var _demographic = _demographicFiller.Create();
                arms.Add(_demographic);
            }
            return arms;
        }
        public static List<RedcapDag> CreateDags(int count = 1)
        {

            var dags = new List<RedcapDag>();
            for (var i = 0; i < count; i++)
            {
                var _dagFiller = new Filler<RedcapDag>();
                _dagFiller.Setup().OnProperty(x => x.UniqueGroupName).Use(string.Empty);
                var _dag = _dagFiller.Create();
                dags.Add(_dag);
            }
            return dags;
        }
        public static RedcapUser CreateRedcapUser(string username)
        {
            return new RedcapUser
            {
                Username = username,
                Expiration = "",
                DataAccessGroup = "",
                Design = "1",
                UserRights = "1",
                DataAccessGroups = "1",
                DataExport = "1",
                Reports = "1",
                StatsAndCharts = "1",
                ManageSurveyParticipants = "1",
                Calendar = "1",
                DataImportTool = "1",
                DataComparisonTool = "1",
                Logging = "1",
                FileRepository = "1",
                DataQualityCreate = "1",
                DataQualityExecute = "1",
                ApiExport = "1",
                ApiImport = "1",
                MobileApp = "1",
                MobileAppDownloadData = "1",
                RecordCreate = "1",
                LockRecordsCustomization = "1"

            };
        }

    }
}
