namespace AppCore.Business.Models.Paging
{
    public class PageModel
    {
        public int PageNumber { get; set; } = 1;
        public int RecordsPerPageCount { get; set; } = 10;
        public int RecordsCount { get; set; }

        List<PageItemModel> _pages;

        public void SetPages()
        {
            _pages = new List<PageItemModel>();
            double totalPageCount = Math.Ceiling((double)RecordsCount / (double)RecordsPerPageCount);
            if (totalPageCount == 0)
            {
                _pages.Add(new PageItemModel()
                {
                    Value = "1",
                    Text = "1"
                });
            }
            else
            {
                for (int pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
                {
                    _pages.Add(new PageItemModel()
                    {
                        Value = pageNumber.ToString(),
                        Text = pageNumber.ToString()
                    });
                }
            }
        }

        public List<PageItemModel> GetPages()
        {
            return _pages;
        }
    }
}
