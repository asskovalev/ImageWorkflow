using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageWorkflow.Model;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace ImageWorkflow.ViewModel
{
	public class WorkflowViewModel : ObservableObject
	{
		public Workflow Workflow { get; set; }

		public ObservableCollection<ITransformation> Transformations
		{
			get { return new ObservableCollection<ITransformation>(Workflow.Transformations); }
		}


		[ImportMany(typeof(ITransformation))]
		public List<ITransformation> AvailableTransformations { get; set; }

		[ImportMany(typeof(ISourceGenerator))]
		public List<ISourceGenerator> AvailableSourceGenerators { get; set; }



		private ISourceGenerator currentSourceGenerator;
		public ISourceGenerator CurrentSourceGenerator
		{
			get { return currentSourceGenerator; }
			set
			{
				currentSourceGenerator = value;
				RaisePropertyChanged(() => CurrentSourceGenerator);
			}
		}

		private ITransformation currentAvailableTransformation;
		public ITransformation CurrentAvailableTransformation
		{
			get { return currentAvailableTransformation; }
			set
			{
				currentAvailableTransformation = value;
				RaisePropertyChanged(() => CurrentAvailableTransformation);
			}
		}

		private ITransformation currentTransformation;
		public ITransformation CurrentTransformation
		{
			get { return currentTransformation; }
			set
			{
				currentTransformation = value;
				RaisePropertyChanged(() => CurrentTransformation);
			}
		}

		public BitmapImage testResult;
		public BitmapImage TestResult
		{
			get { return testResult; }
			private set
			{
				testResult = value;
				RaisePropertyChanged(() => TestResult);
			}
		}


		ICommand _AddTransformationCommand;
		public ICommand AddTransformationCommand
		{
			get
			{
				if (_AddTransformationCommand == null)
				{
					_AddTransformationCommand = new RelayCommand(
						p => AddTransformation(),
						p => CurrentAvailableTransformation != null);
				}
				return _AddTransformationCommand;
			}

		}

		ICommand _RemoveTransformationCommand;
		public ICommand RemoveTransformationCommand
		{
			get
			{
				if (_RemoveTransformationCommand == null)
				{
					_RemoveTransformationCommand = new RelayCommand(
						p => RemoveTransformation(),
						p => CurrentTransformation != null);
				}
				return _RemoveTransformationCommand;
			}

		}

		ICommand _TestWorkflowCommand;
		public ICommand TestWorkflowCommand
		{
			get
			{
				if (_TestWorkflowCommand == null)
				{
					_TestWorkflowCommand = new RelayCommand(
						p => TestWorkflow(),
						p => Transformations.Count() > 0 && CurrentSourceGenerator != null);
				}
				return _TestWorkflowCommand;
			}

		}


		private void AddTransformation()
		{
			Workflow.Transformations.Add(CurrentAvailableTransformation);
			RaisePropertyChanged(() => Transformations);

			//TestResult = null;

			TestWorkflow();
		}

		private void RemoveTransformation()
		{
			var currentIndex = Workflow.Transformations.IndexOf(CurrentTransformation);

			Workflow.Transformations.Remove(CurrentTransformation);
			RaisePropertyChanged(() => Transformations);

			currentIndex = currentIndex == Workflow.Transformations.Count
				? currentIndex - 1 : currentIndex;

			CurrentTransformation = Workflow.Transformations.ElementAtOrDefault(currentIndex);
			RaisePropertyChanged(() => CurrentTransformation);

			//TestResult = null;

			TestWorkflow();
		}

		private void TestWorkflow()
		{
			var src = CurrentSourceGenerator
				.Do(it => it.Configure(new Size(50, 30)))
				.Generate();

			MemoryStream memoryStream = new MemoryStream();
			Workflow.Apply(src).Save(memoryStream, ImageFormat.Png);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
			bitmapImage.EndInit();

			TestResult = bitmapImage;

			RaisePropertyChanged(() => TestResult);
		}
	}
}
