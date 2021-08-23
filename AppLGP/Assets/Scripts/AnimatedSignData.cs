using System.Collections.Generic;

public class AnimatedSignData
{
    public List<AnimatedSignTrack> animTracks { get; set; } = new List<AnimatedSignTrack>();
    public Dictionary<string, string> linguisticTags { get; set; } = new Dictionary<string, string>();
    public string version { get; set; } = "0";
    public string globalSpeed { get; set; } = "1";
    public string globalRepetitions { get; set; } = "1";
}