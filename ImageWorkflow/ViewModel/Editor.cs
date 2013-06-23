using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageWorkflow.Model;

namespace ImageWorkflow.ViewModel
{
	public class Editor : ObservableObject
	{
		private List<Workflow> loadedWorkflows;
		public List<Workflow> LoadedWorkflows
		{
			get
			{
				loadedWorkflows = loadedWorkflows ?? new List<Workflow>();
				return loadedWorkflows;
			}
		}

		public void Load(Workflow wf)
		{
			LoadedWorkflows.Add(wf);
			RaisePropertyChanged(() => LoadedWorkflows);
		}
	}
}
