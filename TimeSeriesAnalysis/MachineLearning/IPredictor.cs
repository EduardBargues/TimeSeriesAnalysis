using System.Collections.Generic;
using System.Data;

namespace MachineLearning
{
    public interface IPredictor
    {
        void Learn(DataTable table, IEnumerable<string> trainingLabels, string targetLabel);
        void Learn(DataTable table, string trainingLabel, string targetLabel);
        object Predict(DataRow row);
    }
}
