using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class Person
    {
        private string _name;
        public Person(string name)
        {
            this._name = name;
        }

        public string Name    // the Name property
        {
            get => _name;
            set => _name = value;
        }
    }
}