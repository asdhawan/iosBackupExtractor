using CommonUtils;
using CsvHelper;
using iosBackupExtractor.iOSModels;
using System;
using System.IO;
using System.Linq;

namespace iosBackupExtractor.Extractors {
    public class TextMessageExtractor : BaseExtractor {
        public TextMessageExtractor() : base("Library/SMS/sms.db") { }
        private class VMIndexItem {
            public DateTime Date { get; set; }
            public string Sender { get; set; }
            public int Duration { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
        }
        private class MessageLogItem {
            public DateTime SentDateTime { get; set; }
            public string Sender { get; set; }
            public string Message { get; set; }
        }
        private static readonly string messageQuery =
            @"select 
                chat_id, 
                c.chat_identifier, 
                h.id as sender, 
                m.ROWID,
                m.guid,
                m.text,
                m.handle_id,
                m.subject,
                m.country,
                m.attributedBody,
                m.service,
                m.account,
                m.account_guid,
                datetime(m.date / 1000000000, 'unixepoch', '31 years') as date,
                case when m.date_read = 0 then NULL else datetime(m.date_read, 'unixepoch', '31 years') end as date_read,
                case when m.date_delivered = 0 then NULL else datetime(m.date_delivered, 'unixepoch', '31 years') end as date_delivered,
                m.is_delivered,
                m.is_audio_message
            from message m 
            inner join chat_message_join cmj 
            on m.ROWID = cmj.message_id 
            inner join chat c 
            on cmj.chat_id = c.ROWID
            inner join handle h 
            on m.handle_id = h.ROWID";
        public void Run() {
            var manifestConn = new SQLite.SQLiteConnection(ManifestDbPath);
            var smsConn = new SQLite.SQLiteConnection(DbPath);
            var iosChats = smsConn.Query<iOSChat>("select ROWID, guid, account_id, chat_identifier, service_name from chat");
            var iosMessages = smsConn.Query<iOSMessage>(messageQuery);
            //var iosFiles = manifestConn.Query<iOSFile>("SELECT * from FILES where relativePath like 'Library/Voicemail/%.amr'");
            iosChats.ForEach(chat => {
                var chatFolder = chat.chat_identifier.RemoveSpecialCharacters();
                var chatFolderPath = $@"{DestinationBasePath}\{chatFolder}";
                if (!Directory.Exists(chatFolderPath)) Directory.CreateDirectory(chatFolderPath);

                var chatMessages = iosMessages.Where(x => x.chat_id == chat.ROWID).OrderBy(x=>x.date);
                var transcriptItems = chatMessages.Select(x => new MessageLogItem() { SentDateTime = x.date, Sender = x.sender, Message = x.text });

                string transcriptFilePath = $@"{chatFolderPath}\transcript.csv";
                using (TextWriter writer = new StreamWriter(transcriptFilePath)) {
                    var csv = new CsvWriter(writer);
                    csv.WriteRecords(transcriptItems);
                }
            });

            Console.WriteLine("Text Message Extract Complete");
        }
    }
}
