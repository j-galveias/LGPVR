using System.Collections.Generic;

[System.Serializable]
public class AnimatedSignTrack
{
    public List<string> properties { get; set; } = new List<string>();
    public bool loops { get; set; } = false; //Used for hand animations

    public List<AnimatedSignKey> keyFrames { get; set; } = new List<AnimatedSignKey>();
}