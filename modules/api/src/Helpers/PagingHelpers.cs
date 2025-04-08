namespace gpconnect_appointment_checker.api.Helpers;

public class PagingHelpers
{
    public static TItem[][] Paginate<TItem>(IEnumerable<TItem> originalItems, int pageSize = 50)
    {
        var itemsArray = originalItems as TItem[] ?? originalItems.ToArray();
        var totalPages = (int)Math.Ceiling((double)itemsArray.Count() / pageSize);
        var pages = new List<TItem[]>();

        for (var pageNumber = 0; pageNumber < totalPages; pageNumber++)
        {
            pages.Add(itemsArray.Skip(pageNumber * pageSize).Take(pageSize).ToArray());
        }

        return pages.ToArray();
    }
}