﻿using System.Net;

namespace BlogApi.Data;

public class ExceptionDetails
{
    public int? Status { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
}