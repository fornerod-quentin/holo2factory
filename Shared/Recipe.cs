using Newtonsoft.Json;

namespace Holo_Recipe
{
    public class Recipe
    {
        public List<PieceCotation> TemplatePiece { get; set; } = new List<PieceCotation>();
        public List<MeasuredPiece> MeasuredPieces { get; set; } = new List<MeasuredPiece>();

        const string SavedPiecesFilename = "PiecesData.json";
        public class MeasuredPiece
        {
            public string? OperatorName { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? PieceName { get; set; }
            public int Id { get; set; }
            public List<MeasuredCotation>? CotationList { get; set; }
        }
        public class MeasuredCotation
        {
            public enum CotationType { Diameter, Rectilign, Hole }
            public int Id { get; set; } //make each cotation unique
            public float Tolerance { get; set; }
            public float ExpectedValue { get; set; }
            public float MeasuredValue { get; set; }

            public CotationType Type { get; set; }
            //OK/NOK
            public bool Result { get; set; }
        }
        public Recipe()
        {
            LoadSavedPieces();
        }
        private void LoadSavedPieces()
        {
            if (File.Exists(SavedPiecesFilename))
            {
                try
                {
                    string jsonString = File.ReadAllText(SavedPiecesFilename);
                    MeasuredPieces = JsonConvert.DeserializeObject<List<MeasuredPiece>>(jsonString)!;

                }
                catch (Newtonsoft.Json.JsonException e)
                {
                    Console.WriteLine(SavedPiecesFilename + " is corrupted");
                    Console.WriteLine(e.Message);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Unable to load the file");
                    Console.WriteLine(e.Message);
                }
            }
        }
        public int GetLastId()
        {
            if (!MeasuredPieces.Any())
            {
                return -1;
            }
            return MeasuredPieces.Count - 1;
        }
        public void SavePiece(MeasuredPiece measuredPiece)
        {
            MeasuredPieces.Add(measuredPiece);
            try
            {
                string jsonString = System.Text.Json.JsonSerializer.Serialize(MeasuredPieces);
                File.WriteAllText(SavedPiecesFilename, jsonString);
            }
            catch (Newtonsoft.Json.JsonException e)
            {
                Console.WriteLine("MeasuredPieces has incorrect format");
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to write in the file");
                Console.WriteLine(e.Message);
            }
            LoadSavedPieces();
        }
        public bool LoadDataPiece(string pieceName)
        {
            string fileName = pieceName + ".json";
            if (File.Exists(fileName))
            {
                try
                {
                    string jsonString = File.ReadAllText(fileName);
                    if (new FileInfo(fileName).Length == 0)
                    {
                        return false;
                    }
                    TemplatePiece = System.Text.Json.JsonSerializer.Deserialize<List<PieceCotation>>(jsonString)!;
                    return true;
                }
                catch (Newtonsoft.Json.JsonException e)
                {
                    Console.WriteLine(fileName + " is corrupted");
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to load the file");
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

    }
}
