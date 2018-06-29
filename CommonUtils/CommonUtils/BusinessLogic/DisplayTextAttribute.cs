namespace CommonUtils {
    public class DisplayTextAttribute : System.Attribute {
        public string DisplayText { get; private set; }
        public DisplayTextAttribute(string displayText) { this.DisplayText = displayText; }
    }
}
