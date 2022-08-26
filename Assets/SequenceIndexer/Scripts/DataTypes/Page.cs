[System.Serializable]
public class Page
{
    public int page;
    public string column;
    public object before;
    public object after;
    public SortBy[] sort;
    public int pageSize;
    public bool more;
}
