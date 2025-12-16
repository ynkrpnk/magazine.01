using System;

namespace magazine._01
{
    public class MusicInstrument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }

        public MusicInstrument(int id, string name, string category, int price)
        {
            Id = id;
            Name = name;
            Category = category;
            Price = price;
        }

        public override string ToString()
        {
            return $"ID: {Id} | {Name} | {Price} грн";
        }
    }
}
