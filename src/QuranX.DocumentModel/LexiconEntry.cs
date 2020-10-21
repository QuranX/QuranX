namespace QuranX.DocumentModel
{
    public class LexiconEntry
    {
        public readonly string Root;
        public readonly string Description;

        public LexiconEntry(string root, string description)
        {
            this.Root = root;
            this.Description = description;
        }
    }
}
