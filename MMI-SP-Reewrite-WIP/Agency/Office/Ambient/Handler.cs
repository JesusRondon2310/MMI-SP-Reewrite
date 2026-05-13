using static MMI_SP.Agency.ItemsManager;

namespace MMI_SP.Agency.Office.Ambient
{
    internal class Handler
    {
        internal OfficeItemsCollection ItemsCollection { get; private set; }

        public Handler()
        {
            ItemsCollection = new OfficeItemsCollection(CollectionPicker.Pick());
            Builder.Build(ItemsCollection);
        }

        public Handler(OfficeItemsCollection collection)
        {
            ItemsCollection = new OfficeItemsCollection(collection);
            Builder.Build(ItemsCollection);
        }

        internal void CleanOffice()
        {
            CleanUp.Execute();
        }
    }
}