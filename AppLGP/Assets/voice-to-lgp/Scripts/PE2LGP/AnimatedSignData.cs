using System.Collections.Generic;
using System.Linq;

public class AnimatedSignData
{
    public List<AnimatedSignTrack> animTracks { get; set; } = new List<AnimatedSignTrack>();
    public Dictionary<string, string> linguisticTags { get; set; } = new Dictionary<string, string>();
    public string version { get; set; } = "0";
    public string globalSpeed { get; set; } = "1";
    public string globalRepetitions { get; set; } = "1";

    //This looks for a track that contains a certain property. Note that it can contain keyframes without that property!
    public AnimatedSignTrack FindTrack(string property)
    {
        return animTracks.Find(t => t.properties.Contains(property));
    }

    //This looks for a track with a certain property, and also filters out the keyframes why don't have that property.
    public List<AnimatedSignKey> FindTrackFiltered(string property)
    {
        return FindTrack(property).keyFrames.Where(k => k.property == property).ToList();
    }

    //This finds a track with a certain property, filters out keyframes without that property and sorts them by time.
    public List<AnimatedSignKey> FindTrackFilteredSorted(string property)
    {
        var keyframes = FindTrack(property).GetSortedKeyframes();
        return keyframes.Where(k => k.property == property).ToList();
    }

    public AnimatedSignTrack FindTrackMuscles()
    {
        return FindTrack(AnimatedSign.MUSCLES);
    }

    public AnimatedSignTrack FindTrackLeftHand()
    {
        return FindTrack(AnimatedSign.LEFT_HAND_CONFIG);
    }

    public AnimatedSignTrack FindTrackRightHand()
    {
        return FindTrack(AnimatedSign.RIGHT_HAND_CONFIG);
    }

    public AnimatedSignTrack FindTrackFace()
    {
        return FindTrack(AnimatedSign.FACIAL_EXPRESSION);
    }

    public HashSet<string> GetHandPoses()
    {
        HashSet<string> handPoses = new HashSet<string>();
        List<AnimatedSignKey> leftHand = FindTrackFiltered(AnimatedSign.LEFT_HAND_CONFIG);
        List<AnimatedSignKey> rightHand = FindTrackFiltered(AnimatedSign.RIGHT_HAND_CONFIG);

        foreach (AnimatedSignKey keyframe in leftHand)
        {
            handPoses.Add(keyframe.GetHandName());
        }

        foreach (AnimatedSignKey keyframe in rightHand)
        {
            handPoses.Add(keyframe.GetHandName());
        }

        return handPoses;
    }
}