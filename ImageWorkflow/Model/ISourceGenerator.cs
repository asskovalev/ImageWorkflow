using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageWorkflow.Model
{
	public interface ISourceGenerator
	{
		string Name { get; }
		Size Size { get; }

		void Configure(Size size, params object[] parameters);
		Bitmap Generate();
	}
}
