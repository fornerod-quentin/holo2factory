using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace Test_ASP.Controllers
{
    [System.Serializable]
    public class SingleMeasure
    {
        public float Measure { get; set; }
    }
    [System.Serializable]
    public class SingleString
    {
        public string? Name { get; set; }
    }
    // not implemented
    public class CaliperStatus
    {
        public bool ConnectionStatus { get; set; } = false;
    }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ARController : ControllerBase
    {
        PieceCotation CurrentCotation { get; set; } = new PieceCotation();
        public static float CurrentMeasure { get; set; } = 999F;
        SingleMeasure CaliperMeasure { get; set; } = new SingleMeasure();
        //Singleton instance
        ProductionManager ProductionManager = ProductionManager.Instance;


        public ARController()
        {

        }

        //************************ GET METHODS **********************
        [HttpGet]
        [ActionName("GetMeasure")]
        public ActionResult GetMeasure()
        {
            CaliperMeasure.Measure = CurrentMeasure;
            ProductionManager.StoreMeasure(CurrentMeasure);
            return Ok(CaliperMeasure);
        }

        //Get the list of all measured pieces (Request "normally" done by the Blazor server)
        [HttpGet]
        [ActionName("GetMesuredPieces")]
        public ActionResult GetMesuredPieces()
        {
            return Ok(ProductionManager.Recipe.MeasuredPieces);
        }

        [HttpGet]
        [ActionName("Ping")]
        public ActionResult Ping()
        {
            return Ok();
        }
        //************************ put METHODS **********************

        [HttpPut]
        [ActionName("BooleanCommand")]
        public ActionResult HoloCommand(BooleanResponse response)
        {
            //default value
            CurrentMeasure = 999F;
            ProductionManager.Refresh(response.Name);
            if (ProductionManager.MeasuredPiece.CotationList != null)
            {
                if (ProductionManager.MeasuredPiece.CotationList.Count > ProductionManager.GetCurrentID())
                {
                    CurrentMeasure = ProductionManager.MeasuredPiece.CotationList.ElementAt(ProductionManager.GetCurrentID()).MeasuredValue;
                }
            }
            CurrentCotation = ProductionManager.CurrentCotation;
            return Ok(CurrentCotation);
        }
        //Choose correct recipe upon piece name, NotFound if unknown
        [HttpPut]
        [ActionName("PieceSelection")]
        public ActionResult SetRecipe(SingleString pieceName)
        {
            if (ProductionManager.Recipe.LoadDataPiece(pieceName.Name!))
            {
                ProductionManager.PieceName = pieceName.Name!;
                return Ok();
            }
            return NotFound();
        }
        //Set operator name, NoContent if no name
        [HttpPut]
        [ActionName("SelectUser")]
        public ActionResult SetOperator(SingleString operatorName)
        {
            if (operatorName.Name != null)
            {
                ProductionManager.CurrentUser = operatorName.Name;
                return Ok();
            }
            return NoContent();
        }
        //*********************************************************************
        //****************** METHODES PUT L'APPLICATION PC WPF ****************
        //******************    APP WPF -> Controller          ****************
        //*********************************************************************

        [HttpPut]
        [ActionName("PutMeasure")]
        public ActionResult Put(SingleMeasure measure)
        {
            if (measure != null)
            {
                CurrentMeasure = measure.Measure;
                if (ProductionManager.MeasuredPiece.CotationList is not null)
                    ProductionManager.MeasuredPiece.CotationList.ElementAt(ProductionManager.GetCurrentID()).MeasuredValue = measure.Measure;
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        [ActionName("CaliperStatus")]
        public void CaliperStatus(SingleString caliperStatus)
        {
            Console.WriteLine(caliperStatus);
        }
    }
}
