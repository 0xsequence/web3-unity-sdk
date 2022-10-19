namespace SequenceSharp
{
    [System.Serializable]
    public class GetTransactionHistoryReturn
    {
        public Page page;
        public Transaction[] transactions;
    }
}