using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICMediaPlayer.Models
{
    public class MySettings
    {
        /*
         * Settings are used to configure the player
         * MediaPlayerURL is the API to pull slides from
         * SlideInterval is how fast the slides change in SECONDS
         * PollInterval is how often the database is polled for
         * new slides in MINUTES
         */
        public string MediaPlayerAPIUrl { get; set; }
        public int SlideInterval { get; set; }
        public int PollInterval { get; set; }

    }
}
