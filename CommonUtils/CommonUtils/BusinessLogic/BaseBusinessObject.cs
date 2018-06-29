namespace CommonUtils {
    public abstract class BaseBusinessObject<BO> : BaseObject<BO> {
        public abstract void Insert();
        public abstract BO InsertAndReturn();
        public abstract void Update();
        public abstract BO UpdateAndReturn();
        public abstract void Delete();
    }
}
