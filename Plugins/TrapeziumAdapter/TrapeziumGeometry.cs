using System;
using System.Drawing;

namespace TrapeziumAdapter
{
  internal static class TrapeziumGeometry
  {
    public static bool IsValidTrapezium(int[] points)
    {
      if (points == null || points.Length != 8)
        return false;

      int ax = points[2] - points[0];
      int ay = points[3] - points[1];
      int bx = points[4] - points[6];
      int by = points[5] - points[7];
      return ax * by == ay * bx;
    }

    public static int[] CreateFromBounds(int left, int top, int width, int height)
    {
      int right = left + Math.Max(0, width);
      int bottom = top + Math.Max(0, height);
      int inset = Math.Max(1, width / 4);

      return new[]
      {
        left, bottom,
        right, bottom,
        right - inset, top,
        left + inset, top
      };
    }

    public static Point GetTopLeft(int[] points)
    {
      int minX = points[0];
      int minY = points[1];
      for (int i = 2; i < 8; i += 2)
      {
        minX = Math.Min(minX, points[i]);
        minY = Math.Min(minY, points[i + 1]);
      }
      return new Point(minX, minY);
    }

    public static int GetWidth(int[] points)
    {
      int minX = points[0];
      int maxX = points[0];
      for (int i = 2; i < 8; i += 2)
      {
        minX = Math.Min(minX, points[i]);
        maxX = Math.Max(maxX, points[i]);
      }
      return maxX - minX;
    }

    public static int GetHeight(int[] points)
    {
      int minY = points[1];
      int maxY = points[1];
      for (int i = 3; i < 8; i += 2)
      {
        minY = Math.Min(minY, points[i]);
        maxY = Math.Max(maxY, points[i]);
      }
      return maxY - minY;
    }

    public static PointF[] ToPolygonPoints(int[] points)
    {
      return new[]
      {
        new PointF(points[0], points[1]),
        new PointF(points[2], points[3]),
        new PointF(points[4], points[5]),
        new PointF(points[6], points[7])
      };
    }
  }
}
