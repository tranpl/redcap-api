using Redcap;
using Redcap.Models;

using RedcapApiDemo.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Tynamix.ObjectFiller;

namespace RedcapApiDemo
{
    partial class Program
    {
        static Random rand = new Random();
        public const string VeryLargeText = @"If I don't make it back, you're the only hope for the Alliance. Luke, don't talk that way. You have a power I--I don't understand and could never have. You're wrong, Leia. You have that power too. In time you'll learn to use it as I have. The Force is strong in my family. My father has it... I have it ...and...my sister has it. Yes. It's you Leia. I know. Somehow...I've always known. Then you know why I have to face him.
I told you to remain on the command ship. A small Rebel force has penetrated the shield and landed on Endor. Yes, I know. My son is with them. Are you sure? I have felt him, my Master. Strange, that I have not. I wonder if your feelings on this matter are clear, Lord Vader. They are clear, my Master. Then you must go to the Sanctuary Moon and wait for them. He will come to me? I have foreseen it. His compassion for you will be his undoing. He will come to you andthen you will bring him before me. As you wish.
Rise, my friend. The Death Star will be completed on schedule. You have done well, Lord Vader. And now I sense you wish to continue your search for young Skywalker. Yes, my Master. Patience, my friend. In time he will seek you out. And when he does, you must bring him before me. He has grown strong. Only together can we turn him to the dark side of the Force. As you wish. Everything is proceeding as I have foreseen.
Greetings, Exalted One. Allow me to introduce myself. I am Luke Skywalker, Jedi Knight and friend to Captain Solo. I know that you are powerful, mighty Jabba, and that your anger with Solo must be equally powerful. I seek an audience with Your Greatness to bargain for Solo's life. With your wisdom, I'm sure that we can work out an arrangement which will be mutually beneficial and enable us to avoid any unpleasant confrontation. As a token of my goodwill, I present to you a gift: these two droids. What did he say? Both are hardworking and will serve you well.
Oh, General Solo, somebody's coming. Oh! Luke! Where's Leia? What? She didn't come back? I thought she was with you. We got separated. Hey, we better go look for her. Take the squad ahead. We'll meet at the shield generator at 0300. Come on, Artoo. We'll need your scanners. Don't worry, Master Luke. We know what to do. And you said it was pretty here. Ugh!
Where is that shuttle going? Shuttle Tydirium, what is your cargo and destination? Parts and technical crew for the forest moon. Do they have a code clearance? It's an older code, sir, but it checks out. I was about to clear them. Shall I hold them? No. Leave them to me. I will deal with them myself. As you wish, my lord. Carry on. They're not goin' for it, Chewie. Shuttle Tydirium, deactivation of the shield will commence immediately. Follow your present course. Okay! I told you it was gonna work. No problem.
Hmm. That face you make. Look I so old to young eyes? No... of course not. I do, yes, I do! Sick have I become. Old and weak. When nine hundred years old you reach, look as good you will not. Hmm? Soon will I rest. Yes, forever sleep. Earned it, I have. Master Yoda, you can't die. Strong am I with the Force... but not that strong! Twilight is upon me and soon night must fall. That is the way of things... the way of the Force. But I need your help. I've come back to complete the training. No more training do you require. Already know you that which you need. Then I am a Jedi? Ohhh. Not yet. One thing remains: Vader. You must confront Vader. Then, only then, a Jedi will you be. And confront him you will.
Command station, this is ST 321. Code Clearance Blue. We're starting our approach. Deactivate the security shield. The security deflector shield will be deactivated when we have confirmation of your code transmission. Stand by... You are clear to proceed. We're starting our approach. Inform the commander that Lord Vader's shuttle has arrived. Yes, sir.
Not bad for a little furball. There's only one left. You stay here. We'll take care of this. I have decided that we shall stay here.
You can see here the Death Star orbiting the forest Moon of Endor. Although the weapon systems on this Death Star are not yet operational, the Death Star does have a strong defense mechanism. It is protected by an energy shield, which is generated from the nearby forest Moon of Endor. The shield must be deactivated if any attack is to be attempted. Once the shield is down, our cruisers will create a perimeter, while the fighters fly into the superstructure and attempt to knock out the main reactor. General Calrissian has volunteered to lead the fighter attack";
        public static string[] Places =
        {
            "Drall",
            "Ylesia",
            "Hoth",
            "Almania",
            "Duro",
            "Selonia",
            "Talus",
            "Mon Calamari",
            "Agamar",
            "Borleias",
            "Fondor",
            "Kalarba",
            "Antar Four",
            "Bespin",
            "Munto Codru",
            "Carratos",
            "J't'p'tan",
            "Bakura",
            "Pydyr",
            "N'zoth",
            "Dantooine",
            "Abregado-rae",
            "Lwhekk",
            "Teyr",
            "Dagobah",
            "Firrerre",
            "Aquaris",
            "Etti IV",
            "Carida",
            "Wayland",
        };
        public static string[] Names =
        {
            "Owen Lars",
            "Dannik Jerriko",
            "Emperor's Royal Guards",
            "Tusken Raiders",
            "Bollux",
            "Sy Snootles",
            "Hethril",
            "Tessek",
            "Beru Lars",
            "Moruth Doole",
            "Momaw Nadon",
            "Tenel Ka",
            "Muftak",
            "Gartogg",
            "Princess Kneesaa",
            "Wedge Antilles",
            "Qwi Xux",
            "Lady Valarian",
            "Cindel Towani",
            "Vima-Da-Boda",
            "Nomi Sunrider",
            "Admiral Ackbar",
            "IG-88",
            "Ulic Qel-Droma",
            "Rillao",
            "Brea Tonnika",
            "General Crix Madine",
            "Chewbacca",
            "Dengar",
            "Talon Karrde"
        };

