public class EvilUi
{
    public EvilUi()
    {
    }

    public void EvilUIMethod()
    {
        // hello
        throw new NotImplementedException("cannot be called from a unit test - this method will open a UI window.");
    }
}