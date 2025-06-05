namespace CasinoDeYann.Src.DataAccess.Dbo;

public partial class User : IObjectWithId
{
    public long Id { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
    public long Money { get; set; }
    public long Xp { get; set; }
    
    public string Role { get; set; }
}