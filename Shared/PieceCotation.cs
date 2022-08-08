[System.Serializable]
public class PieceCotation
{
    public enum CotationType { Diameter, Rectilign, Hole }
    public int Id { get; set; } //make each cotation unique
    public float Tolerance { get; set; }
    public float ExpectedValue { get; set; }
    public CotationType Type { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float Rx { get; set; }
    public float Ry { get; set; }
    public float Rz { get; set; }
}

