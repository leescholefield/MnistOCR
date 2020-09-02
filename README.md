# MnistOCR
An OCR program written in C# for use with the MNIST dataset of handwritten characters.

# Method

The process is as follows:

    1. Split the 28x28 digit image into 16 7x7 sections.
    2. Perform the [Local Binary Pattern](https://en.wikipedia.org/wiki/Local_binary_patterns) to generate a 
       greyscale histogram for each of the 16 sections.
    3. Calculate the [K-Nearest Neighbours](https://en.wikipedia.org/wiki/K-nearest_neighbors_algorithm) using Euclidian distance.
    4. Guess which digit the unknown image is based on it's nearest neighbours.
   
In order to save time, during the training processes the sectioned histogram for each image is dumped to file along with other histograms of the same digit. This allows us to skip this stage 
during the testing process.

Also, instead of spending time writting a custom parser for the MNIST raw images I have used a pre-parsed CSV file. This contains a label (if it's a training image) and the greyscale 
pixel value of each of the 784 pixels in the image. The files can be found here: 

https://pjreddie.com/projects/mnist-in-csv/

# Results

For the 10,000 image testing set with a K=3.

| Trained sample size*| Errors     | Error rate % | Runtime**|
| ------------------- | -----------|--------------| --------|
| 100                 | 1658       | 16.58        | 3.41    |
| 1000                | 770        | 7.7          | 8.54    |
| 1500                | 695        | 6.95         | 11.42   |
| 2000                | 640        | 6.4          | 15.58   |
| 2500                | 590        | 5.89         | 18.41   |
| 3000                | 578        | 5.77         | 37.34   |
| 5000                | 521        | 5.21         | 1:02:24 |

\* this is the number of histograms I have taken for each digit from the training data. So a sample size of 100 means I am comparing the test image to 100 examples of each digit.

\** on a Lenovo Legion Y520 Intel Core i5-7300HQ. The timer starts after the trained data has been loaded into memory so the total program runtime will be longer.

# Next Steps

The next step I'm going to take is some pre-processing of the image data (primarilly deskewing) to hopefully bring this down. Another problem I'm hoping to deal with is 
the long run-time, which currently stands at 1 hour 2 minutes for a set of 50,000 pre-processed histograms (5,000 for each digit). 
