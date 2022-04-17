namespace AppCore.Business.Models.Ordering
{
    public class OrderModel
    {
        public string Expression { get; set; }
        public bool DirectionAscending { get; set; } = true; // ascending (asc): artan, descending (desc): azalan
    }
}
