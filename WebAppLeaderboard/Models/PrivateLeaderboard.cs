using Common.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppLeaderboard.Models
{
    public class PrivateLeaderboard:PageModel
    {
        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string UserId1 { get; set; }

        [BindProperty]
        public string UserId2 { get; set; }

        [BindProperty]
        public string UserId3 { get; set; }

        [BindProperty]
        public string UserId4 { get; set; }

        [BindProperty]
        public string PrivateLeaderboardName { get; set; }

        [BindProperty]
        public Segment Segment { get; set; }

        [BindProperty]
        public string Leaderboard { get; set; }

        [BindProperty]
        public List<KeyValuePair<string,Dictionary<string, double>>> leaderboards { get; set; }

        public PrivateLeaderboard()
        {
        }
        public PrivateLeaderboard(string userId)
        {
            UserId = userId;
        }
        public PrivateLeaderboard(string userId, Segment segment)
        {
            UserId = userId;
            Segment = segment;
        }
    }
}