        static async Task RunAdditionalMethodWiringAsync(RedcapApi redcap_api, string token, string superToken, string recordId, string eventName, string fileDownloadPath)
        {
            Console.WriteLine("Running additional wiring examples...");

            var instrument = "demographics";
            var reportId = 1;

            _ = await redcap_api.ExportUserDagAssignmentAsync(token, Content.UserDagMapping, RedcapFormat.json, RedcapReturnFormat.json);

            _ = await redcap_api.ExportUsersAsync(token, RedcapFormat.json, RedcapReturnFormat.json);
            _ = await redcap_api.ExportUsersAsync(token, Content.User, RedcapFormat.json, RedcapReturnFormat.json);

            _ = await redcap_api.DeleteUsersAsync(token, new List<string> { "test1", "test2" }, Content.User, RedcapAction.Delete);

            _ = await redcap_api.ExportUserRolesAsync(token, Content.UserRole, RedcapFormat.json, RedcapReturnFormat.json);
            _ = await redcap_api.ExportUserRoleAssignmentAsync(token, Content.UserRoleMapping, RedcapFormat.json, RedcapReturnFormat.json);
            _ = await redcap_api.DeleteUserRolesAsync(token, new List<string> { "test role" }, Content.UserRole, RedcapAction.Delete);

            _ = await redcap_api.ExportInstrumentMappingAsync(token, RedcapFormat.json, new[] { "1" }, RedcapReturnFormat.json);

            _ = await redcap_api.ExportRecordAsync(token, Content.Record, recordId, RedcapFormat.json, RedcapDataType.flat, fields: new[] { "record_id" }, forms: new[] { "demographics" }, events: new[] { eventName });

            _ = await redcap_api.ExportProjectXmlAsync(token, Content.ProjectXml, returnMetadataOnly: true, records: new[] { recordId }, fields: new[] { "record_id" }, events: new[] { eventName }, returnFormat: RedcapReturnFormat.json, exportSurveyFields: true, exportDataAccessGroups: false, filterLogic: null, exportFiles: false);

            _ = await redcap_api.GenerateNextRecordNameAsync(token);

            _ = await redcap_api.ExportReportsAsync(token, reportId, RedcapFormat.json, RedcapReturnFormat.json);

            _ = await redcap_api.ExportRedcapVersionAsync(token, Content.Version, RedcapFormat.json);

            _ = await redcap_api.ExportSurveyLinkAsync(token, recordId, instrument, eventName, 1, RedcapReturnFormat.json);
            _ = await redcap_api.ExportSurveyAccessCodeAsync(token, recordId, instrument, eventName, 1, RedcapReturnFormat.json);
            _ = await redcap_api.ExportSurveyParticipantsAsync(token, instrument, eventName, RedcapFormat.json, RedcapReturnFormat.json);
            _ = await redcap_api.ExportSurveyQueueLinkAsync(token, recordId, RedcapReturnFormat.json);
            _ = await redcap_api.ExportSurveyReturnCodeAsync(token, recordId, instrument, eventName, "1", RedcapReturnFormat.json);

            _ = await redcap_api.ExportRepeatingInstrumentsAndEvents(token, RedcapFormat.json);

            _ = await redcap_api.CreateFolderFileRepositoryAsync(token, Content.FileRepository, RedcapAction.CreateFolder, "demo-folder", RedcapFormat.json, returnFormat: RedcapReturnFormat.json);
            _ = await redcap_api.ExportFilesFoldersFileRepositoryAsync(token, Content.FileRepository, RedcapAction.List, RedcapFormat.json, folderId: null, RedcapReturnFormat.json);
            _ = await redcap_api.ImportFileRepositoryAsync(token, Content.FileRepository, RedcapAction.Import, file: "demo-file-content", folderId: null, returnFormat: RedcapReturnFormat.json);
            _ = await redcap_api.ExportFileFileRepositoryAsync(token, Content.FileRepository, RedcapAction.Export, docId: "1", returnFormat: RedcapReturnFormat.json);
            _ = await redcap_api.DeleteFileRepositoryAsync(token, Content.FileRepository, RedcapAction.Delete, docId: "1", returnFormat: RedcapReturnFormat.json);

            _ = superToken;
            _ = fileDownloadPath;

            Console.WriteLine("Completed additional wiring examples.");
        }

        public static Demographic GetRandomPerson(string id, bool includeBio = false)
        {
            var person = new Demographic();
            person.RecordId = id;
            person.FirstName = Names[rand.Next(Names.Length)];
            person.LastName = Places[rand.Next(Places.Length)];
            if(includeBio)
            {
                person.Bio = VeryLargeText;
            }
            return person;
        }

        public static List<Demographic> CreateDemographics(bool includeBio = false, int count = 1)
        {
            var demographics = new List<Demographic>();
            for(var i = 1; i <= count; i++)
            {
                demographics.Add(GetRandomPerson(i.ToString(), includeBio));
            }

            return demographics;
        }

        public static List<RedcapArm> CreateArms(int count = 1)
        {
            var arms = new List<RedcapArm>();
            for(var i = 0; i < count; i++)
            {
                var armFiller = new Filler<RedcapArm>();
                armFiller.Setup().OnProperty(x => x.ArmNumber).Use(i.ToString());
                var arm = armFiller.Create();
                arms.Add(arm);
            }

            return arms;
        }

        public static List<RedcapDag> CreateDags(int count = 1)
        {
            var dags = new List<RedcapDag>();
            for(var i = 0; i < count; i++)
            {
                var dagFiller = new Filler<RedcapDag>();
                dagFiller.Setup().OnProperty(x => x.UniqueGroupName).Use(string.Empty);
                var dag = dagFiller.Create();
                dags.Add(dag);
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
