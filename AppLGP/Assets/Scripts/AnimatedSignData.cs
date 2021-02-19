using System.Collections.Generic;

public class AnimatedSignData
{
    public List<AnimatedSignTrack> animTracks { get; set; } = new List<AnimatedSignTrack>();
    public float version { get; set; } = 0;
    public float globalSpeed { get; set; } = 1;
    public int globalRepetitions { get; set; } = 1;
}