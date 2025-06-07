using CasinoDeYann.Services.Stats.Models;

namespace CasinoDeYann.Services.User.Models;

public record UserProfileModel(
    long Level,
    long Balance,
    UserStatsModel Stats) 
{ }