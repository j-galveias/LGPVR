using System.Collections.Generic;

public class AnimatedSignTrack
{
    public List<string> properties { get; set; } = new List<string>();
    public List<AnimatedSignKey> keyFrames { get; set; } = new List<AnimatedSignKey>();
}