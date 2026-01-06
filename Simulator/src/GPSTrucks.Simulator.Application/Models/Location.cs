using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Application.Models
{
    public class Location
    {
        private const double EarthRadiusKm = 6371.0;
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Location(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }


        public Location MoveTowards(Location target, double distanceKm)
        {
            // 1. Convert degrees to radians
            double lat1 = ToRadians(Latitude);
            double lon1 = ToRadians(Longitude);
            double lat2 = ToRadians(target.Latitude);
            double lon2 = ToRadians(target.Longitude);

            // 2. Calculate the Bearing (direction)
            double dLon = lon2 - lon1;
            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                       Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double bearing = Math.Atan2(y, x);

            // 3. Calculate actual distance to target (Haversine)
            // This prevents overshooting the target in a simulator
            double currentDistanceToTarget = CalculateDistance(target);
            if (distanceKm >= currentDistanceToTarget)
            {
                return target.Clone();
            }

            // 4. Calculate the Destination Point
            double angularDistance = distanceKm / EarthRadiusKm;

            double newLat = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) +
                                      Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearing));

            double newLon = lon1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(angularDistance) * Math.Cos(lat1),
                                             Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(newLat));

            return new Location((float)ToDegrees(newLat), (float)ToDegrees(newLon));
        }

        public double CalculateDistance(Location other)
        {
            double dLat = ToRadians(other.Latitude - this.Latitude);
            double dLon = ToRadians(other.Longitude - this.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(this.Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        private static double ToDegrees(double radians) => radians * 180.0 / Math.PI;



        public override string ToString()
        {
            return $"({Latitude}, {Longitude})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Location other)
            {
                return Latitude == other.Latitude && Longitude == other.Longitude;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Latitude.GetHashCode();
                hash = hash * 23 + Longitude.GetHashCode();
                return hash;
            }
        }

        public Location Clone()
        {
            return new Location(Latitude, Longitude);
        }
    }
}
