# simple python script to combine the raw pixel array values (found in the mnist_train.csv file) with the computed histograms
# (found in the individual TrainedModels/model_{}.csv files)

import csv, getopt, sys

def validate_args(argv):
   csvloc = ''
   histoloc = ''
   try:
      opts, args = getopt.getopt(argv,"hi:o:",["ifile=","ofile="])
   except getopt.GetoptError:
      print('combine_script.py -i <csv pixel file loc> -o <histogram dir loc>')
      sys.exit(2)
   for opt, arg in opts:
      if opt == '-h':
         print('combine_script.py -i <csv pixel file loc> -o <histogram dir loc>')
         sys.exit()
      elif opt in ("-i", "--csv"):
         csvloc = arg
      elif opt in ("-o", "--histo"):
         histoloc = arg

   if(csvloc == '' or histoloc == ''):
      print('combine_script.py -i <csv pixel file loc> -o <histogram dir loc>')
      sys.exit(2)

   return (csvloc, histoloc)

def main(train_file_loc, histo_dir_loc):

    # not exactly efficient . . .
    histo_dict = {
        '0' : csv.reader(open(histo_dir_loc + '\model_0.csv'), delimiter=','),
        '1' : csv.reader(open(histo_dir_loc + '\model_1.csv'), delimiter=','),
        '2' : csv.reader(open(histo_dir_loc + '\model_2.csv'), delimiter=','),
        '3' : csv.reader(open(histo_dir_loc + '\model_3.csv'), delimiter=','),
        '4' : csv.reader(open(histo_dir_loc + '\model_4.csv'), delimiter=','),
        '5' : csv.reader(open(histo_dir_loc + '\model_5.csv'), delimiter=','),
        '6' : csv.reader(open(histo_dir_loc + '\model_6.csv'), delimiter=','),
        '7' : csv.reader(open(histo_dir_loc + '\model_7.csv'), delimiter=','),
        '8' : csv.reader(open(histo_dir_loc + '\model_8.csv'), delimiter=','),
        '9' : csv.reader(open(histo_dir_loc + '\model_9.csv'), delimiter=','),
    }

    result_dict = {
        '0' : csv.writer(open('combined_model_0.csv', 'w+', newline=''), delimiter=','),
        '1' : csv.writer(open('combined_model_1.csv', 'w+', newline=''), delimiter=','),
        '2' : csv.writer(open('combined_model_2.csv', 'w+', newline=''), delimiter=','),
        '3' : csv.writer(open('combined_model_3.csv', 'w+', newline=''), delimiter=','),
        '4' : csv.writer(open('combined_model_4.csv', 'w+', newline=''), delimiter=','),
        '5' : csv.writer(open('combined_model_5.csv', 'w+', newline=''), delimiter=','),
        '6' : csv.writer(open('combined_model_6.csv', 'w+', newline=''), delimiter=','),
        '7' : csv.writer(open('combined_model_7.csv', 'w+', newline=''), delimiter=','),
        '8' : csv.writer(open('combined_model_8.csv', 'w+', newline=''), delimiter=','),
        '9' : csv.writer(open('combined_model_9.csv', 'w+', newline=''), delimiter=','),
    }

    with open(train_file_loc) as train_file:
        csv_reader = csv.reader(train_file, delimiter=',')
        for row in csv_reader:
            label = row[0]
            histo = next(histo_dict[label])
            result_writer = result_dict[label]
            result_writer.writerow(row[1:])
            result_writer.writerow(histo)

def compare_lines(histo_dir_loc, data_dir):
    with open(histo_dir_loc + '\model_7.csv') as histo, open(data_dir + '\combined_model_7.csv') as combined:
        histo_reader = csv.reader(histo, delimiter=',');
        combined_reader = csv.reader(combined, delimiter=',');

        for row in histo_reader:
            next(combined_reader) # to skip a line
            combined_histo = next(combined_reader)

            for i in range(0, len(row)):
                assert row[i] == combined_histo[i]
 
            
if __name__ == "__main__":
    csv_loc, histo_dir = validate_args(sys.argv[1:])
    #main(csv_loc, histo_dir)
    compare_lines(csv_loc, 'Data')
