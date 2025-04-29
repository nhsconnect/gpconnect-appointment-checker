using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.Models;

public class PagingModel
{
    public PagingModel()
    {
    }

    public PagingModel(int currentPage, int totalPages, Func<int, Dictionary<string, string>> getRouteValues)
    {
        CurrentPage = currentPage;
        TotalPages = totalPages;
        GetRouteValues = getRouteValues;
    }

    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 50;
    public int TotalItems { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public Func<int, Dictionary<string, string>> GetRouteValues { get; set; }
}