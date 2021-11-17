using System.Collections.Generic;

public class AnimatedSignTrack
{
    public List<string> properties { get; set; } = new List<string>();
    public string loops { get; set; } = "false"; //Used for hand animations

    public List<AnimatedSignKey> keyFrames { get; set; } = new List<AnimatedSignKey>();

    public List<AnimatedSignKey> GetSortedKeyframes()
    {
        keyFrames.Sort();
        return keyFrames;
    }
}