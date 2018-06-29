using CommonUtils;
using CsvHelper;
using iosBackupExtractor.iOSModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iosBackupExtractor.Extractors {
    public class VoiceMailExtractor : BaseExtractor {
        public VoiceMailExtractor() : base("Library/Voicemail/voicemail.db") { }

        private class VMIndexItem {
            public DateTime Date { get; set; }
            public string Sender { get; set; }
            public int Duration { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
        }
        public void Run() {
            var manifestConn = new SQLite.SQLiteConnection(ManifestDbPath);
            var voicemailConn = new SQLite.SQLiteConnection(DbPath);
            var voicemails = voicemailConn.Query<iOSVoiceMail>("SELECT ROWID, datetime(date, 'unixepoch') as recdate, sender, duration from voicemail");
            var iosFiles = manifestConn.Query<iOSFile>("SELECT * from FILES where relativePath like 'Library/Voicemail/%.amr'");
            var vmIndexItems = new List<VMIndexItem>();
            voicemails.ForEach(vm => {
                var iosFile = iosFiles.FirstOrDefault(x => x.relativePath.ToUpper() == vm.relativePath.ToUpper());
                var amrOrigPath = $@"{BaseBcpPath}\{iosFile.fileID.Left(2)}\{iosFile.fileID}";
                var amrNewPath = $@"{DestinationBasePath}\{vm.ROWID}.amr";
                if (!File.Exists(amrNewPath)) File.Copy(amrOrigPath, amrNewPath);
                vmIndexItems.Add(new VMIndexItem() {
                    Date = vm.recdate,
                    Sender = vm.sender,
                    Duration = vm.duration,
                    FileName = vm.amrName,
                    Path = amrNewPath
                });
            });

            using (TextWriter writer = new StreamWriter(IndexFilePath)) {
                var csv = new CsvWriter(writer);
                csv.WriteRecords(vmIndexItems);
            }

            Console.WriteLine("Voice Mail Extract Complete");
        }
    }
}
