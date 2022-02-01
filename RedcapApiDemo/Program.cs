using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tynamix.ObjectFiller;

namespace RedcapApiDemo
{
    /// <summary>
    /// A class that represents the demopgrahics form for the demographics template.
    /// Add additional properties that you've added to the redcap instrument as needed.
    /// </summary>
    public class Demographic
    {
        [JsonRequired]
        [JsonProperty("record_id")]
        public string RecordId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("bio")]
        public string Bio { get; set; }

        /// <summary>
        /// Test file uploads
        /// </summary>
        [JsonProperty("upload_file")]
        public string UploadFile { get; set; }
    }
    class Program
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
            "Hethrir",
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
        static async Task Main(string[] args)
        {
            /*
             * This is a demo. This program provides a demonstration of potential calls using the API library.
             * 
             * This program runs through all the APIs methods sequentially.
             * 
             * Directions:
             * 
             * 1. Go into your Redcap instance and create a new project with demographics form.
             * 2. Turn on longitudinal and add two additional event. Event name should be "Event 1, Event 2, Event 3"
             *  Important, make sure you designate the instrument to atleast one event
             * 3. Create a folder in C: , name it redcap_download_files
             * 4. Create a text file in that folder, save it as test.txt
             * 5. Add a field in the project that was created; the field type should contain file upload, name it "protocol_upload"
             * This allows the upload file method to upload files
             * 
             */
            await InitializeDemo();
        }
        static async Task InitializeDemo()
        {
            /*
             * Change this token for your demo project
             * Using one created from a local dev instance
             */
            string _token = string.Empty;
            /*
             * Change this token for your demo project
             * Using one created from a local dev instance
            */
            string _superToken = "2E59CA118ABC17D393722524C501CF0BAC51689746E24BFDAF47B38798BD827A";
            /*
             * Using a local redcap development instsance
             */
            string _uri = string.Empty;
            var fieldName = "file_upload";
            var eventName = "event_1_arm_1";

            /*
            * Output to console
            */
            Console.WriteLine("Starting Redcap Api Demo..");
            Console.WriteLine("Please make sure you include a working redcap api token.");
            Console.WriteLine("Enter your redcap instance uri, example: http://localhost/redcap");
            _uri = Console.ReadLine();
            if (string.IsNullOrEmpty(_uri))
            {
                // provide a default one here..
                _uri = "http://localhost/redcap";
            }
            _uri = _uri + "/api/";
            Console.WriteLine("Enter your api token for the project to test: ");
            var token = Console.ReadLine();

            if (string.IsNullOrEmpty(token))
            {
                _token = "DF70F2EC94AE05021F66423B386095BD";
            }
            else
            {
                _token = token;
            }
            Console.WriteLine($"Using Endpoint=> {_uri} Token => {_token}");

            Console.WriteLine("-----------------------------Starting API Version 1.0.5+-------------");
            Console.WriteLine("Starting demo for API Version 1.0.0+");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine("Creating a new instance of RedcapApi");
            var redcap_api_2_0_0 = new RedcapApi(_uri);
            Console.WriteLine($"Using {_uri.ToString()} for redcap api endpoint.");

            #region ExportLoggingAsync()
            Console.WriteLine("Calling ExportLoggingAsync() . . .");
            Console.WriteLine($"Exporting logs for User . . .");
            var ExportLoggingAsync = await redcap_api_2_0_0.ExportLoggingAsync(_token, Content.Log, ReturnFormat.json, LogType.User);
            Console.WriteLine($"ExportLoggingAsync Results: {JsonConvert.DeserializeObject(ExportLoggingAsync)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion

            #region ImportDagsAsync()
            Console.WriteLine("Calling ImportDagsAsync() . . .");
            Console.WriteLine($"Importing Dags . . .");
            var dags = CreateDags(5);
            var ImportDagsAsyncResult = await redcap_api_2_0_0.ImportDagsAsync(_token, Content.Dag, RedcapAction.Import, ReturnFormat.json, dags);
            Console.WriteLine($"ImportDagsAsync Results: {JsonConvert.DeserializeObject(ImportDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #endregion

            #region ExportDagsAsync()
            Console.WriteLine("Calling ExportDagsAsync() . . .");
            Console.WriteLine($"Exporting Dags . . .");
            var ExportDagsAsyncResult = await redcap_api_2_0_0.ExportDagsAsync(_token, Content.Dag, ReturnFormat.json);
            Console.WriteLine($"ExportDagsAsync Results: {JsonConvert.DeserializeObject(ExportDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion

            #region DeleteDagsAsync()
            Console.WriteLine("Calling DeleteDagsAsync() . . .");
            Console.WriteLine($"Deleting Dags . . .");
            var dagsToDelete = JsonConvert.DeserializeObject<List<RedcapDag>>(ExportDagsAsyncResult).Select(x => x.UniqueGroupName).ToArray();
            var DeleteDagsAsyncResult = await redcap_api_2_0_0.DeleteDagsAsync(_token, Content.Dag, RedcapAction.Delete, dagsToDelete);
            Console.WriteLine($"DeleteDagsAsync Results: {JsonConvert.DeserializeObject(DeleteDagsAsyncResult)}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion

            #region ImportRecordsAsync()
            Console.WriteLine("Calling ImportRecordsAsync() . . .");
            // get demographics data
            var importDemographicsData = CreateDemographics(includeBio: true, 5);
            Console.WriteLine("Serializing the data . . .");
            Console.WriteLine($"Importing record {string.Join(",", importDemographicsData.Select(x => x.RecordId).ToList())} . . .");
            var ImportRecordsAsync = await redcap_api_2_0_0.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, importDemographicsData, "MDY", CsvDelimiter.tab, ReturnContent.count, OnErrorFormat.json);
            var ImportRecordsAsyncData = JsonConvert.DeserializeObject(ImportRecordsAsync);
            Console.WriteLine($"ImportRecordsAsync Result: {ImportRecordsAsyncData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion ImportRecordsAsync()

            #region ExportRecordsAsync()
            Console.WriteLine($"Calling ExportRecordsAsync()");
            Console.WriteLine($"Using records from the imported method..");
            var recordsToExport = importDemographicsData.Select(x => x.RecordId).ToArray();
            var instrumentName = new string[] { "demographics" };
            var ExportRecordsAsyncResult = await redcap_api_2_0_0.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, recordsToExport, null, instrumentName);
            Console.WriteLine($"ExportRecordsAsyncResult: {ExportRecordsAsyncResult}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion ExportRecordsAsync()

            #region DeleteRecordsAsync()
            Console.WriteLine("Calling DeleteRecordsAsync() . . .");
            var records = importDemographicsData.Select(x => x.RecordId).ToArray();
            Console.WriteLine($"Deleting record {string.Join(",", recordsToExport)} . . .");
            var DeleteRecordsAsync = await redcap_api_2_0_0.DeleteRecordsAsync(_token, Content.Record, RedcapAction.Delete, recordsToExport, 1);
            var DeleteRecordsAsyncData = JsonConvert.DeserializeObject(DeleteRecordsAsync);
            Console.WriteLine($"DeleteRecordsAsync Result: {DeleteRecordsAsyncData}");

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion DeleteRecordsAsync()

            #region RenameRecordAsync()
            Console.WriteLine("Calling RenameRecordAsync() . . .");
            var recordToRename = importDemographicsData.Select(x => x.RecordId).SingleOrDefault();
            Console.WriteLine($"Renaming record {recordToRename} . . .");
            var newRecordName = "2";
            var RenameRecordAsyncResult = await redcap_api_2_0_0.RenameRecordAsync(_token, recordToRename, newRecordName, Content.Record, RedcapAction.Rename, 1);
            var RenameRecordAsyncData = JsonConvert.DeserializeObject(RenameRecordAsyncResult);
            Console.WriteLine($"RenameRecordAsync Result: {DeleteRecordsAsyncData}");

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion RenameRecordAsync()



            #region Users & User Priveleges
            Console.WriteLine("Calling ImportUsersAsync() . . .");
            var redcapUser1 = CreateRedcapUser("test1");
            var redcapUser2 = CreateRedcapUser("test2");
            var redcapUsers = new List<RedcapUser>();
            redcapUsers.Add(redcapUser1);
            redcapUsers.Add(redcapUser2);
            Console.WriteLine($"Importing  {redcapUsers.Count} user. . .");
            var ImportUsersAsyncResult = await redcap_api_2_0_0.ImportUsersAsync(_token, redcapUsers, ReturnFormat.json, OnErrorFormat.json);
            var ImportUsersAsyncData = JsonConvert.DeserializeObject(ImportUsersAsyncResult);
            Console.WriteLine($"ImportUsersAsync Result: {ImportUsersAsyncData}");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();
            #endregion  Users & User Priveleges


            #region ExportArmsAsync()
            var arms = new string[] { };
            Console.WriteLine("Calling ExportArmsAsync()");
            var ExportArmsAsyncResult = await redcap_api_2_0_0.ExportArmsAsync(_token, Content.Arm, ReturnFormat.json, arms, OnErrorFormat.json);
            Console.WriteLine($"ExportArmsAsyncResult: {JsonConvert.DeserializeObject(ExportArmsAsyncResult)}");
            #endregion ExportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportArmsAsync()
            var ImportArmsAsyncData = CreateArms(count: 3);
            Console.WriteLine("Calling ImportArmsAsync()");
            var ImportArmsAsyncResult = await redcap_api_2_0_0.ImportArmsAsync(_token, Content.Arm, Override.False, RedcapAction.Import, ReturnFormat.json, ImportArmsAsyncData, OnErrorFormat.json);
            Console.WriteLine($"ImportArmsAsyncResult: {JsonConvert.DeserializeObject(ImportArmsAsyncResult)}");
            #endregion ImportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteArmsAsync()
            var DeleteArmsAsyncData = ImportArmsAsyncData.Select(x => x.ArmNumber).ToArray();
            Console.WriteLine("Calling DeleteArmsAsync()");
            var DeleteArmsAsyncResult = await redcap_api_2_0_0.DeleteArmsAsync(_token, Content.Arm, RedcapAction.Delete, DeleteArmsAsyncData);
            Console.WriteLine($"DeleteArmsAsyncResult: {JsonConvert.DeserializeObject(DeleteArmsAsyncResult)}");
            #endregion DeleteArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportEventsAsync()
            var ExportEventsAsyncData = new string[] { "1" };
            Console.WriteLine("Calling ExportEventsAsync()");
            var ExportEventsAsyncResult = await redcap_api_2_0_0.ExportEventsAsync(_token, Content.Event, ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json);
            Console.WriteLine($"ExportEventsAsyncResult: {JsonConvert.DeserializeObject(ExportEventsAsyncResult)}");
            #endregion ExportEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportEventsAsync()
            Console.WriteLine("Calling ExportEventsAsync()");
            var eventList = new List<RedcapEvent> {
                new RedcapEvent {
                    EventName = "baseline",
                    ArmNumber = "1",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "baseline_arm_1",
                    CustomEventLabel = "hello baseline"
                },
                new RedcapEvent {
                    EventName = "clinical",
                    ArmNumber = "1",
                    DayOffset = "1",
                    MinimumOffset = "0",
                    MaximumOffset = "0",
                    UniqueEventName = "clinical_arm_1",
                    CustomEventLabel = "hello clinical"
                }
            };
            var ImportEventsAsyncResult = await redcap_api_2_0_0.ImportEventsAsync(_token, Content.Event, RedcapAction.Import, Override.False, ReturnFormat.json, eventList, OnErrorFormat.json);
            Console.WriteLine($"ImportEventsAsyncResult: {ImportEventsAsyncResult}");
            #endregion ImportEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region DeleteEventsAsync()
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };
            Console.WriteLine("Calling DeleteEventsAsync()");
            var DeleteEventsAsyncResult = await redcap_api_2_0_0.DeleteEventsAsync(_token, Content.Event, RedcapAction.Delete, DeleteEventsAsyncData);
            Console.WriteLine($"DeleteEventsAsyncResult: {DeleteEventsAsyncResult}");
            #endregion DeleteEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region SwitchDagAsync()
            var SwitchDagAsyncData = new RedcapDag { GroupName = "testGroup", UniqueGroupName = "unique_name" };
            Console.WriteLine("Calling SwitchDagAsync()");
            var SwitchDagAsyncResult = await redcap_api_2_0_0.SwitchDagAsync(_token, SwitchDagAsyncData, Content.Dag, RedcapAction.Switch);
            Console.WriteLine($"SwitchDagAsyncResult: {SwitchDagAsyncResult}");
            #endregion SwitchDagAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();



            #region ExportFieldNamesAsync()
            Console.WriteLine("Calling ExportFieldNamesAsync(), first_name");
            var ExportFieldNamesAsyncResult = await redcap_api_2_0_0.ExportFieldNamesAsync(_token, Content.ExportFieldNames, ReturnFormat.json, "first_name", OnErrorFormat.json);
            Console.WriteLine($"ExportFieldNamesAsyncResult: {ExportFieldNamesAsyncResult}");
            #endregion ExportFieldNamesAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region ImportFileAsync()
            var recordId = "1";
            var fileName = "Demographics_TestProject_DataDictionary.csv";
            DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            string parentDirectory = myDirectory.Parent.FullName;
            var parent = Directory.GetParent(parentDirectory).FullName;
            var filePath = Directory.GetParent(parent).FullName + @"\Docs\";
            Console.WriteLine($"Calling ImportFileAsync(), {fileName}");
            var ImportFileAsyncResult = await redcap_api_2_0_0.ImportFileAsync(_token, Content.File, RedcapAction.Import, recordId, fieldName, eventName, null, fileName, filePath, OnErrorFormat.json);
            Console.WriteLine($"ImportFileAsyncResult: {ImportFileAsyncResult}");
            #endregion ImportFileAsync()


            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, not save the file.");
            var ExportFileAsyncResult = await redcap_api_2_0_0.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, OnErrorFormat.json);
            Console.WriteLine($"ExportFileAsyncResult: {ExportFileAsyncResult}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            var filedDownloadPath = @"C:\redcap_download_files";
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, saving the file.");
            var ExportFileAsyncResult2 = await redcap_api_2_0_0.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, OnErrorFormat.json, filedDownloadPath);
            Console.WriteLine($"ExportFileAsyncResult2: {ExportFileAsyncResult2}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteFileAsync()
            Console.WriteLine($"Calling DeleteFileAsync(), deleting file: {fileName} for field: {fieldName}");
            var DeleteFileAsyncResult = await redcap_api_2_0_0.DeleteFileAsync(_token, Content.File, RedcapAction.Delete, recordId, fieldName, eventName, "1", OnErrorFormat.json);
            Console.WriteLine($"DeleteFileAsyncResult: {DeleteFileAsyncResult}");
            #endregion DeleteFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportInstrumentsAsync()
            Console.WriteLine($"Calling DeleteFileAsync()");
            var ExportInstrumentsAsyncResult = await redcap_api_2_0_0.ExportInstrumentsAsync(_token, Content.Instrument, ReturnFormat.json);
            Console.WriteLine($"ExportInstrumentsAsyncResult: {ExportInstrumentsAsyncResult}");
            #endregion ExportInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), returns raw");
            var ExportPDFInstrumentsAsyncResult = await redcap_api_2_0_0.ExportPDFInstrumentsAsync(_token, Content.Pdf, recordId, eventName, "demographics", true);
            Console.WriteLine($"ExportInstrumentsAsyncResult: {JsonConvert.SerializeObject(ExportPDFInstrumentsAsyncResult)}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), saving pdf file to {filedDownloadPath}");
            var ExportPDFInstrumentsAsyncResult2 = await redcap_api_2_0_0.ExportPDFInstrumentsAsync(_token, recordId, eventName, "demographics", true, filedDownloadPath, OnErrorFormat.json);
            Console.WriteLine($"ExportPDFInstrumentsAsyncResult2: {ExportPDFInstrumentsAsyncResult2}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            //#region ExportInstrumentMappingAsync()
            //Console.WriteLine($"Calling ExportInstrumentMappingAsync()");
            //var armsToExportInstrumentMapping = arms;
            //var ExportInstrumentMappingAsyncResult = await redcap_api_1_1_0.ExportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, armsToExportInstrumentMapping, OnErrorFormat.json);
            //Console.WriteLine($"ExportInstrumentMappingAsyncResult: {ExportInstrumentMappingAsyncResult}");
            //#endregion ExportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportInstrumentMappingAsync()
            var importInstrumentMappingData = new List<FormEventMapping> { new FormEventMapping { arm_num = "1", unique_event_name = "clinical_arm_1", form = "demographics" } };
            Console.WriteLine($"Calling ImportInstrumentMappingAsync()");
            var ImportInstrumentMappingAsyncResult = await redcap_api_2_0_0.ImportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, importInstrumentMappingData, OnErrorFormat.json);
            Console.WriteLine($"ImportInstrumentMappingAsyncResult: {ImportInstrumentMappingAsyncResult}");
            #endregion ImportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportMetaDataAsync()
            Console.WriteLine($"Calling ExportMetaDataAsync()");
            var ExportMetaDataAsyncResult = await redcap_api_2_0_0.ExportMetaDataAsync(_token, Content.MetaData, ReturnFormat.json, null, null, OnErrorFormat.json);
            Console.WriteLine($"ExportMetaDataAsyncResult: {ExportMetaDataAsyncResult}");
            #endregion ExportMetaDataAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportMetaDataAsync()
            /*
             * This imports 1 field into the data dictionary
             */
            var importMetaData = new List<RedcapMetaData> { new RedcapMetaData { field_name = "first_name", form_name = "demographics", field_type = "text", field_label = "First Name" } };
            Console.WriteLine($"Not calling ImportMetaDataAsync(), still change data dictionary to include 1 field");
            //var ImportMetaDataAsyncResult = redcap_api_1_0_7.ImportMetaDataAsync(_token, "metadata", ReturnFormat.json, importMetaData, OnErrorFormat.json);
            //Console.WriteLine($"ImportMetaDataAsyncResult: {ImportMetaDataAsyncResult}");
            #endregion ImportMetaDataAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region CreateProjectAsync()
            var projectData = new List<RedcapProject> { new RedcapProject { project_title = "Amazing Project ", purpose = ProjectPurpose.Other, purpose_other = "Test" } };
            Console.WriteLine($"Calling CreateProjectAsync(), creating a new project with Amazing Project as title, purpose 1 (other) ");
            Console.WriteLine($"-----------------------Notice the use of SUPER TOKEN------------------------");
            var CreateProjectAsyncResult = await redcap_api_2_0_0.CreateProjectAsync(_superToken, Content.Project, ReturnFormat.json, projectData, OnErrorFormat.json, null);
            Console.WriteLine($"CreateProjectAsyncResult: {CreateProjectAsyncResult}");
            #endregion CreateProjectAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportProjectInfoAsync()
            var projectInfo = new RedcapProjectInfo { ProjectTitle = "Updated Amazing Project ", Purpose = ProjectPurpose.QualityImprovement, SurveysEnabled = 1 };
            Console.WriteLine($"Calling ImportProjectInfoAsync()");
            var ImportProjectInfoAsyncResult = await redcap_api_2_0_0.ImportProjectInfoAsync(_token, Content.ProjectSettings, ReturnFormat.json, projectInfo);
            Console.WriteLine($"ImportProjectInfoAsyncResult: {ImportProjectInfoAsyncResult}");
            #endregion ImportProjectInfoAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportProjectInfoAsync()
            Console.WriteLine($"Calling ExportProjectInfoAsync()");
            var ExportProjectInfoAsyncResult = await redcap_api_2_0_0.ExportProjectInfoAsync(_token, Content.ProjectSettings, ReturnFormat.json);
            Console.WriteLine($"ExportProjectInfoAsyncResult: {ExportProjectInfoAsyncResult}");
            #endregion ExportProjectInfoAsync()

            Console.WriteLine("----------------------------Demo completed! Press Enter to Exit-------------");
            Console.ReadLine();

        }
        public static Demographic GetRandomPerson(string id, bool includeBio = false)
        {
            var person = new Demographic();
            person.RecordId = id;
            person.FirstName = Names[rand.Next(Names.Length)];
            person.LastName = Places[rand.Next(Places.Length)];
            if (includeBio)
            {
                person.Bio = VeryLargeText;
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
                    _demographic.Bio = VeryLargeText;
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
        public static List<RedcapDag> CreateDags(int count = 1){

            var dags = new List<RedcapDag>();
            for(var i = 0; i < count; i++)
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
