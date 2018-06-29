using CommonUtils;
using iosBackupExtractor.iOSModels;
using System;
using System.IO;
using System.Linq;

namespace iosBackupExtractor.Extractors {
    public class BaseExtractor {
        protected string BaseBcpPath { get; set; }
        protected string ManifestDbPath { get; set; }
        protected string DestinationBasePath { get; set; }
        protected string IndexFilePath { get; set; }
        protected string DbFileRelativePath { get; set; }
        protected string DbPath { get; set; }

        public BaseExtractor(string dbFileRelativePath) {
            DbFileRelativePath = dbFileRelativePath;
        }

        public void Initialize(string baseBcpPath, string destinationBasePath) {
            BaseBcpPath = baseBcpPath;
            DestinationBasePath = destinationBasePath;

            if (!Directory.Exists(BaseBcpPath))
                Console.WriteLine($"Base path of Backup not found - {BaseBcpPath}");
            else {
                if (!Directory.Exists(DestinationBasePath))
                    Directory.CreateDirectory(DestinationBasePath);

                //set up index file path and delete any existing version
                IndexFilePath = $@"{DestinationBasePath}\index.csv";
                if (File.Exists(IndexFilePath)) File.Delete(IndexFilePath);

                //setup manifest DB and locate file
                ManifestDbPath = $@"{BaseBcpPath}\Manifest.db";
                var manifestConn = new SQLite.SQLiteConnection(ManifestDbPath);

                var dbFile = manifestConn.Query<iOSFile>($"SELECT * from FILES where relativePath = '{DbFileRelativePath}'").FirstOrDefault();
                DbPath = $@"{BaseBcpPath}\{dbFile.fileID.Left(2)}\{dbFile.fileID}";
            }
        }
    }
}
