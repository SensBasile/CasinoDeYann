namespace CasinoDeYann.Services.User.Models;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; }
    public long Money { get; set; }
    public long Xp { get; set; }
    public string Role { get; set; }
}