using System;
using System.Collections.Generic;
using System.Text;

namespace GenericModals
{
   
    public class ConnectionStrings
    {
        public string ProductsDatabase { get; set; }
        public string ItemsDatabase { get; set; }
    }

    public class Configs
    {
        public string Default_select_Limit { get; set; }
    }

    public class NoteTypes
    {
        public string vendor { get; set; }
    }

    public class UploadPath
    {
        public string path { get; set; }
    }

    public class S3config
    {
        public string Bucket_Name { get; set; }
        public string Folder { get; set; }
    }

    public class Services
    {
        public string Badger { get; set; }
        public string NotesAndDoc { get; set; }
        public string ItemsService { get; set; }
    }

    public class ServicesLIVE
    {
        public string Badger { get; set; }
        public string NotesAndDoc { get; set; }
        public string ItemsService { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class Sizes
    {
        public string X { get; set; }
        public string S { get; set; }
        public string M { get; set; }
        public string L { get; set; }
    }

    public class Categories
    {
        public string Clothing { get; set; }
        public string Accessories { get; set; }
    }

    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public Configs configs { get; set; }
        public NoteTypes Note_Types { get; set; }
        public UploadPath UploadPath { get; set; }
        public S3config S3config { get; set; }
        public Services Services { get; set; }
        public ServicesLIVE Services_LIVE { get; set; }
        public Logging Logging { get; set; }
        public Sizes Sizes { get; set; }
        public Categories Categories { get; set; }
        public string AllowedHosts { get; set; }
    }
}
