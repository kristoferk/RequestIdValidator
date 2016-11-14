using System;

namespace UnitTests
{
    public class SimpleIntObject
    {
        public int Id { get; set; } = 1;
    }

    public class NestedObject
    {
        public SimpleIntObject Child { get; set; } = new SimpleIntObject();
    }

    public class DeepNestedObject
    {
        public NestedObject Child { get; set; } = new NestedObject();
    }

    public class DeepDeepNestedObject
    {
        public DeepNestedObject Child { get; set; } = new DeepNestedObject();
    }

    public class SimpleGuidObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class SimpleStringObject
    {
        public string Id { get; set; } = "Test";
    }
}