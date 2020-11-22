/*
 * www.gso-koeln.de 2020
 */
namespace CityLibrary.Model
{
    public class Item
    {
        private int id; // --> see OnModelCreating(...)
        public int Id { get { return id; } } // readonly access
        public ItemState State { get; set; }
        public string StorageLocation { get; set; }
    }
    public enum ItemState { Usable, Ordered, Defect }
}
