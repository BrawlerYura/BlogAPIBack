using System;
using System.Collections.Generic;

namespace BlogApi.Migrations;

public partial class AsHouse
{
    public long? Id { get; set; }

    public long? Objectid { get; set; }

    public Guid? Objectguid { get; set; }

    public long? Changeid { get; set; }

    public string? Housenum { get; set; }

    public string? Addnum1 { get; set; }

    public string? Addnum2 { get; set; }

    public int? Housetype { get; set; }

    public int? Addtype1 { get; set; }

    public int? Addtype2 { get; set; }

    public int? Opertypeid { get; set; }

    public long? Previd { get; set; }

    public long? Nextid { get; set; }

    public DateOnly? Updatedate { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public int? Isactual { get; set; }

    public int? Isactive { get; set; }
}
