namespace CasinoDeYann.Services.User.Models;

public class User(long id, string username, long money, long xp, string role)
{
    public long Id { get; set; } = id;
    public string Username { get; set; } = username;
    public long Money { get; set; } = money;
    public long Xp { get; set; } = xp;
    public string Role { get; set; } = role;
}