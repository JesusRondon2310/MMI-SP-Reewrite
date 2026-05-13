namespace MMI_SP.Agency.Office.Entry
{
    internal static class CleanUpSequence
    {
        public static void Execute(Office.Manager office)
        {
            office.DestroyOffice();
        }
    }
}