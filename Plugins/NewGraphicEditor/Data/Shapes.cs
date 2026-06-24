namespace NewGraphicEditor.Data
{
  public abstract class Shapes
  {
    public abstract int[] point { get; }
    protected abstract int countPoint { get; }
    public virtual string nameShapes => string.Empty;
    public string NameShape { get; set; }
    public string Info { get; set; }
  }
}
