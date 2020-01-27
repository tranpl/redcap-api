using Newtonsoft.Json;
using Redcap;
using Redcap.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedcapApiDemo
{
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
        public static void Main(string[] args)
        {
            /*
             * This is a demo. This program provides a demonstration of potential calls using the API library.
             * 
             * This program sequently runs through all the APIs methods.
             * 
             * Directions:
             * 
             * 1. Go into Redcap and create a new project with demographics.
             * 2. Turn on longitudinal and add two additional event. Event name should be "Event 1, Event 2, Event 3"
             *  Important, make sure you designate the instrument to atleast one event
             * 3. Create a folder in C: , name it redcap_download_files
             * 4. Create a text file in that folder, save it as test.txt
             * 5. Add a field, field type file upload to the project, name it "protocol_upload"
             * This allows the upload file method to upload files
             * 
             */
            InitializeDemo();

            /*
             * Start a new instance of Redcap APi
             */
            //var redcapApi = new RedcapApi(_token, _uri);

            //Console.WriteLine("Calling API Methods < 1.0.0");

            //Console.WriteLine("Calling GetRecordAsync() . . .");
            //var GetRecordAsync = redcapApi.GetRecordAsync("1", ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json, null, null, null, null).Result;
            //var GetRecordAsyncData = JsonConvert.DeserializeObject(GetRecordAsync);
            //Console.WriteLine($"GetRecordAsync Result: {GetRecordAsyncData}");

            //Console.WriteLine("Calling ExportEventsAsync() . . .");
            //var exportEvents = redcapApi.ExportEventsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            //var exportEventsAsync = JsonConvert.DeserializeObject(exportEvents);
            //Console.WriteLine($"ExportEventsAsync Result: {exportEventsAsync}");

            //Console.WriteLine("Calling GetRecordsAsync() . . .");
            //var GetRecordsAsync = redcapApi.GetRecordsAsync(ReturnFormat.json, OnErrorFormat.json, RedcapDataType.flat).Result;
            //var GetRecordsAsyncData = JsonConvert.DeserializeObject(GetRecordsAsync);
            //Console.WriteLine($"GetRecordsAsync Result: {GetRecordsAsyncData}");

            //Console.WriteLine("Calling GetRedcapVersionAsync() . . .");
            //var GetRedcapVersionAsync = redcapApi.GetRedcapVersionAsync(ReturnFormat.json, RedcapDataType.flat).Result;
            //Console.WriteLine($"GetRedcapVersionAsync Result: {GetRedcapVersionAsync}");


            //var saveRecordsAsyncObject = new
            //{
            //    record_id = "1",
            //    redcap_event_name = "event_1_arm_1",
            //    first_name = "John",
            //    last_name = "Doe"
            //};

            //Console.WriteLine("Calling SaveRecordsAsync() . . .");
            //var SaveRecordsAsync = redcapApi.SaveRecordsAsync(saveRecordsAsyncObject, ReturnContent.ids, OverwriteBehavior.overwrite, ReturnFormat.json, RedcapDataType.flat, OnErrorFormat.json).Result;
            //var SaveRecordsAsyncData = JsonConvert.DeserializeObject(SaveRecordsAsync);
            //Console.WriteLine($"SaveRecordsAsync Result: {SaveRecordsAsyncData}");


            //Console.WriteLine("Calling ExportRecordsAsync() . . .");
            //var ExportRecordsAsync = redcapApi.ExportRecordsAsync(_token).Result;
            //var ExportRecordsAsyncData = JsonConvert.DeserializeObject(ExportRecordsAsync);
            //Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncData}");

            //Console.WriteLine("Calling ExportArmsAsync() . . .");
            //var ExportArmsAsync = redcapApi.ExportArmsAsync(ReturnFormat.json, OnErrorFormat.json).Result;
            //var ExportArmsAsyncData = JsonConvert.DeserializeObject(ExportArmsAsync);
            //Console.WriteLine($"ExportArmsAsync Result: {ExportArmsAsyncData}");

            //Console.WriteLine("Calling ExportRecordsAsync() . . .");
            //var ExportRecordsAsync2 = redcapApi.ExportRecordsAsync(_token, Content.Record).Result;
            //var ExportRecordsAsyncdata = JsonConvert.DeserializeObject(ExportRecordsAsync2);
            //Console.WriteLine($"ExportRecordsAsync Result: {ExportRecordsAsyncdata}");

            //var listOfEvents = new List<RedcapEvent>() {
            //    new RedcapEvent{arm_num = "1", custom_event_label = null, event_name = "Event 1", day_offset = "1", offset_min = "0", offset_max = "0", unique_event_name = "event_1_arm_1" }
            //};
            //Console.WriteLine("Calling ImportEventsAsync() . . .");
            //var ImportEventsAsync = redcapApi.ImportEventsAsync(listOfEvents, Override.False, ReturnFormat.json, OnErrorFormat.json).Result;
            //var ImportEventsAsyncData = JsonConvert.DeserializeObject(ImportEventsAsync);
            //Console.WriteLine($"ImportEventsAsync Result: {ImportEventsAsyncData}");

            //var pathImport = "C:\\redcap_download_files";
            //string importFileName = "test.txt";
            //var pathExport = "C:\\redcap_download_files";
            //var record = "1";
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";
            //var repeatingInstrument = "1";

            //Console.WriteLine("Calling ImportFile() . . .");
            //var ImportFile = redcapApi.ImportFileAsync(_uri, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, importFileName, pathImport, OnErrorFormat.json).Result;
            //Console.WriteLine($"File has been imported! To verify, field history!");

            //Console.WriteLine("Calling ExportFile() . . .");
            //var ExportFile = redcapApi.ExportFileAsync(_uri, Content.File, RedcapAction.Import, record, fieldName, eventName, repeatingInstrument, OnErrorFormat.json, pathExport).Result;
            //Console.WriteLine($"ExportFile Result: {ExportFile} to : {pathExport}");

            //Console.WriteLine("Calling DeleteFile() . . .");
            //var DeleteFile = redcapApi.DeleteFileAsync("1", "protocol_upload", "event_1_arm_1", "", OnErrorFormat.json).Result;
            //Console.WriteLine($"File has been deleted! To verify, field history!");

            //Console.WriteLine("Calls to < 1.0.0 completed...");
            //// Make a sound!
            //Console.Beep();





        }
        static void InitializeDemo()
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
            string _superToken = "92F719F0EC97783D06B0E0FF49DC42DDA2247BFDC6759F324EE0D710FCA87C33";
            /*
             * Using a local redcap development instsance
             */
            string _uri = string.Empty;
            var fieldName = "protocol_upload";
            var eventName = "event_1_arm_1";

            /*
                * Output to console
                */
            Console.WriteLine("Starting Redcap Api Demo..");
            Console.WriteLine("Please make sure you include a working redcap api token.");
            Console.WriteLine("Enter your redcap instance uri, example: http://localhost/redcap");
            _uri = Console.ReadLine();
            _uri = _uri + "/api/";
            Console.WriteLine("Enter your api token for the project to test: ");
            var token = Console.ReadLine();
            _token = token;

            Console.WriteLine("-----------------------------Starting API Version 1.0.5+-------------");
            Console.WriteLine("Starting demo for API Version 1.0.0+");
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine("Creating a new instance of RedcapApi");
            var redcap_api_1_0_7 = new RedcapApi(_uri);

            Console.WriteLine($"Using {_uri.ToString()} for redcap api endpoint.");

            #region ImportRecordsAsync()
            Console.WriteLine("Calling ImportRecordsAsync() . . .");
            /*
             * Create a list of object of type instrument or fields. Add its properties then add it to the list.
             * record_id is required
             */
            var data = new List<Demographic> {
                new Demographic {
                    FirstName = "Jon", LastName = "Doe", RecordId = "1"
                }
            };
            Console.WriteLine($"Importing record {string.Join(",", data.Select(x => x.RecordId).ToList())} . . .");
            var ImportRecordsAsync = redcap_api_1_0_7.ImportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, OverwriteBehavior.normal, false, data, "MDY", ReturnContent.count, OnErrorFormat.json).Result;
            var ImportRecordsAsyncData = JsonConvert.DeserializeObject(ImportRecordsAsync);
            Console.WriteLine($"ImportRecordsAsync Result: {ImportRecordsAsyncData}");
            #endregion ImportRecordsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteRecordsAsync()
            Console.WriteLine("Calling DeleteRecordsAsync() . . .");
            var records = new string[] { "1" };
            Console.WriteLine($"Deleting record {string.Join(",", records)} . . .");
            var DeleteRecordsAsync = redcap_api_1_0_7.DeleteRecordsAsync(_token, Content.Record, RedcapAction.Delete, records, 1).Result;

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            Console.WriteLine($"DeleteRecordsAsync Result: {DeleteRecordsAsync}");
            #endregion DeleteRecordsAsync()

            #region ExportArmsAsync()
            var arms = new string[] { };
            Console.WriteLine("Calling ExportArmsAsync()");
            var ExportArmsAsyncResult = redcap_api_1_0_7.ExportArmsAsync(_token, Content.Arm, ReturnFormat.json, arms, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportArmsAsyncResult: {ExportArmsAsyncResult}");
            #endregion ExportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportArmsAsync()
            var ImportArmsAsyncData = new List<RedcapArm> { new RedcapArm { ArmNumber = "1", Name = "hooo" }, new RedcapArm { ArmNumber = "2", Name = "heee" }, new RedcapArm { ArmNumber = "3", Name = "hawww" } };
            Console.WriteLine("Calling ImportArmsAsync()");
            var ImportArmsAsyncResult = redcap_api_1_0_7.ImportArmsAsync(_token, Content.Arm, Override.False, RedcapAction.Import, ReturnFormat.json, ImportArmsAsyncData, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportArmsAsyncResult: {ImportArmsAsyncResult}");
            #endregion ImportArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteArmsAsync()
            var DeleteArmsAsyncData = new string[] { "3" };
            Console.WriteLine("Calling DeleteArmsAsync()");
            var DeleteArmsAsyncResult = redcap_api_1_0_7.DeleteArmsAsync(_token, Content.Arm, RedcapAction.Delete, DeleteArmsAsyncData).Result;
            Console.WriteLine($"DeleteArmsAsyncResult: {DeleteArmsAsyncResult}");
            #endregion DeleteArmsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportEventsAsync()
            var ExportEventsAsyncData = new string[] { "1" };
            Console.WriteLine("Calling ExportEventsAsync()");
            var ExportEventsAsyncResult = redcap_api_1_0_7.ExportEventsAsync(_token, Content.Event, ReturnFormat.json, ExportEventsAsyncData, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportEventsAsyncResult: {ExportEventsAsyncResult}");
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
            var ImportEventsAsyncResult = redcap_api_1_0_7.ImportEventsAsync(_token, Content.Event, RedcapAction.Import, Override.False, ReturnFormat.json, eventList, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportEventsAsyncResult: {ImportEventsAsyncResult}");
            #endregion ImportEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region DeleteEventsAsync()
            var DeleteEventsAsyncData = new string[] { "baseline_arm_1" };
            Console.WriteLine("Calling DeleteEventsAsync()");
            var DeleteEventsAsyncResult = redcap_api_1_0_7.DeleteEventsAsync(_token, Content.Event, RedcapAction.Delete, DeleteEventsAsyncData).Result;
            Console.WriteLine($"DeleteEventsAsyncResult: {DeleteEventsAsyncResult}");
            #endregion DeleteEventsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region ExportFieldNamesAsync()
            Console.WriteLine("Calling ExportFieldNamesAsync(), first_name");
            var ExportFieldNamesAsyncResult = redcap_api_1_0_7.ExportFieldNamesAsync(_token, Content.ExportFieldNames, ReturnFormat.json, "first_name", OnErrorFormat.json).Result;
            Console.WriteLine($"ExportFieldNamesAsyncResult: {ExportFieldNamesAsyncResult}");
            #endregion ExportFieldNamesAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();


            #region ImportFileAsync()
            var recordId = "1";
            var fileName = "test.txt";
            var fileUploadPath = @"C:\redcap_upload_files";
            Console.WriteLine($"Calling ImportFileAsync(), {fileName}");
            var ImportFileAsyncResult = redcap_api_1_0_7.ImportFileAsync(_token, Content.File, RedcapAction.Import, recordId, fieldName, eventName, null, fileName, fileUploadPath, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportFileAsyncResult: {ImportFileAsyncResult}");
            #endregion ImportFileAsync()


            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, not save the file.");
            var ExportFileAsyncResult = redcap_api_1_0_7.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportFileAsyncResult: {ExportFileAsyncResult}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportFileAsync()
            var filedDownloadPath = @"C:\redcap_download_files";
            Console.WriteLine($"Calling ExportFileAsync(), {fileName} for field name {fieldName}, saving the file.");
            var ExportFileAsyncResult2 = redcap_api_1_0_7.ExportFileAsync(_token, Content.File, RedcapAction.Export, recordId, fieldName, eventName, null, OnErrorFormat.json, filedDownloadPath).Result;
            Console.WriteLine($"ExportFileAsyncResult2: {ExportFileAsyncResult2}");
            #endregion ExportFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region DeleteFileAsync()
            Console.WriteLine($"Calling DeleteFileAsync(), deleting file: {fileName} for field: {fieldName}");
            var DeleteFileAsyncResult = redcap_api_1_0_7.DeleteFileAsync(_token, Content.File, RedcapAction.Delete, recordId, fieldName, eventName, "1", OnErrorFormat.json).Result;
            Console.WriteLine($"DeleteFileAsyncResult: {DeleteFileAsyncResult}");
            #endregion DeleteFileAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportInstrumentsAsync()
            Console.WriteLine($"Calling DeleteFileAsync()");
            var ExportInstrumentsAsyncResult = redcap_api_1_0_7.ExportInstrumentsAsync(_token, Content.Instrument, ReturnFormat.json).Result;
            Console.WriteLine($"ExportInstrumentsAsyncResult: {ExportInstrumentsAsyncResult}");
            #endregion ExportInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), returns raw");
            var ExportPDFInstrumentsAsyncResult = redcap_api_1_0_7.ExportPDFInstrumentsAsync(_token, Content.Pdf, recordId, eventName, "demographics", true, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportInstrumentsAsyncResult: {JsonConvert.SerializeObject(ExportPDFInstrumentsAsyncResult)}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportPDFInstrumentsAsync()
            Console.WriteLine($"Calling ExportPDFInstrumentsAsync(), saving pdf file to {filedDownloadPath}");
            var ExportPDFInstrumentsAsyncResult2 = redcap_api_1_0_7.ExportPDFInstrumentsAsync(_token, recordId, eventName, "demographics", true, filedDownloadPath, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportPDFInstrumentsAsyncResult2: {ExportPDFInstrumentsAsyncResult2}");
            #endregion ExportPDFInstrumentsAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportInstrumentMappingAsync()
            Console.WriteLine($"Calling ExportInstrumentMappingAsync()");
            var ExportInstrumentMappingAsyncResult = redcap_api_1_0_7.ExportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, arms, OnErrorFormat.json).Result;
            Console.WriteLine($"ExportInstrumentMappingAsyncResult: {ExportInstrumentMappingAsyncResult}");
            #endregion ExportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportInstrumentMappingAsync()
            var importInstrumentMappingData = new List<FormEventMapping> { new FormEventMapping { arm_num = "1", unique_event_name = "clinical_arm_1", form = "demographics" } };
            Console.WriteLine($"Calling ImportInstrumentMappingAsync()");
            var ImportInstrumentMappingAsyncResult = redcap_api_1_0_7.ImportInstrumentMappingAsync(_token, Content.FormEventMapping, ReturnFormat.json, importInstrumentMappingData, OnErrorFormat.json).Result;
            Console.WriteLine($"ImportInstrumentMappingAsyncResult: {ImportInstrumentMappingAsyncResult}");
            #endregion ImportInstrumentMappingAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportMetaDataAsync()
            Console.WriteLine($"Calling ExportMetaDataAsync()");
            var ExportMetaDataAsyncResult = redcap_api_1_0_7.ExportMetaDataAsync(_token, Content.MetaData, ReturnFormat.json, null, null, OnErrorFormat.json).Result;
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
            //var ImportMetaDataAsyncResult = redcap_api_1_0_7.ImportMetaDataAsync(_token, "metadata", ReturnFormat.json, importMetaData, OnErrorFormat.json).Result;
            //Console.WriteLine($"ImportMetaDataAsyncResult: {ImportMetaDataAsyncResult}");
            #endregion ImportMetaDataAsync()

            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region CreateProjectAsync()
            var projectData = new List<RedcapProject> { new RedcapProject { project_title = "Amazing Project ", purpose = ProjectPurpose.Other, purpose_other = "Test" } };
            Console.WriteLine($"Calling CreateProjectAsync(), creating a new project with Amazing Project as title, purpose 1 (other) ");
            Console.WriteLine($"-----------------------Notice the use of SUPER TOKEN------------------------");
            var CreateProjectAsyncResult = redcap_api_1_0_7.CreateProjectAsync(_superToken, Content.Project, ReturnFormat.json, projectData, OnErrorFormat.json, null).Result;
            Console.WriteLine($"CreateProjectAsyncResult: {CreateProjectAsyncResult}");
            #endregion CreateProjectAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ImportProjectInfoAsync()
            var projectInfo = new RedcapProjectInfo { ProjectTitle = "Updated Amazing Project ", Purpose = ProjectPurpose.QualityImprovement, SurveysEnabled = 1 };
            Console.WriteLine($"Calling ImportProjectInfoAsync()");
            var ImportProjectInfoAsyncResult = redcap_api_1_0_7.ImportProjectInfoAsync(_token, Content.ProjectSettings, ReturnFormat.json, projectInfo).Result;
            Console.WriteLine($"ImportProjectInfoAsyncResult: {ImportProjectInfoAsyncResult}");
            #endregion ImportProjectInfoAsync()
            Console.WriteLine("----------------------------Press Enter to Continue-------------");
            Console.ReadLine();

            #region ExportProjectInfoAsync()
            Console.WriteLine($"Calling ExportProjectInfoAsync()");
            var ExportProjectInfoAsyncResult = redcap_api_1_0_7.ExportProjectInfoAsync(_token, Content.ProjectSettings, ReturnFormat.json).Result;
            Console.WriteLine($"ExportProjectInfoAsyncResult: {ExportProjectInfoAsyncResult}");
            #endregion ExportProjectInfoAsync()

            Console.WriteLine("----------------------------Demo completed! Press Enter to Exit-------------");
            Console.ReadLine();

            #region ExportRecordsAsync()
            Console.WriteLine($"Calling ExportRecordsAsync()");
            Console.WriteLine($"Using record 1");
            Console.WriteLine($"Using instrumentname = demographics");
            var instrumentName = new string[] { "demographics" };
            var ExportRecordsAsyncResult = redcap_api_1_0_7.ExportRecordsAsync(_token, Content.Record, ReturnFormat.json, RedcapDataType.flat, null, null, instrumentName).Result;
            Console.WriteLine($"ExportRecordsAsyncResult: {ExportProjectInfoAsyncResult}");
            #endregion ExportRecordsAsync()

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

    }


}
