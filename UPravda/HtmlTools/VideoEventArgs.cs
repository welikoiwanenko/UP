using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTools
{
    public class VideoEventArgs
    {
		private string videoSource;

		public string VideoSource
		{
			get { return videoSource; }
			set { videoSource = value; }
		}

        public VideoEventArgs(string videoSource)
        {
            // TODO: Complete member initialization
            this.videoSource = videoSource;
        }
    }
}
