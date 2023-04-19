namespace EcFeed
{
    public static class Factory
    {
        public static StructureInitializer GetStructureInitializer()
        {
            return StructureInitializerDefault.Get();
        }

        public static StructureSetter GetStructureSetter()
        {
            return StructureSetterDefault.Get();
        }

        public static StructuresSetter GetStructuresSetter(StructureSetter setter)
        {
            return StructuresSetterDefault.Get(setter);
        }
    }
}