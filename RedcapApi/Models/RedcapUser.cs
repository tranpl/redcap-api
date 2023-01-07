using System.ComponentModel.DataAnnotations;

namespace Redcap.Models
{
    public class RedcapUser
    {
        [Display(Name = "username")]
        public string Username { get; set; }

        [Display(Name = "expiration")]
        public string Expiration { get; set; }

        [Display(Name = "data_access_group")]
        public string DataAccessGroup { get; set; }

        [Display(Name = "design")]
        public string Design { get; set; }

        [Display(Name = "user_rights")]
        public string UserRights { get; set; }

        [Display(Name = "data_access_groups")]
        public string DataAccessGroups { get; set; }

        [Display(Name = "data_export")]
        public string DataExport { get; set; }

        [Display(Name = "reports")]
        public string Reports { get; set; }

        [Display(Name = "stats_and_charts")]
        public string StatsAndCharts { get; set; }

        [Display(Name = "manage_survey_participants")]
        public string ManageSurveyParticipants { get; set; }

        [Display(Name = "calendar")]
        public string Calendar { get; set; }

        [Display(Name = "data_import_tool")]
        public string DataImportTool { get; set; }

        [Display(Name = "data_comparison_tool")]
        public string DataComparisonTool { get; set; }

        [Display(Name = "logging")]
        public string Logging { get; set; }

        [Display(Name = "file_repository")]
        public string FileRepository { get; set; }

        [Display(Name = "data_quality_create")]
        public string DataQualityCreate { get; set; }

        [Display(Name = "data_quality_execute")]
        public string DataQualityExecute { get; set; }

        [Display(Name = "api_export")]
        public string ApiExport { get; set; }

        [Display(Name = "api_import")]
        public string ApiImport { get; set; }

        [Display(Name = "mobile_app")]
        public string MobileApp { get; set; }

        [Display(Name ="record_create_tool")]
        public string RecordCreateTool { get; set; }

        [Display(Name = "mobile_app_download_data")]
        public string MobileAppDownloadData { get; set; }

        [Display(Name = "record_create")]
        public string RecordCreate { get; set; }

        [Display(Name = "record_rename")]
        public string RecordRename { get; set; }

        [Display(Name = "record_delete")]
        public string RecordDelete { get; set; }

        [Display(Name = "lock_records_all_forms")]
        public string LockRecordsAllForms { get; set; }

        [Display(Name = "lock_records")]
        public string LockRecords { get; set; }

        [Display(Name = "lock_records_customization")]
        public string LockRecordsCustomization { get; set; }
    }
}
