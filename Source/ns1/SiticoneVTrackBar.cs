using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ns1
{
	[ToolboxBitmap(typeof(TrackBar))]
	[ToolboxItem(true)]
	[DebuggerStepThrough]
	[Description("A vertical TrackBar Control")]
	public class SiticoneVTrackBar : SiticoneTrackBar
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style |= 1;
				return createParams;
			}
		}
	}
}
