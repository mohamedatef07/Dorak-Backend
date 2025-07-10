namespace Data
{
    public class CommitData
    {
        private DorakContext context;
        public CommitData(DorakContext _context)
        {
            if (context == null)
            {
                context = _context;
            }
        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
