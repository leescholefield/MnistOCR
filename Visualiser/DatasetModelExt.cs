using Visualiser.Dataset;

namespace Visualiser
{
    public class DatasetModelExt : DatasetModel
    {

        public double Distance { get; set; }

        public static DatasetModelExt CopyFromDatasetModel(DatasetModel model)
        {
            return new DatasetModelExt { Histogram = model.Histogram, PixelMatrix = model.PixelMatrix };
        }
    }
}
