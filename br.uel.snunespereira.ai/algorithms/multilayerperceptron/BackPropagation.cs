using System;
using System.Text;
using br.uel.snunespereira.ai.shared;
using System.Linq;
using System.Collections.Generic;

namespace br.uel.snunespereira.ai
{
	/// <summary>
	/// Back propagation class
	/// </summary>
	public class BackPropagation : AlgorithmBase
	{
		/// <summary>
		/// Property that contains all perceptrons from input layer
		/// </summary>
		/// <value>The input layer.</value>
		protected Perceptron[] InputLayer { get; set; }

		/// <summary>
		/// Property that contains all perceptrons from hidden layer
		/// </summary>
		/// <value>The hidden layer.</value>
		protected Perceptron[] HiddenLayer { get; set; }

		/// <summary>
		/// Property that contains all perceptrons from output layer
		/// </summary>
		/// <value>The output layer.</value>
		protected Perceptron[] OutputLayer { get; set; }

		/// <summary>
		/// Property that contain all weights that should be used in the process.
		/// </summary>
		/// <value>The weights.</value>
		protected double[] Weights { get; set; }

		/// <summary>
		/// Gets or sets the learn rate.
		/// </summary>
		/// <value>The learn rate.</value>
		protected double LearnRate { get; set; }

		/// <summary>
		/// Gets or sets the momentum.
		/// </summary>
		/// <value>The momentum.</value>
		protected double Momentum { get; set; }

		/// <summary>
		/// Gets or sets the error threshold.
		/// </summary>
		/// <value>The error threshold.</value>
		protected double ErrorThreshold { get; set; }

		/// <summary>
		/// Gets or sets the type of the hidden activation.
		/// </summary>
		/// <value>The type of the hidden activation.</value>
		private FunctionType HiddenActivationType { get; set; }

		/// <summary>
		/// Gets or sets the type of the output activation.
		/// </summary>
		/// <value>The type of the output activation.</value>
		private FunctionType OutputActivationType { get; set; }



		/// <summary>
		/// Initializes a new instance of the <see cref="br.uel.snunespereira.ai.BackPropagation"/> class.
		/// </summary>
		/// <param name="input">How many perceptrons in input layer</param>
		/// <param name="hidden">How many perceptrons in hidden layer</param>
		/// <param name="output">How many perceptrons in output layer</param>
		public BackPropagation (int input, int hidden, int output, double learnRate, double momentum, 
																			double errorThreshold)
		{
			this.InputLayer = new Perceptron[input];
			this.HiddenLayer = new Perceptron[hidden];
			this.OutputLayer = new Perceptron[output];
			this.Weights = new double[(input * hidden) + (hidden * output) + (hidden + output)];
			this.LearnRate = learnRate;
			this.Momentum = momentum;
			this.ErrorThreshold = errorThreshold;

			// initialize weights
			InitWeights ();

			// initialize network
			InitNetwork ();
		}

		/// <summary>
		/// Method responsible for giving random values to weights
		/// </summary>
		private void InitWeights()
		{
			// creates a random object
			Random rnd = new Random ();

			// for each weight
			for (int x = 0; x < this.Weights.Length; x++)
				this.Weights [x] = rnd.NextDouble() / 10; // gets an arbitrary number
		}

		/// <summary>
		/// Inits the network.
		/// </summary>
		private void InitNetwork()
		{
			for (int x = 0; x < this.InputLayer.Length; x++)
				this.InputLayer [x] = new Perceptron (PerceptronType.Input);

			for (int x = 0; x < this.HiddenLayer.Length; x++)
				this.HiddenLayer [x] = new Perceptron (PerceptronType.Hidden);

			for (int x = 0; x < this.OutputLayer.Length; x++)
				this.OutputLayer [x] = new Perceptron (PerceptronType.Output);
		}

		public void BuildNetwork(string[][] data, shared.Attribute[] listAttributes,
																			shared.Attribute categoricalAttribute)
		{
			// to each epoch (we are assuming that each training data is an epoch)
			foreach(var line in data)
			{
				int index = 0;
				foreach (string value in line) {
					if (index < listAttributes.Count()) {
						if (listAttributes [index].Type == typeof(double))
							this.InputLayer[index].ComputeOutput(double.Parse (value));
						else
							this.InputLayer[index].ComputeOutput(listAttributes [index].Values.IndexOf (value));
						index++;
					}
				}
			}
		}

		public void TestNetwork(StringBuilder executor, string[][] data, shared.Attribute categoricalAttribute)
		{
		}

		/// <summary>
		/// Method that executes the process.
		/// </summary>
		/// <param name="data">Array of string containg data.</param>
		public static StringBuilder Execute(string[] data)
		{
			// creates a string builder
			StringBuilder execution = new StringBuilder();

			int size = data.Length - data.ToList().FindIndex(m => m.ToString().Trim().ToLower() == "@data");
			List<shared.Attribute> attributeList = AlgorithmBase.GetAttributes(data);

			// print some info
			execution.AppendLine();
			execution.AppendLine();
			execution.AppendFormat("The file has {0} entries", size);
			execution.AppendLine();
			execution.AppendLine("The attributes are: ");

			// for each founded attribute
			foreach (shared.Attribute att in attributeList)
				execution.AppendFormat("- {0}\n", att.Name);

			// format the data to usage
			string[][] filteredData =
				data.Skip(data.ToList().FindIndex(m => m.ToString().Trim().ToLower() == "@data") + 1)
					.ToList()
					.ConvertAll<string[]>(i => i.Split(new string[] { "," }, 
						StringSplitOptions.RemoveEmptyEntries) ).ToArray();

			// creates the neural network
			BackPropagation bpg = new BackPropagation(attributeList.Count - 1, 
				(int)Math.Abs((Math.Sqrt(attributeList.Count - 1 * 1))), 1, 0.5, 0.1, 0.00001);

			// builds up network
			bpg.BuildNetwork (filteredData, attributeList.Take(attributeList.Count() -1).ToArray(), attributeList.Last());

			// tests network
			bpg.TestNetwork (execution, filteredData, attributeList.Last());

			return execution;
		}
	}
}