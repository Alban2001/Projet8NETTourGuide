using GpsUtil.Location;
using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double DegToRad = Math.PI / 180.0;
    private const double EarthRadiusKm = 6371.0;
    private const double KmToNauticalMiles = 0.539957;

    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    //public async Task CalculateRewards(User user)
    //{
    //    count++;

    //    List<VisitedLocation> userLocations = user.VisitedLocations.ToList();
    //    List<Attraction> attractions = _gpsUtil.GetAttractions().ToList();

    //    var rewardsToAdd = new List<UserReward>();
    //    Parallel.ForEach(userLocations, (visitedLocation) =>
    //    {
    //        Parallel.ForEach(attractions, (attraction) =>
    //        {
    //            if (!user.UserRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
    //            {
    //                if (NearAttraction(visitedLocation, attraction))
    //                {
    //                    rewardsToAdd.Add(
    //                        new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user))
    //                    );
    //                }
    //            }
    //        });
    //    });

    //    Parallel.ForEach(rewardsToAdd, (reward) =>
    //    {
    //        user.AddUserReward(reward);
    //    });
    //}

    //public Task CalculateRewards(User user)
    //{
    //    count++;

    //    var userLocations = user.VisitedLocations.ToList();
    //    var attractions = (_gpsUtil.GetAttractions()).ToList();

    //    var userRewardNames = new HashSet<string>(
    //        user.UserRewards.Select(r => r.Attraction.AttractionName)
    //    );

    //    var rewards = new ConcurrentBag<UserReward>();

    //    Parallel.ForEach(userLocations, visitedLocation =>
    //    {
    //        foreach (var attraction in attractions)
    //        {
    //            //bool hasReward = user.UserRewards
    //            //    .Any(r => r.Attraction.AttractionName == attraction.AttractionName);

    //            if (userRewardNames.Contains(attraction.AttractionName))
    //                continue;
    //            //if (hasReward)
    //            //    continue;

    //            if (NearAttraction(visitedLocation, attraction))
    //            {
    //                rewards.Add(new UserReward(
    //                    visitedLocation,
    //                    attraction,
    //                    GetRewardPoints(attraction, user)
    //                ));
    //            }
    //        }
    //    });

    //    // Ajout final en séquentiel (évite les corruptions de données)
    //    foreach (var reward in rewards)
    //        user.AddUserReward(reward);

    //    return Task.CompletedTask;
    //}

    public async Task CalculateRewards(User user)
    {
        List<VisitedLocation> userLocations = user.VisitedLocations.ToList();
        List<Attraction> attractions = await _gpsUtil.GetAttractions();

        var rewardedAttractions = new HashSet<string>(
            user.UserRewards.Select(r => r.Attraction.AttractionName));

        List<UserReward> rewardsToAdd = new List<UserReward>();

        foreach (var visitedLocation in userLocations)
        {
            foreach (var attraction in attractions)
            {
                if (!rewardedAttractions.Contains(attraction.AttractionName)
                    && NearAttraction(visitedLocation, attraction))
                {
                    int rewardsPoints = GetRewardPoints(attraction, user);
                    UserReward reward = new UserReward(visitedLocation, attraction, rewardsPoints);
                    rewardsToAdd.Add(reward);

                    rewardedAttractions.Add(attraction.AttractionName);
                }
            }
        }

        foreach (UserReward reward in rewardsToAdd)
        {
            user.AddUserReward(reward);
        }
    }

    public async Task CalculateRewardsAsync(User user)
    {
        await Task.Run(() => CalculateRewards(user));
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    public async Task<List<NearbyAttraction>> ClosestFiveAttractions(Locations location, User user)
    {
        // Pour chaque attraction, on calcule la distance avec la location de l'utilisateur puis on les classe juste après et on récupère les 5 les plus proches
        List<Attraction> attractions = await _gpsUtil.GetAttractions();
        var closestAttractions = attractions
            .Select(a => new NearbyAttraction
            {
                Attraction = a,
                Distance = GetDistance(a, location),
                RewardPoints = GetRewardPoints(a, user)
            })
            .OrderBy(x => x.Distance)
            .Take(5)
            .ToList();

        return closestAttractions;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    private int GetRewardPoints(Attraction attraction, User user)
    {
        return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
    }

    //public double GetDistance(Locations loc1, Locations loc2)
    //{
    //    double nauticalMiles = 0;
    //    double lat1 = Math.PI * loc1.Latitude / 180.0;
    //    double lon1 = Math.PI * loc1.Longitude / 180.0;
    //    double lat2 = Math.PI * loc2.Latitude / 180.0;
    //    double lon2 = Math.PI * loc2.Longitude / 180.0;

    //    double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
    //                            + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

    //    nauticalMiles = 60.0 * angle * 180.0 / Math.PI;

    //    return StatuteMilesPerNauticalMile * nauticalMiles;
    //}

    public double GetDistance(Locations loc1, Locations loc2)
    {
        // 🚀 Filtrage rapide (évite 90% des calculs lourds)
        if (Math.Abs(loc1.Latitude - loc2.Latitude) > 0.1 ||
            Math.Abs(loc1.Longitude - loc2.Longitude) > 0.1)
        {
            return double.MaxValue;
        }

        double lat1 = loc1.Latitude * DegToRad;
        double lat2 = loc2.Latitude * DegToRad;
        double dLat = (loc2.Latitude - loc1.Latitude) * DegToRad;
        double dLon = (loc2.Longitude - loc1.Longitude) * DegToRad;

        double sinLat = Math.Sin(dLat / 2);
        double sinLon = Math.Sin(dLon / 2);

        double h =
            sinLat * sinLat +
            Math.Cos(lat1) * Math.Cos(lat2) *
            sinLon * sinLon;

        double c = 2 * Math.Asin(Math.Min(1.0, Math.Sqrt(h)));

        double nauticalMiles = EarthRadiusKm * c * KmToNauticalMiles;

        // ⚠️ Si tu dois absolument retourner des miles statutaires
        return nauticalMiles * StatuteMilesPerNauticalMile;
    }
}
