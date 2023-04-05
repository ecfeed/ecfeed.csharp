namespace EcFeed
{
    public static class Factory
    {
        public static StructureInitializer GetStructureInitializer()
        {
            return StructureInitializerDefault.Get();
        }
    }
}