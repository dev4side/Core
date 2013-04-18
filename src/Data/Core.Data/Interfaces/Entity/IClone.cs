namespace Core.Data.Interfaces.Entity
{
    public interface IClone<out T>
    {
        T CloneAsNew();
    }
}
