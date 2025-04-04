using System;

namespace gpconnect_appointment_checker.Models;

public class PagedData<T> where T : class
{
    public PagedData(T[] items)
    {
        Items = items;
    }

    public T[] Items { get; set; }
    public required int TotalItems { get; set; }
    public int PageSize { get; set; } = 50;
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}