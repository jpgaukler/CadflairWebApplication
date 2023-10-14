namespace CadflairBlazorServer.Pages.McMaster_Idea.Models;

public static class DummyData
{
    private static List<Category> _categories = new()
    {
        new Category()
        {
            Name = "Flanges",
            Description = "This is a flange that is great for general use.",
            ChildCategories = new()
            {
                new Category()
                {
                    Name = "Stainless Steel Flanges",
                    Description = "This is a flange that is great for corrosion resistance.",
                    ProductDefinitions = new()
                    {
                        new ProductDefinition()
                        {
                            Id = 1,
                            Name = "304 Standard Corrosion Resistant Blind Flange",
                            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                        },
                        new ProductDefinition()
                        {
                            Id = 2,
                            Name = "316 Super Corrosion Resistant Blind Flange",
                            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                        },

                    }
                },
                new Category()
                {
                    Id = 3,
                    Name = "Carbon Steel Flanges",
                    Description = "This is a flange that is poor for corrosion resistance.",
                    ProductDefinitions = new()
                    {
                        new ProductDefinition()
                        {
                            Id = 3,
                            Name = "Black Oxide Blind Flange",
                            Description = "This is a flange that is coated in black oxide for mild corrosion resistance."
                        },
                        new ProductDefinition()
                        {
                            Id = 4,
                            Name = "Black Oxide Raised Flange",
                            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                        },
                    }
                },
            }
        },
    };

    private static List<ProductDefinition> _productDefinitions = new()
    {
        new ProductDefinition()
        {
            Id = 1,
            Name = "304 Standard Corrosion Resistant Blind Flange",
            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
        },
        new ProductDefinition()
        {
            Id = 2,
            Name = "316 Super Corrosion Resistant Blind Flange",
            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
        },
        new ProductDefinition()
        {
            Id = 3,
            Name = "Black Oxide Blind Flange",
            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
        },
        new ProductDefinition()
        {
            Id = 4,
            Name = "Black Oxide Raised Flange",
            Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
        },
    };

    private static ProductTable _productTable = new()
    {
        Id = 1,
        ColumnDefinitions = new()
        {
            new ColumnDefinition()
            {
                Id = 1,
                Header = "Diameter",
            },
            new ColumnDefinition()
            {
                Id = 2,
                Header = "Length",
            },
            new ColumnDefinition()
            {
                Id = 3,
                Header = "Bolt Hole Qty",
            },
            new ColumnDefinition()
            {
                Id = 4,
                Header = "Diameter",
            },
            new ColumnDefinition()
            {
                Id = 5,
                Header = "Length",
            },
            new ColumnDefinition()
            {
                Id = 6,
                Header = "Bolt Hole Qty",
            },
            new ColumnDefinition()
            {
                Id = 7,
                Header = "Diameter",
            },
            new ColumnDefinition()
            {
                Id = 8,
                Header = "Length",
            },
            new ColumnDefinition()
            {
                Id = 9,
                Header = "Bolt Hole Qty",
            },
            new ColumnDefinition()
            {
                Id = 10,
                Header = "Diameter",
            },
            new ColumnDefinition()
            {
                Id = 11,
                Header = "Length",
            },
            new ColumnDefinition()
            {
                Id = 12,
                Header = "Bolt Hole Qty",
            },
        },
    };

    public static List<Category> GetCategories()
    {
        return _categories;
    }

    public static List<ProductDefinition> GetProductDefinitions()
    {
        return _productDefinitions;
    }

    public static ProductTable GetProductTableByProductDefinitionId(int productDefinitionId)
    {
        return _productTable;
    }

}
