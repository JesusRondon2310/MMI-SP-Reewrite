using static MMI_SP.Agency.ItemsManager;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.Office.Ambient
{
    internal class Handler
    {
        // ==========================================
        // BLOQUE 1: Variables de la clase y creación del objeto
        // ==========================================
        internal OfficeItemsCollection ItemsCollection { get; private set; }

        public Handler()
        {
            var pickResult = CollectionPicker.Pick();
            if (pickResult is Err<OfficeItemsCollection> err)
            {
                // Si no se pudo elegir colección, se usa Empty y se notifica
                Logger.Error(err.FullMessage);
                ItemsCollection = new OfficeItemsCollection(Empty);
                return;
            }

            ItemsCollection = new OfficeItemsCollection(((Ok<OfficeItemsCollection>)pickResult).Value);
            var buildResult = Builder.Build(ItemsCollection);
            if (buildResult is Err<bool> buildErr)
            {
                Logger.Error(buildErr.FullMessage);
                CleanUp.Execute();
            }
        }

        public Handler(OfficeItemsCollection collection)
        {
            ItemsCollection = new OfficeItemsCollection(collection);
            var buildResult = Builder.Build(ItemsCollection);
            if (buildResult is Err<bool> buildErr)
            {
                Logger.Error(buildErr.FullMessage);
                CleanUp.Execute();
            }
        }

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal void CleanOffice()
        {
            CleanUp.Execute();
        }
    }
}