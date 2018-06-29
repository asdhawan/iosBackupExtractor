using System;

namespace iosBackupExtractor.iOSModels {
    public class iOSVoiceMail {
        public int ROWID { get; set; }
        public DateTime recdate { get; set; }
        public string sender { get; set; }
        public int duration { get; set; }
        public string amrName { get { return $"{ROWID}.amr"; } }
        public string relativePath { get { return $"Library/Voicemail/{amrName}"; } }
    }
}
