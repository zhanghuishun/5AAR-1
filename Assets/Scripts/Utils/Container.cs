public class Container<T>
{
    public T content;
    
    public Container(T cont)
    {
        content = cont;
    }

    public Container()
    {
        content = default(T);
    }

    public override string ToString()
    {
        return content.ToString();
    }
}
