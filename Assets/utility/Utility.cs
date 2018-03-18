
using Database.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Utility
{
    public static Location getUTM32Location(Location loc)
    {
        return null;
    }

    public static double getDistanceBetweenLocations(Location loc1, Location loc2)
    {
        double R = 6371e3; // metres
        double φ1 = ToRadians(loc1.X);
        double φ2 = ToRadians(loc2.X);
        double Δφ = ToRadians((loc2.X - loc1.X));
        double Δλ = ToRadians((loc2.Y - loc1.Y));

        double a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double d = R * c;
        return d;
    }

    public static double ToRadians(double val)
    {
        return (Math.PI / 180) * val;
    }
}
