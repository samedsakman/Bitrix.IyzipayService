namespace Bitrix.Iyzipay.Models.Invoice;

public class InvoiceViewModel
{
    public Result Result { get; set; }
    public Time Time { get; set; }
}

public class Result
{
    public Item Item { get; set; }
}

public class Item
{
    public int Id { get; set; } = 1;
    public string XmlId { get; set; }
    public string Title { get; set; } = "TITLE";
    public int CreatedBy { get; set; }
    public int UpdatedBy { get; set; }
    public int MovedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
    public DateTime MovedTime { get; set; }
    public int CategoryId { get; set; }
    public string Opened { get; set; }
    public string StageId { get; set; }
    public string PreviousStageId { get; set; }
    public DateTime Begindate { get; set; }
    public DateTime Closedate { get; set; }
    public int CompanyId { get; set; }
    public int ContactId { get; set; } = 1;
    public int Opportunity { get; set; } = 1;
    public string IsManualOpportunity { get; set; }
    public int TaxValue { get; set; }
    public string CurrencyId { get; set; }
    public int MycompanyId { get; set; }
    public string SourceId { get; set; }
    public string SourceDescription { get; set; }
    public int WebformId { get; set; }
    public string UfCrmSmartInvoice1681886423 { get; set; }
    public int AssignedById { get; set; }
    public string Comments { get; set; }
    public string AccountNumber { get; set; }
    public string LocationId { get; set; }
    public int LastActivityBy { get; set; }
    public DateTime LastActivityTime { get; set; }
    public object ParentId2 { get; set; }
    public object ParentId7 { get; set; }
    public object ParentId131 { get; set; }
    public object ParentId157 { get; set; }
    public object ParentId165 { get; set; }
    public object ParentId168 { get; set; }
    public object ParentId184 { get; set; }
    public object UtmSource { get; set; }
    public object UtmMedium { get; set; }
    public object UtmCampaign { get; set; }
    public object UtmContent { get; set; }
    public object UtmTerm { get; set; }
    public List<object> Observers { get; set; }
    public List<int> ContactIds { get; set; }
    public int EntityTypeId { get; set; }
}

public class Time
{
    public double Start { get; set; }
    public double Finish { get; set; }
    public double Duration { get; set; }
    public double Processing { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateFinish { get; set; }
    public int OperatingResetAt { get; set; }
    public double Operating { get; set; }
}