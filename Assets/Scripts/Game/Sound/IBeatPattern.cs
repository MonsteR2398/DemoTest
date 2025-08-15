using System.Collections.Generic;

public interface IBeatPattern
{
    /// Вернёт все точки (в битах), попадающие в полуоткрытый интервал (fromBeat, toBeat]
    IEnumerable<double> NextBeatsInRange(double fromBeatExclusive, double toBeatInclusive);
}

/// Явная последовательность: 1, 3, 5, 10, 3.5 ...
public sealed class SequencePattern : IBeatPattern
{
    private readonly double[] _beats;
    public SequencePattern(params double[] beats) => _beats = beats ?? new double[0];

    public IEnumerable<double> NextBeatsInRange(double from, double to)
    {
        foreach (var b in _beats)
            if (b > from && b <= to) yield return b;
    }
}

/// Каждые N тактов, с фазой offset (может быть дробной)
public sealed class EveryNBeatsPattern : IBeatPattern
{
    private readonly double _n;
    private readonly double _offset;
    public EveryNBeatsPattern(double n, double offset = 0)
    {
        _n = System.Math.Max(1e-6, n);
        _offset = offset;
    }

    public IEnumerable<double> NextBeatsInRange(double from, double to)
    {
        // найти первое кратное точке в диапазоне
        // k такое, что b = offset + k*n > from
        double kStart = System.Math.Floor((from - _offset) / _n) + 1;
        for (double k = kStart;; k += 1)
        {
            double b = _offset + k * _n;
            if (b > to) yield break;
            yield return b;
        }
    }
}

/// Шаговый паттерн: каждые step битов (step может быть 0.5, 0.25, и т.п.)
public sealed class StepPattern : IBeatPattern
{
    private readonly double _step;
    private readonly double _start;
    public StepPattern(double step, double start = 0)
    {
        _step = System.Math.Max(1e-6, step);
        _start = start;
    }
    public IEnumerable<double> NextBeatsInRange(double from, double to)
    {
        double kStart = System.Math.Floor((from - _start) / _step) + 1;
        for (double k = kStart;; k += 1)
        {
            double b = _start + k * _step;
            if (b > to) yield break;
            yield return b;
        }
    }
}
