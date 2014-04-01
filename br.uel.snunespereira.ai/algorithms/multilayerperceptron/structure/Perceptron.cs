using System;

namespace br.uel.snunespereira.ai
{
	public class Perceptron
	{
		/// <summary>
		/// Gets or sets the type of the output activation.
		/// </summary>
		/// <param name="x">Value</param>
		private delegate double ExecuteFunctionDelegate (double x);

		/// <summary>
		/// The execute function.
		/// </summary>
		private ExecuteFunctionDelegate ExecuteFunction;

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		private PerceptronType Type { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="br.uel.snunespereira.ai.Perceptron"/> class.
		/// </summary>
		/// <param name="perceptronType">Perceptron type.</param>
		public Perceptron(PerceptronType perceptronType)
		{
			this.Type = perceptronType;

			switch (perceptronType) {
				case PerceptronType.Input:
					ExecuteFunction = null;
					break;
				case PerceptronType.Hidden:
					ExecuteFunction += HyperTanFunction;
					break;
				case PerceptronType.Output:
					ExecuteFunction += SigmoidFunction;
					break;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="br.uel.snunespereira.ai.Perceptron"/> class.
		/// </summary>
		/// <param name="perceptronType">Perceptron type.</param>
		public Perceptron (PerceptronType percepctronType, FunctionType functionType)
		{
			this.Type = percepctronType;

			if (functionType == FunctionType.LogSigmoid)
				ExecuteFunction += SigmoidFunction;
			else if (functionType == FunctionType.Tanh)
				ExecuteFunction += HyperTanFunction;
		}

		/// <summary>
		/// Computes the outputs.
		/// </summary>
		/// <returns>The outputs.</returns>
		/// <param name="xValues">X values.</param>
		public double ComputeOutput(double value)
		{
			return 0;
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
	}
}

