namespace CledAcademy.Core.Models
{
    public class NewsPageParameters
    {
        public int PageStartIndex { get; set; }
        public int PageEndIndex { get; set; }
        public int CurrentPageNumber { get; set; }
        public int TotalPageCount { get; set; }
    }
}