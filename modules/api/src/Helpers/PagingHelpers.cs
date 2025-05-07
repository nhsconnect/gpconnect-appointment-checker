namespace gpconnect_appointment_checker.api.Helpers;

public static class PagingHelpers
{
    public static TItem[][] Paginate<TItem>(IEnumerable<TItem> originalItems, int pageSize = 50)
    {
        ArgumentNullException.ThrowIfNull(originalItems);

        var itemsArray = originalItems as TItem[] ?? originalItems.ToArray();

        if (itemsArray.Length == 0)
        {
            return [];
        }

        var totalPages = (int)Math.Ceiling((double)itemsArray.Length / pageSize);
        var pages = new List<TItem[]>();

        for (var pageNumber = 0; pageNumber < totalPages; pageNumber++)
        {
            pages.Add(itemsArray.Skip(pageNumber * pageSize).Take(pageSize).ToArray());
        }

        return pages.ToArray();
    }
}