using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageWorkflow.Model
{
	public class Workflow : ITransformation
	{
		public string Name { get; set; }

		public List<ITransformation> Transformations { get; set; }


		public Bitmap Apply(Bitmap source)
		{
			return Transformations.Aggregate(source, (acc, tr) => tr.Apply(acc));
		}
	}
}
