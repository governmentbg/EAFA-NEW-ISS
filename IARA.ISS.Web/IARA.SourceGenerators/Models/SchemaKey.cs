namespace IARA.SourceGenerators
{
    public class SchemaKey
    {
        public string SchemaFolder { get; internal set; }
        public string EntitiesDir { get; internal set; }
        public string ExtendedEntitiesDir { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is SchemaKey)
            {
                SchemaKey other = (obj as SchemaKey);

                return this.SchemaFolder == other.SchemaFolder
                    && this.EntitiesDir == other.EntitiesDir
                    && this.ExtendedEntitiesDir == other.ExtendedEntitiesDir;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return SchemaFolder.GetHashCode() ^ EntitiesDir.GetHashCode() ^ ExtendedEntitiesDir.GetHashCode();
        }
    }
}
