using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;

namespace ImageWorkflow.Model
{
	[Export(typeof(ITransformation))]
	public class IdTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "Id"; }
		}

		public override Bitmap Apply(Bitmap source)
		{
			return source;
		}
	}
}
