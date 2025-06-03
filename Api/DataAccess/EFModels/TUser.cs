namespace CasinoDeYann.Api.DataAccess.EFModels;

public partial class TUser : IObjectWithId
{
    public long Id { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
    public long Money { get; set; }
    public long Xp { get; set; }
    public string Role{ get; set; }
    
    public virtual ICollection<TStats> Stats { get; set; } = new List<TStats>();
}
