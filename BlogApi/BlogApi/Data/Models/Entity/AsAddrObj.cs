using System;
using System.Collections.Generic;

namespace BlogApi.Migrations;

public partial class AsAddrObj
{
    public long? Id { get; set; }

    public long? Objectid { get; set; }

    public Guid? Objectguid { get; set; }

    public long? Changeid { get; set; }

    public string? Name { get; set; }

    public string? Typename { get; set; }

    public string? Level { get; set; }

    public int? Opertypeid { get; set; }

    public long? Previd { get; set; }

    public long? Nextid { get; set; }

    public DateOnly? Updatedate { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public int? Isactual { get; set; }

    public int? Isactive { get; set; }
}
