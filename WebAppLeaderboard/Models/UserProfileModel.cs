using Common.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppLeaderboard.Models
{
    public class UserProfileModel : PageModel
    {
        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public Segment Segment { get; set; }

        [BindProperty]
        public Activity Leaderboard { get; set; }

        [BindProperty]
        public float Points { get; set; }

        [BindProperty]
        public Dictionary<string, KeyValuePair<float,long>> dict { get; set; }

        [BindProperty]
        public List<ActivityRequest> Activities { get; set; }

        public UserProfileModel()
        {
        }
        public UserProfileModel(string userId)
        {
            UserId = userId;
        }
        public UserProfileModel(string userId, Segment segment)
        {
            UserId = userId;
            Segment = segment;
        }

    }

    public enum Segment
    {
        Local,
        Sports,
        Autos,
        Travel,
        Health,
        Entertainment,
        Traffic,
        News,
        Shopping,
    }

    public enum Activity
    {
        Like,
        Comment,
        PhotoUpload,
        AddPhotoFeedback,
        NameFeedback,
        AddressFeedback,
        PhoneFeedback,
        HOOFeedback,
        CloseFeedback,
        TempCloseFeedback,
        WebsiteFeedback,
        ImageFeedback,
        AmenityFeedback,
    }
}
