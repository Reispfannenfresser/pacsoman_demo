namespace Example.Scripts;

public static class Interpolation
{
    // Taken from Freya Holmér
    // https://www.youtube.com/watch?v=LSNQuFEDOyQ
    public static double ExpDecay(double a, double b, double decay, double dt)
    {
        return b + ((a - b) * Math.Exp(-decay * dt));
    }
}
