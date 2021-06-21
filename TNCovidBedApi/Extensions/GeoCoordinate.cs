using System;
using System.Drawing;

namespace TNCovidBedApi
{
    /// <summary>
    /// Manipulate the GeoCo-ordintates
    /// </summary>
    internal static class GeoCoordinate
    {
        /// <summary>
        /// Finds distance between two Geo Co-Ordinates
        /// </summary>
        /// <param name="start">Start point of location</param>
        /// <param name="end">End point of location</param>
        /// <returns>Distance betweens start and end</returns>
        public static float FindDistance(PointF start, PointF end)
        {
            return (start == end) ? 0 :(float)
                RadiansToDegree(
                Math.Acos(
                    Math.Sin(DegreeToRadians(start.X)) * Math.Sin(DegreeToRadians(end.X)) +
                    Math.Cos(DegreeToRadians(start.X)) * Math.Cos(DegreeToRadians(end.X)) * Math.Cos(DegreeToRadians(start.Y - end.Y))
                    )
                ) * 111.18957696f;
        }

        /// <summary>
        /// Converts degree to radians
        /// </summary>
        /// <param name="deg">Degree</param>
        /// <returns>Radian equivalent of degree</returns>
        private static double DegreeToRadians(double deg)
        {
            return (deg * Math.PI / 180);
        }

        /// <summary>
        /// Converts radians to degree
        /// </summary>
        /// <param name="rad">Radian</param>
        /// <returns>Degree equivalent of radian</returns>
        private static double RadiansToDegree(double rad)
        {
            return (rad / Math.PI * 180);
        }
    }
}