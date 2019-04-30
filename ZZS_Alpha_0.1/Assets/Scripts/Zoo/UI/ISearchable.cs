namespace Zoo.UI
{
    public interface ISearchable
    {
        /// <summary>
        /// Get tags by which this item can be searched.
        /// </summary>
        string[] GetSearchTags();
    }
}
