namespace apiServer.Models.ForUser
{
    public class FullArticle<T>
    {
        public T Articles { get; set; }
        public Emotions? Emotion { get; set; }
        public bool Selected { get; set; }
        public int CountReactions { get; set; }
    }
}
