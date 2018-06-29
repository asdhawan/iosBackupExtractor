using System;

namespace iosBackupExtractor.iOSModels {
    public class iOSMessage {
        public int chat_id { get; set; }
        public string chat_identifier { get; set; }
        public string sender { get; set; }
        public int ROWID { get; set; }
        public string guid { get; set; }
        public string text { get; set; }
        public int handle_id { get; set; }
        public string subject { get; set; }
        public string country { get; set; }
        public byte[] attributedBody { get; set; }
        public string service { get; set; }
        public string account { get; set; }
        public string account_guid { get; set; }
        public DateTime date { get; set; }
        public DateTime? date_read { get; set; }
        public DateTime? date_delivered { get; set; }
        public bool is_delivered { get; set; }
        public bool is_audio_message { get; set; }        
    }
}


