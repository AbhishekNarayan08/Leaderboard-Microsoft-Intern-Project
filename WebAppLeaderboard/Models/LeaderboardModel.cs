using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppLeaderboard.Models
{
    public class LeaderboardModel : PageModel
    {
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public Dictionary<string, double> Segmenttopn { get; set; }
        [BindProperty]
        public Dictionary<string, double> Activitytopn { get; set; }
        [BindProperty]
        public long Segmentrank { get; set; }
        [BindProperty]
        public long Activityrank { get; set; }
        [BindProperty]
        public Segment Segment { get; set; }

        [BindProperty]
        public string Leaderboard { get; set; }
        public LeaderboardModel()
        {
        }
    }
}
