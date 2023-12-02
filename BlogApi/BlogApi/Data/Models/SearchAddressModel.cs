namespace BlogApi.Data.Models;

public class SearchAddressModel
{
    public int ObjectId { get; set; }
    public int ObjectGuid { get; set; }
    public string? Text { get; set; }
    public GarAddressLevel ObjectLevel { get; set; }
    public string? ObjectLevelText { get; set; }
}