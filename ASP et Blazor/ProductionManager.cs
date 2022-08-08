using Holo_Recipe;
using static Holo_Recipe.Recipe;

namespace Test_ASP
{
    public sealed class ProductionManager
    {
        private static ProductionManager? instance = null;
        private static readonly object padlock = new();
        //Current caliper measure used in ARController
        public float Measure { get; set; }
        //recipe extracted from json file
        public Holo_Recipe.Recipe Recipe = new Holo_Recipe.Recipe();
        //Current cotation used in the sequence
        public PieceCotation CurrentCotation { get; set; } = new PieceCotation();
        //Current cotation index in recipe
        public static int RecipeIndex { get; set; } = 0;
        public string CurrentUser { get; set; } = "None";
        public string PieceName { get; set; } = "None";
        //measured piece -> will be serialized in the json file
        public MeasuredPiece MeasuredPiece { get; set; } = new MeasuredPiece();
        //Current measured cotation used in the sequence
        MeasuredCotation MeasuredCotation { get; set; } = new MeasuredCotation();
        public void Refresh(string? command)
        {
            if (command == "Start")
            {
                RecipeIndex = 0;
                //create a new measured piece
                MeasuredPiece = new MeasuredPiece
                {
                    CotationList = new List<MeasuredCotation>(),
                    PieceName = PieceName,
                    OperatorName = CurrentUser,
                    Id = Recipe.GetLastId() + 1,
                    StartDate = DateTime.Now.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR"))
                };
                CurrentCotation = Recipe.TemplatePiece.ElementAt(0);
                //fill cotation list with all known data
                for (int i = 0; i < Recipe.TemplatePiece.Count; i++)
                {
                    MeasuredCotation newCot = new()
                    {
                        ExpectedValue = Recipe.TemplatePiece.ElementAt(i).ExpectedValue,
                        Type = (MeasuredCotation.CotationType)Recipe.TemplatePiece.ElementAt(i).Type,
                        Id = i,
                        Tolerance = Recipe.TemplatePiece.ElementAt(i).Tolerance,
                        //default value
                        MeasuredValue = 999F
                    };
                    MeasuredPiece.CotationList.Add(newCot);
                }
            }
            else if (command == "Stop")
            {
                MeasuredPiece.EndDate = DateTime.Now.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR"));
                Recipe.SavePiece(MeasuredPiece);
            }
            else if (command == "Next")
            {
                if (RecipeIndex + 1 < Recipe.TemplatePiece.Count)
                {
                    if (MeasuredPiece.CotationList != null)
                    {
                        MeasuredCotation = MeasuredPiece.CotationList.ElementAt(RecipeIndex);
                        MeasuredPiece.CotationList.ElementAt(RecipeIndex).Result = (Measure <= MeasuredCotation.ExpectedValue + MeasuredCotation.Tolerance && Measure >= MeasuredCotation.ExpectedValue - MeasuredCotation.Tolerance);
                        MeasuredPiece.CotationList.ElementAt(RecipeIndex).MeasuredValue = Measure;
                    }
                    RecipeIndex++;
                    CurrentCotation = Recipe.TemplatePiece.ElementAt(RecipeIndex);
                }
                else
                {
                    MeasuredCotation = MeasuredPiece.CotationList!.ElementAt(RecipeIndex);
                    MeasuredPiece.CotationList!.ElementAt(RecipeIndex).Result = (Measure <= MeasuredCotation.ExpectedValue + MeasuredCotation.Tolerance && Measure >= MeasuredCotation.ExpectedValue - MeasuredCotation.Tolerance);
                    MeasuredPiece.CotationList!.ElementAt(RecipeIndex).MeasuredValue = Measure;
                    CurrentCotation.Id = -1;
                }
            }
            else if (command == "Previous")
            {
                if (RecipeIndex - 1 >= 0)
                {
                    RecipeIndex--;
                    CurrentCotation = Recipe.TemplatePiece.ElementAt(RecipeIndex);
                }
            }
            Console.WriteLine("Actual index: " + RecipeIndex.ToString());
        }
        private ProductionManager()
        {
        }

        public static ProductionManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ProductionManager();
                    }
                    return instance;
                }
            }
        }
        //save measure from controller in variable
        public void StoreMeasure(float newMeasure)
        {
            Measure = newMeasure;
        }
        public int GetCurrentID()
        {
            return RecipeIndex;
        }
    }
}
