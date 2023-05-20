namespace Bitrix.Iyzipay.Models.Contact;

public class ContactViewModel
{
    public Result Result { get; set; }
    public Time Time { get; set; }
}

public class Result
{
    private object? _address;
    public string? ID { get; set; } = "ID";
    public object POST { get; set; }
    public object COMMENTS { get; set; }
    public object HONORIFIC { get; set; }
    public string? NAME { get; set; } = "NAME";
    public string SECONDNAME { get; set; }
    public string? LASTNAME { get; set; } = "LASTNAME";
    public object PHOTO { get; set; }
    public object LEADID { get; set; }
    public string TYPEID { get; set; }
    public string SOURCEID { get; set; }
    public object SOURCEDESCRIPTION { get; set; }
    public object COMPANYID { get; set; }
    public string BIRTHDATE { get; set; }
    public string EXPORT { get; set; }
    public string HASPHONE { get; set; }
    public string HASEMAIL { get; set; }
    public string HASIMOL { get; set; }
    public DateTime DATECREATE { get; set; }
    public DateTime DATEMODIFY { get; set; }
    public string ASSIGNEDBYID { get; set; }
    public string CREATEDBYID { get; set; }
    public string MODIFYBYID { get; set; }
    public string OPENED { get; set; }
    public object ORIGINATORID { get; set; }
    public object ORIGINID { get; set; }
    public object ORIGINVERSION { get; set; }
    public object FACEID { get; set; }

    public object? ADDRESS
    {
        get => _address;
        set
        {
            if(value != null)
                _address = value;
            _address = "address";
        }
    }
    public object? ADDRESS2 { get; set; }
    public object? ADDRESSCITY { get; set; } = "ADDRESSCITY";
    public object ADDRESSPOSTALCODE { get; set; }
    public object ADDRESSREGION { get; set; }
    public object ADDRESSPROVINCE { get; set; }
    public object? ADDRESSCOUNTRY { get; set; } = "ADDRESSCOUNTRY";
    public object ADDRESSLOCADDRID { get; set; }
    public object UTMSOURCE { get; set; }
    public object UTMMEDIUM { get; set; }
    public object UTMCAMPAIGN { get; set; }
    public object UTMCONTENT { get; set; }
    public object UTMTERM { get; set; }
    public object PARENTID131 { get; set; }
    public object PARENTID144 { get; set; }
    public object PARENTID157 { get; set; }
    public object PARENTID165 { get; set; }
    public object PARENTID168 { get; set; }
    public object PARENTID184 { get; set; }
    public string LASTACTIVITYBY { get; set; }
    public DateTime LASTACTIVITYTIME { get; set; }
    public bool UFCRM1682353897548 { get; set; }
    public List<IM> IM { get; set; }
    public List<LINK> LINK { get; set; }
    public List<PHONE>? PHONE { get; set; }
    public List<EMAIL>? EMAIL { get; set; }
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

public class EMAIL
{
    public string ID { get; set; }
    public string VALUETYPE { get; set; }
    public string? VALUE { get; set; } = "EMAIL";
    public string TYPEID { get; set; }
}

public class IM
{
    public string ID { get; set; }
    public string VALUETYPE { get; set; }
    public string VALUE { get; set; }
    public string TYPEID { get; set; }
}

public class LINK
{
    public string ID { get; set; }
    public string VALUETYPE { get; set; }
    public string VALUE { get; set; }
    public string TYPEID { get; set; }
}

public class PHONE
{
    public string ID { get; set; }
    public string VALUETYPE { get; set; }
    public string VALUE { get; set; }
    public string TYPEID { get; set; }
}