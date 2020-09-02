Explanation of the CSV test files:

The CSV files contain the MNIST dataset in CSV format. This makes the whole processes simpler since we don't have to parse them 
ourselves.

Each row contains 785 values; the first value is the label for that number. The remaining 784 values are the int values for each pixel 
in the 28x28 grid. These are stored row by row.