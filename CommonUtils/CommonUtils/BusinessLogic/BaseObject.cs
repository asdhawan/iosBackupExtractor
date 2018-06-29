using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace CommonUtils {
    public abstract class BaseObject<BO> {
        public override bool Equals(object obj) {
            return this.GetHashCode() == obj.GetHashCode();
        }
        public override int GetHashCode() {
            int hash = 17;
            foreach (PropertyInfo oProperty in this.GetType().GetProperties()) {
                KeyAttribute keyAttribute = oProperty.GetCustomAttribute<KeyAttribute>();
                if (keyAttribute != null)
                    hash ^= oProperty.GetValue(this, null).GetHashCode();
            }
            return hash;
        }

        public string ToFullString(int indentLevel = 0) {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo oProperty in this.GetType().GetProperties()) {
                try {
                    Type targetType = oProperty.PropertyType.IsGenericType && oProperty.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(oProperty.PropertyType) : oProperty.PropertyType;
                    if (targetType.FullName.Contains("System.Boolean") ||
                        targetType.FullName.Contains("System.DateTime") ||
                        targetType.FullName.Contains("System.String") ||
                        targetType.FullName.Contains("System.Guid") ||
                        targetType.FullName.Contains("System.Double") ||
                        targetType.IsPrimitive ||
                        targetType.IsEnum) {
                        string indent = new string('\t', indentLevel);
                        sb.AppendLine(string.Format("{0}{1} : {2}", indent, oProperty.Name, oProperty.GetValue(this, null)));
                    }
                } catch {/*skip any errors*/ }
            }
            return sb.ToString();
        }
    }
}
