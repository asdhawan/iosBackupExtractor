using iosBackupExtractor.Extractors;
using System;

namespace iosBackupExtractor {
    class Program {
        static void Main(string[] args) {
            string baseBcpPath = @"C:\CatPhoneBcp\b89f4ad30c11e8a09253e777aa3f1a700b22d633";

            string vmBasePath = @"C:\CatPhoneBcp\Voicemails";
            string msgsBasePath = @"C:\CatPhoneBcp\Messages";

            var vmExtractor = new VoiceMailExtractor();
            vmExtractor.Initialize(baseBcpPath, vmBasePath);
            vmExtractor.Run();

            var messageExtractor = new TextMessageExtractor();
            messageExtractor.Initialize(baseBcpPath, msgsBasePath);
            messageExtractor.Run();

            //TextMessageExtractor.Run(baseBcpPath, msgsBasePath);

            Console.ReadKey();
        }
    }
}
