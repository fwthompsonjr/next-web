namespace next.processor.models
{
    public class DrillDownModel
    {
        public string Name { get; set; } = "SUBMITTED";
        public int Id
        {
            get
            {
                var items = "ERROR,SUBMITTED,PURCHASED".Split(',').ToList();
                var index = items.IndexOf(Name);
                return index;
            }
        }
    }
}
