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
		#region Properties

		/// <summary>
		/// Property that contains all perceptrons from input layer
		/// </summary>
		/// <value>The input layer.</value>
		protected int InputLayer { get; set; }

		/// <summary>
		/// Property that contains all perceptrons from hidden layer
		/// </summary>
		/// <value>The hidden layer.</value>
		protected int HiddenLayer { get; set; }

		/// <summary>
		/// Property that contains all perceptrons from output layer
		/// </summary>
		/// <value>The output layer.</value>
		protected int OutputLayer { get; set; }

		/// <summary>
		/// Property that contain all weights that should be used in the process.
		/// </summary>
		/// <value>The weights.</value>
		protected double[,] InputWeights { get; set; }

		/// <summary>
		/// Gets or sets the output weights.
		/// </summary>
		/// <value>The output weights.</value>
		protected double[,] OutputWeights { get; set; }

		/// <summary>
		/// Gets or sets the hidden biases.
		/// </summary>
		/// <value>The input biases.</value>
		protected double[] HiddenBiases { get; set; }

		/// <summary>
		/// Gets or sets the output biases.
		/// </summary>
		/// <value>The output biases.</value>
		protected double[] OutputBiases { get; set; }

		/// <summary>
		/// Gets or sets the output gradients.
		/// </summary>
		/// <value>The output gradients.</value>
		protected double[] OutputGradients { get; set; }

		/// <summary>
		/// Gets or sets the hidden gradients.
		/// </summary>
		/// <value>The hidden gradients.</value>
		protected double[] HiddenGradients { get; set; }

		/// <summary>
		/// Gets or sets the input to hidden deltas.
		/// </summary>
		/// <value>The input to hidden deltas.</value>
		protected double[,] InputToHiddenDeltas { get; set; }

		/// <summary>
		/// Gets or sets the hidden to output deltas.
		/// </summary>
		/// <value>The hidden to output deltas.</value>
		protected double[,] HiddenToOutputDeltas { get; set; }

		/// <summary>
		/// Gets or sets the hidden bias deltas.
		/// </summary>
		/// <value>The hidden bias deltas.</value>
		protected double[] HiddenBiasDeltas { get; set; }

		/// <summary>
		/// Gets or sets the output bias deltas.
		/// </summary>
		/// <value>The output bias deltas.</value>
		protected double[] OutputBiasDeltas { get; set; }

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

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="br.uel.snunespereira.ai.BackPropagation"/> class.
		/// </summary>
		/// <param name="input">How many perceptrons in input layer</param>
		/// <param name="hidden">How many perceptrons in hidden layer</param>
		/// <param name="output">How many perceptrons in output layer</param>
		public BackPropagation (int input, int hidden, int output, double learnRate, double momentum, 
																			double errorThreshold)
		{
			this.InputLayer = input;
			this.HiddenLayer = hidden;
			this.OutputLayer = output;
			this.LearnRate = learnRate;
			this.Momentum = momentum;
			this.ErrorThreshold = errorThreshold;

			// initialize weights
			InitWeights ();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Builds the network.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="listAttributes">List attributes.</param>
		/// <param name="categoricalAttribute">Categorical attribute.</param>
		public void BuildNetwork(string[][] data, shared.Attribute[] listAttributes,
																			shared.Attribute categoricalAttribute)
		{
			double error = 0;

			// to each epoch (we are assuming that each training data line is an epoch)
			foreach (var line in data) {

				int epoch = 0;
				int index = 0;
				List<double> inputValues = new List<double> ();

				double[] targetValues = new double[1] 
				{ categoricalAttribute.Values.IndexOf (line [categoricalAttribute.Index]) };

				foreach (string value in line) {
					if (index < listAttributes.Count ()) {
						if (listAttributes [index].Values.Count () > 0)
							inputValues.Add (listAttributes [index].Values.IndexOf (value));
						else
							inputValues.Add (double.Parse (value));
						index++;
					}
				}

				while (epoch < 10000) {

					double[] hiddenOut;
					double[] outputOut = this.ComputeOutputs (inputValues.ToArray (), out hiddenOut);

					// get the error
					error = this.GetError (targetValues, outputOut);

					// if current error is less the the defined error threshold
					if (error < this.ErrorThreshold) {
						break;
					}

					// update the weights
					this.UpdateWeights (inputValues.ToArray (), hiddenOut, outputOut, targetValues);

					epoch++;
				}

				// if current error is less the the defined error threshold
				if (error < this.ErrorThreshold) {
					break;
				}
			}
		}

		/// <summary>
		/// Tests the network.
		/// </summary>
		/// <param name="executor">Executor.</param>
		/// <param name="data">Data.</param>
		/// <param name="categoricalAttribute">Categorical attribute.</param>
		public void TestNetwork(StringBuilder execution, string[][] data, shared.Attribute[] listAttributes,
																				shared.Attribute categoricalAttribute)
		{
			int success = 0, failure = 0;

			// to each epoch (we are assuming that each training data line is an epoch)
			foreach (var line in data) {
			
				int index = 0;
				List<double> inputValues = new List<double> ();

				double[] targetValues = new double[1] 
				{ categoricalAttribute.Values.IndexOf (line [categoricalAttribute.Index]) };

				foreach (string value in line) {
					if (index < listAttributes.Count ()) {
						if (listAttributes [index].Values.Count () > 0)
							inputValues.Add (listAttributes [index].Values.IndexOf (value));
						else
							inputValues.Add (double.Parse (value));
						index++;
					}
				}

				double[] hiddenOut;
				double[] outputOut = this.ComputeOutputs (inputValues.ToArray (), out hiddenOut);

				if (Math.Round (outputOut.First (), 0) == targetValues.First ())
					success++;
				else
					failure++;

			}

			execution.AppendLine();
			execution.AppendFormat("Successful: {0}\n", success);
			execution.AppendFormat("Failure: {0}\n", failure);
			execution.AppendFormat("Rate: {0}%\n", Math.Ceiling((double)success / (double)data.Length * 100));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Method responsible for giving random values to weights
		/// </summary>
		private void InitWeights()
		{
			// creates a random object
			Random rnd = new Random ();

			// create the weights and gradients
			this.InputWeights = new double[this.HiddenLayer,this.InputLayer];
			this.HiddenBiases = new double[this.HiddenLayer];
			this.OutputWeights = new double[this.OutputLayer, this.HiddenLayer];
			this.OutputBiases = new double[this.OutputLayer];
			this.OutputGradients = new double[this.OutputLayer];
			this.HiddenGradients = new double[this.HiddenLayer];
			this.InputToHiddenDeltas = new double[this.InputLayer, this.HiddenLayer];
			this.HiddenToOutputDeltas = new double[this.HiddenLayer, this.OutputLayer];
			this.HiddenBiasDeltas = new double[this.HiddenLayer];
			this.OutputBiasDeltas = new double[this.OutputLayer];

			// for each neuron in hidden layer
			for (int x = 0; x < this.HiddenLayer; x++)
				for (int y = 0; y < this.InputLayer; y++) // for each neuron in input layer
					this.InputWeights [x,y] = rnd.NextDouble() / 10; // gets an arbitrary number

			// for each neuron in hidden layer
			for (int x = 0; x < this.HiddenLayer; x++)
				this.HiddenBiases[x] = rnd.NextDouble() / 10; // gets an arbitrary number

			// for each neuron in output layer
			for (int x = 0; x < this.OutputLayer; x++)
				for (int y = 0; y < this.HiddenLayer; y++) // for each neuron in input layer
					this.OutputWeights [x,y] = rnd.NextDouble() / 10; // gets an arbitrary number

			// for each neuron in output layer
			for (int x = 0; x < this.OutputLayer; x++)
				this.OutputBiases[x] = rnd.NextDouble() / 10; // gets an arbitrary number
		}

		/// <summary>
		/// Computes the outputs.
		/// </summary>
		/// <returns>The outputs.</returns>
		/// <param name="values">Values.</param>
		/// <param name="hiddenOut">Hidden out.</param>
		private double[] ComputeOutputs(double[] values, out double[] hiddenOut)
		{
			#region Hidden Layer

			// create the array to sum the values and output values in hidden layer
			double[] hiddenSums = new double[this.HiddenLayer];
			hiddenOut = new double[this.HiddenLayer];

			// for each neuron in hidden layer
			for (int j = 0; j < this.HiddenLayer; j++) 
			{
				// for each neuron in input layer
				for (int i = 0; i < this.InputLayer; i++) 
				{
					// sums the input values and multiplies to input weights
					hiddenSums [j] += values [i] * this.InputWeights [j,i];
				}

				// apply the current biases
				hiddenSums [j] += HiddenBiases [j];

				// apply the activation function (in this layer is tanh)
				hiddenOut[j] = this.HyperTanFunction(hiddenSums[j]);
			}

			#endregion

			#region Output Layer

			// create the array to sum the values and output values in hidden layer
			double[] outputSums = new double[this.OutputLayer];
			double[] outputOut = new double[this.OutputLayer];

			// for each neuron in output layer
			for (int j = 0; j < this.OutputLayer; j++) 
			{
				// for each neuron in hidden layer
				for (int i = 0; i < this.HiddenLayer; i++) 
				{
					// sums the input values and multiplies to output weights
					outputSums [j] += hiddenOut[j] * this.OutputWeights [j,i];
				}

				// apply the current biases
				outputSums [j] += OutputBiases [j];

				// apply the activation function (in this layer is sigmoid)
				outputOut[j] = this.SigmoidFunction(outputSums[j]);
			}

			#endregion

			return outputOut;
		}

		/// <summary>
		/// Updates the weights.
		/// </summary>
		/// <param name="inputValues">Input values.</param>
		/// <param name="hiddenValues">Hidden values.</param>
		/// <param name="outputValues">Output values.</param>
		/// <param name="targetValues">Target values.</param>
		private void UpdateWeights(double[] inputValues, double[] hiddenValues, double[] outputValues, double[] targetValues)
		{
			#region Computes Gradients

			// apply the error gradient on output layer
			for (int x = 0; x < this.OutputLayer; x++) 
			{
				this.OutputGradients[x] = ((1 - outputValues[x]) * outputValues[x]) * (targetValues[x] - outputValues[x]);
			}

			// apply the error gradient on hidden layer
			for (int x = 0; x < this.HiddenLayer; x++) 
			{
				// derivative of tanh is (1-y)(1+y)
				double derivative = (1 - hiddenValues[x]) * (1 + hiddenValues[x]); 
				double sum = 0.0;

				// each hidden delta is the sum of numOutput terms
				for (int y = 0; y < this.OutputLayer; y++) {
					// each downstream gradient * outgoing weight
					sum += OutputGradients [y] * OutputWeights [y, x]; 
				}

				// hGrad = (1-O)(1+O) * E(oGrads*oWts)
				HiddenGradients[x] = derivative * sum; 
			}

			#endregion

			#region Computes Weights

			// for each weight in input layer
			for(int x = 0; x < this.InputWeights.GetLength(1); x++)
			{
				// for each weight in hidden layer
				for(int y = 0; y < this.InputWeights.GetLength(0); y++)
				{
					// compute the new delta = "eta * hGrad * input"
					double delta = this.LearnRate * this.HiddenGradients[y] * inputValues[x]; 

					// add the delta to input weights
					this.InputWeights[y, x] += delta;

					// add the momentum * previous delta
					this.InputWeights[y, x] += this.Momentum * this.InputToHiddenDeltas[x, y];

					// preserves the previous delta
					this.InputToHiddenDeltas[x, y] = delta;
				}
			}

			// for each weight in input layer
			for(int x = 0; x < this.OutputWeights.GetLength(1); x++)
			{
				// for each weight in hidden layer
				for(int y = 0; y < this.OutputWeights.GetLength(0); y++)
				{
					// compute the new delta = "eta * hGrad * input"
					double delta = this.LearnRate * this.OutputGradients[y] * hiddenValues[x]; 

					// add the delta to input weights
					this.OutputWeights[y, x] += delta;

					// add the momentum * previous delta
					this.OutputWeights[y, x] += this.Momentum * this.HiddenToOutputDeltas[x, y];

					// preserves the previous delta
					this.HiddenToOutputDeltas[x, y] = delta;
				}
			}

			#endregion

			#region Computes Biases

			// for each item in hidden layer
			for(int x = 0; x < this.HiddenLayer; x++)
			{
				double delta = this.LearnRate * this.HiddenGradients[x] * 1.0;

				// apply the delta to hidden biases
				this.HiddenBiases[x] += delta;

				// apply the momentum * previous delta
				this.HiddenBiases[x] += this.Momentum * this.HiddenBiasDeltas[x];

				// saves the previous delta
				this.HiddenBiasDeltas[x] = delta;
			}

			// for each item in output layer
			for(int x = 0; x < this.OutputLayer; x++)
			{
				double delta = this.LearnRate * this.OutputGradients[x] * 1.0;

				// apply the delta to hidden biases
				this.OutputBiases[x] += delta;

				// apply the momentum * previous delta
				this.OutputBiases[x] += this.Momentum * this.OutputBiasDeltas[x];

				// saves the previous delta
				this.OutputBiasDeltas[x] = delta;
			}

			#endregion
		}

		private double GetError(double[] targetValues, double[] outputValues)
		{
			double sum = 0.0;

			for (int i = 0; i < targetValues.Length; ++i)
				sum += (targetValues[i] - outputValues[i]) * (targetValues[i] - outputValues[i]);

			return Math.Sqrt(sum);
		}

		/// <summary>
		/// Function Log-Sigmoid.
		/// </summary>
		/// <returns>The function.</returns>
		/// <param name="x">The x coordinate.</param>
		private double SigmoidFunction(double x)
		{
			if (x < -45.0) return 0.0;
			else if (x > 45.0) return 1.0;
			else return 1.0 / (1.0 + Math.Exp(-x));
		}

		/// <summary>
		/// Function HyperTan.
		/// </summary>
		/// <returns>The tan function.</returns>
		/// <param name="x">The x coordinate.</param>
		private double HyperTanFunction(double x)
		{
			if (x < -45.0) return -1.0;
			else if (x > 45.0) return 1.0;
			else return Math.Tanh(x);
		}

		#endregion

		#region Static Methods

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
			string[] cleanData =
				data.Skip (data.ToList ().FindIndex (m => m.ToString ().Trim ().ToLower () == "@data") + 1)
					.ToList ().ToArray();

			int numberFolds = 10;

			execution.AppendLine();
			execution.AppendFormat("Crossfolding data. Number of folds: {0}", numberFolds);
			execution.AppendLine();

			// crossfold data to usage
			List<string[]> crossfoldedData = AlgorithmBase.CrossfoldData (cleanData, numberFolds);

			bool[] usedFolds = new bool[numberFolds];
			Random rnd = new Random ();


			// for each fold
			for (int fold = 0; fold < numberFolds; fold++) {

				// creates the neural network
				BackPropagation bpg = new BackPropagation(attributeList.Count - 1, 
					(int)Math.Abs((Math.Sqrt(attributeList.Count - 1 * 1))), 1, 0.5, 0.1, 0.001);

				int position = rnd.Next (0, numberFolds);

				while (usedFolds [position])
					position = rnd.Next (0, numberFolds);

				List<string> buildData = new List<string> ();

				foreach (var item in crossfoldedData.Where ((m, i) => !i.Equals (position)))
					buildData.AddRange (item);

 				string[][] filteredData = buildData.ConvertAll<string[]>(i => i.Split(new string[] { "," }, 
					StringSplitOptions.RemoveEmptyEntries) ).ToArray();

				string[][] testData = crossfoldedData[position].ToList().
					ConvertAll<string[]>(i => i.Split(new string[] { "," }, 
						StringSplitOptions.RemoveEmptyEntries) ).ToArray();

				execution.AppendLine ();
				execution.AppendFormat ("Building network for fold {0}.", fold + 1);
				execution.AppendLine ();

				// builds up network
				bpg.BuildNetwork (filteredData, attributeList.Take(attributeList.Count() -1).ToArray(), 
																								attributeList.Last());

				execution.AppendFormat ("Testing network for fold {0}.", fold + 1);
				execution.AppendLine ();

				// tests network
				bpg.TestNetwork (execution, testData, attributeList.Take(attributeList.Count() -1).ToArray(),
																								attributeList.Last());
			}

			return execution;
		}
	
		#endregion
	}
}