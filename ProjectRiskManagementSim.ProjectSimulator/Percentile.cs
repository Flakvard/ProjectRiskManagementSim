using ProjectRiskManagementSim.ProjectSimulation;

public static class EnumerableExtensions
{
    public static double Percentile(this IEnumerable<double> source, double percentile)
    {
        if (percentile < 0 || percentile > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 1.");
        }

        var list = source.ToList();
        if (!list.Any())
        {
            throw new InvalidOperationException("Cannot compute percentile for an empty set.");
        }

        int n = list.Count;
        double position = percentile * (n - 1);

        int k = (int)position;
        double fractional = position - k;

        // Find the k-th smallest element (0-indexed) in O(n) time
        double lower = QuickSelect(list, k);

        // If the fractional part is zero, return the exact element
        if (fractional == 0)
        {
            return lower;
        }

        // Otherwise, interpolate with the next element
        double upper = QuickSelect(list, k + 1);
        return lower + fractional * (upper - lower);
    }

    // QuickSelect algorithm for finding the k-th smallest element
    private static double QuickSelect(List<double> list, int k)
    {
        if (list.Count == 1)
        {
            return list[0];
        }

        var pivot = list[ThreadSafeRandom.ThisThreadsRandom.Next(list.Count)];

        var lows = list.Where(x => x < pivot).ToList();
        var highs = list.Where(x => x > pivot).ToList();
        var pivots = list.Where(x => x == pivot).ToList();

        if (k < lows.Count)
        {
            return QuickSelect(lows, k);
        }
        else if (k < lows.Count + pivots.Count)
        {
            return pivots[0]; // We found the pivot
        }
        else
        {
            return QuickSelect(highs, k - lows.Count - pivots.Count);
        }
    }
}

