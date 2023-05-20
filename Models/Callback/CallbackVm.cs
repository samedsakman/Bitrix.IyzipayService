namespace Bitrix.Iyzipay.Models.Callback;

public class CallbackVm
{
    public Fields fields { get; set; }
}

public class Fields
{
    public string stageId { get; set; }
}