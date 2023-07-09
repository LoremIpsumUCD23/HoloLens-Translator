using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Util.Cache;


public class LRUCacheTests
{
    [Test]
    public void EvictionTest()
    {
        // Add items more than the capacity and checks if eviction works

        // Arrange
        ICache<string, string> c = new LRUCache<string, string>(5);
        c.Put("a", "1");
        c.Put("b", "2");
        c.Put("c", "3");
        c.Put("d", "4");
        c.Put("e", "5"); // # items reaches the capacity.
        c.Put("a", "6"); // "a" moves from the first to the end

        // Act
        c.Put("f", "7"); // "b" should be evicted

        // Assert
        Assert.AreEqual(5, c.Size());
        Assert.AreEqual(null, c.Get("b")); // Returns a default value (the default value for string is null).
    }

    [Test]
    public void SameKeyTest()
    {
        // Try to "put" a same key twice with different values
        // and make sure it first creates a pair and then updates
        // the associated value.

        // Arrange
        ICache<string, string> c = new LRUCache<string, string>(5);

        // Act
        c.Put("satoshi", "sugai");
        c.Put("satoshi", "nakamoto");

        // Assert
        Assert.AreEqual("nakamoto", c.Get("satoshi"));
    }

    private class Person
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

    [Test]
    public void CustomClassKeyNullTest()
    {
        // Try to use null as a key when key's type is a custome class
        // and it fails.

        // Arrange
        ICache<Person, string> c = new LRUCache<Person, string>(5);

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => c.Put(null, "test"));
    }

    [Test]
    public void CustomClassKeyEqualityTest()
    {
        // Try to put two different objects with the same attributes,
        // and make sure the class treat them as different things

        // Arrange. Create two objects with the same name.
        ICache<Person, string> c = new LRUCache<Person, string>(5);
        Person p1 = new Person("satoshi");
        Person p2 = new Person("satoshi");

        // Act
        c.Put(p1, "sugai");
        c.Put(p2, "nakamoto");

        // Assert
        Assert.IsFalse(c.Get(p1) == c.Get(p2));
        Assert.AreEqual(2, c.Size());
    }
}
