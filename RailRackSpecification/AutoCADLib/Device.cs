using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCADLib
{   
    class Device
    {
        private string name;
        private int quantity = 1;
        public Device(string name) {
            this.name = name;
        }
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
        public int Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                quantity = value;
            }
        }
        public void AddDevice() {
            quantity++;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;
            Device device = (Device)obj;
            return (this.name == device.name);
        }
        public override string ToString()
        {
            return name + ": " + quantity;
        }
    }
}