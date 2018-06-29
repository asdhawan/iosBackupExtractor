using System;

namespace iosBackupExtractor.iOSModels {
    public class iOSFile {
        public string fileID { get; set; }
        public string domain { get; set; }
        public string relativePath { get; set; }
        public int flags { get; set; }
        public Byte[] file { get; set; }
    }
}
