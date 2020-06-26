namespace DataAccessLibrary.Models
{
    public class WindowOptionModel
    {
        /// <summary> The unique identifier. </summary>
        public int Id { get; set; }

        /// <summary> The name of the option. </summary>
        public string Name { get; set; }

        /// <summary> The description of the option </summary>
        public string Description { get; set; }

        /// <summary> The base price of the option. </summary>
        public decimal BasePrice { get; set; }
    }
}
