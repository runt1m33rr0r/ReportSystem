using ReportSystem.Data.SaveContext.Contracts;

namespace ReportSystem.Data.SaveContext
{
    public class SaveContext : ISaveContext
    {
        private readonly ReportSystemContext context;

        public SaveContext(ReportSystemContext context)
        {
            this.context = context;
        }

        public void Commit()
        {
            this.context.SaveChanges();
        }
    }
}