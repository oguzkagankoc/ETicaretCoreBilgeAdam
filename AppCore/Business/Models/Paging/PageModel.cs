namespace AppCore.Business.Models.Paging
{
    public class PageModel
    {
        public int PageNumber { get; set; } = 1;
        public int RecordsPerPageCount { get; set; } = 10;
        public int RecordsCount { get; set; } = 0;

        private List<PageItemModel> _pages;
        public List<PageItemModel> Pages // sayfalar sayfadaki kayıt sayısı (RecordsPerPageCount) ve toplam kayıt sayısına (RecordsCount) göre doldurularak istenilen yerde get edilebilsin
        {
            get
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
                return _pages;
            }
        }
    }
}
