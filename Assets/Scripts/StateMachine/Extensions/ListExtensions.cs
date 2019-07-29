using System.Collections;

public static class ListExtensions 
{
    public static void ReorderItem(this IList collection, int currentIndex, int desiredIndex)
    {
        if (desiredIndex < 0)
        {
            throw new System.Exception(string.Format("DesiredIndex {0} is smaller than zero", desiredIndex));
        }
        else if (desiredIndex > collection.Count - 1)
        {
            throw new System.Exception(string.Format("DesiredIndex {0} is larger than collection range", desiredIndex));
        }

        object item = collection[currentIndex];
        collection.RemoveAt(currentIndex);
        collection.Insert(desiredIndex, item);
    }
}
