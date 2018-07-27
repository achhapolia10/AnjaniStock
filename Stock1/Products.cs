using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock1
{
    class Products
    {
        string name;
        private string date;
        double weight;
        int boxCapacity;
        DateTime dateCreated;
        int packetCapacity;
        int stockedBoxes;
        int stockedPackets;
        double price;
        private int id;
        public string Name { get => name; set => name = value; }
        public double Weight { get => weight; set => weight = value; }
        public int BoxCapacity { get => boxCapacity; set=>boxCapacity = value; }
        public int PacketCapacity { get => packetCapacity; set => packetCapacity = value; }
        public int StockedBoxes1 { get => stockedBoxes; set => stockedBoxes = value; }
        public int StockedPackets { get => stockedPackets; set => stockedPackets = value; }
        public double Price { get => price; set => price = value; }
        public string Date { get => date; set => date = value; }
        public DateTime DateCreated { get => dateCreated; set => dateCreated = value; }
        public int Id { get => id; set => id = value; }

        public Products(string name,int boxCapacity,int stockedBoxes,
            int stockedPackets,string date,int id)
        {
            this.name = name;
            this.boxCapacity = boxCapacity;
            this.stockedBoxes = stockedBoxes;
            this.stockedPackets = stockedPackets;
            this.id = id;
            this.date = date;
            string[] dateSplit = date.Split(new char[] { '/' });
            this.DateCreated = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1])
                , Convert.ToInt32(dateSplit[0]));
        }
        public Products(string name, double weight, int boxCapacity, int packetCapacity, int stockedBoxes, int stockedPackets, double price,int id)
        {
            this.name = name;
            this.price = price;
            this.weight = weight;
            this.boxCapacity = boxCapacity;
            this.packetCapacity = packetCapacity;
            this.id = id;
            this.stockedBoxes = stockedBoxes;
            this.stockedPackets = stockedPackets;

        }

        public Products(string name, double weight, int boxCapacity, int packetCapacity,int id)
        {
            this.name = name;
            this.weight = weight;
            this.boxCapacity = boxCapacity;
            this.id = id;
            this.packetCapacity = packetCapacity;
            this.stockedBoxes = 0;
            this.stockedPackets = 0;
        }

        public void balancePackets()
        {
            int remainder = 0;
            remainder = stockedPackets % boxCapacity;
            stockedBoxes += stockedPackets / boxCapacity;
            stockedPackets = remainder;
        }
    }
}
