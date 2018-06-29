namespace CommonUtils {
    public class DbColumnAttribute : System.Attribute {
        public string ColumnName { get; private set; }
        public DbColumnAttribute(string columnName = null) { this.ColumnName = columnName; }
    }
}
