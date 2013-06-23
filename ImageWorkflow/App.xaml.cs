using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ImageWorkflow.ViewModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace ImageWorkflow
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			MainWindow app = new MainWindow();
			WorkflowViewModel viewModel = new WorkflowViewModel();
			viewModel.Workflow = new Model.Workflow();
			viewModel.Workflow.Transformations = new List<Model.ITransformation>();
			AssembleComponents(viewModel);

			app.DataContext = viewModel;
			app.Show();
		}

		public void AssembleComponents<T>(T target)
		{
			try
			{
				var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
				var container = new CompositionContainer(catalog);
				container.ComposeParts(target);
			}
			catch
			{
				throw;
			}
		}
	}
}
