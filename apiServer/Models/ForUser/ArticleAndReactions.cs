namespace apiServer.Models.ForUser
{
    public class ArticleAndReactions
    {
        public Articles Articles { get; set; }
        public Emotions? Emotion { get; set; }
        public int CountReactions { get; set; }
    }
}
