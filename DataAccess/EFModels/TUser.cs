namespace CasinoDeYann.DataAccess.EfModels;

public partial class TUser
{
    public long Id { get; set; }
    public  string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
    public long money { get; set; }
    public long xp { get; set; }
}
